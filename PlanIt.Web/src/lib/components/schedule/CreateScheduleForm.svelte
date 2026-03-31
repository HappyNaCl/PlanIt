<script lang="ts">
	import { api, type ApiError } from "$lib/services/api";
	import { Plus } from "@lucide/svelte";
	import axios from "axios";
	import { Dialog } from "bits-ui";
	import toast from "svelte-french-toast";

	type Props = {
		date: string;
		onSuccess: () => void;
	};

	let { date, onSuccess }: Props = $props();

	function defaultForm() {
		return {
			name: "",
			description: "",
			location: "",
			startTime: `${date}T09:00`,
			endTime: `${date}T17:00`
		};
	}

	let form = $state(defaultForm());
	let creating = $state(false);
	let formErrors = $state<Record<string, string>>({});

	$effect(() => {
		form.startTime = `${date}T09:00`;
		form.endTime = `${date}T17:00`;
	});

	export function reset() {
		form = defaultForm();
		formErrors = {};
	}

	async function submit() {
		creating = true;
		formErrors = {};
		try {
			await api.post("/schedules", {
				name: form.name,
				description: form.description,
				location: form.location,
				startTime: new Date(form.startTime).toISOString(),
				endTime: new Date(form.endTime).toISOString()
			});
			toast.success("Expedition charted successfully");
			onSuccess();
		} catch (error) {
			if (axios.isAxiosError<ApiError>(error)) {
				const errors = error?.response?.data?.error;
				if (errors && typeof errors === "object") {
					formErrors = errors;
					if (errors["general"]) {
						toast.error(errors["general"]);
					}
				} else {
					toast.error(`Failed to chart expedition: ${error}`);
				}
			}
		} finally {
			creating = false;
		}
	}
</script>

<form
	onsubmit={(e) => {
		e.preventDefault();
		submit();
	}}
	class="space-y-4"
>
	<div>
		<label class="mb-1.5 block text-xs font-medium text-white/50" for="name">
			Expedition Name
		</label>
		<input
			id="name"
			type="text"
			bind:value={form.name}
			required
			placeholder="Morning Market Run"
			class="w-full rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 placeholder-white/20 transition outline-none focus:bg-white/8 {formErrors.name
				? 'border-red-500/60 focus:border-red-500/80'
				: 'border-white/10 focus:border-blue-500/40'}"
		/>
		{#if formErrors.name}
			<p class="mt-1 text-xs text-red-400">{formErrors.name}</p>
		{/if}
	</div>

	<div>
		<label class="mb-1.5 block text-xs font-medium text-white/50" for="description">
			Description
		</label>
		<textarea
			id="description"
			bind:value={form.description}
			required
			rows="2"
			placeholder="A brief description of the outing..."
			class="w-full resize-none rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 placeholder-white/20 transition outline-none focus:bg-white/8 {formErrors.description
				? 'border-red-500/60 focus:border-red-500/80'
				: 'border-white/10 focus:border-blue-500/40'}"
		></textarea>
		{#if formErrors.description}
			<p class="mt-1 text-xs text-red-400">{formErrors.description}</p>
		{/if}
	</div>

	<div>
		<label class="mb-1.5 block text-xs font-medium text-white/50" for="location"> Location </label>
		<input
			id="location"
			type="text"
			bind:value={form.location}
			required
			placeholder="Salt Lake City, UT"
			class="w-full rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 placeholder-white/20 transition outline-none focus:bg-white/8 {formErrors.location
				? 'border-red-500/60 focus:border-red-500/80'
				: 'border-white/10 focus:border-blue-500/40'}"
		/>
		{#if formErrors.location}
			<p class="mt-1 text-xs text-red-400">{formErrors.location}</p>
		{/if}
	</div>

	<div class="grid grid-cols-2 gap-3">
		<div>
			<label class="mb-1.5 block text-xs font-medium text-white/50" for="startTime">
				Launch Time
			</label>
			<input
				id="startTime"
				type="datetime-local"
				bind:value={form.startTime}
				required
				class="w-full rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 scheme-dark transition outline-none {formErrors.startTime
					? 'border-red-500/60 focus:border-red-500/80'
					: 'border-white/10 focus:border-blue-500/40'}"
			/>
			{#if formErrors.startTime}
				<p class="mt-1 text-xs text-red-400">{formErrors.startTime}</p>
			{/if}
		</div>
		<div>
			<label class="mb-1.5 block text-xs font-medium text-white/50" for="endTime">
				Landing Time
			</label>
			<input
				id="endTime"
				type="datetime-local"
				bind:value={form.endTime}
				required
				class="w-full rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 scheme-dark transition outline-none {formErrors.endTime
					? 'border-red-500/60 focus:border-red-500/80'
					: 'border-white/10 focus:border-blue-500/40'}"
			/>
			{#if formErrors.endTime}
				<p class="mt-1 text-xs text-red-400">{formErrors.endTime}</p>
			{/if}
		</div>
	</div>

	<div class="flex justify-end gap-2 pt-1">
		<Dialog.Close
			class="rounded-lg border border-white/10 px-4 py-2 text-sm text-white/50 transition hover:bg-white/5 hover:text-white/70"
		>
			Cancel
		</Dialog.Close>
		<button
			type="submit"
			disabled={creating}
			class="flex items-center gap-1.5 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-50"
		>
			{#if creating}
				<div
					class="h-3.5 w-3.5 animate-spin rounded-full border-2 border-white/30 border-t-white"
				></div>
			{:else}
				<Plus class="h-3.5 w-3.5" />
			{/if}
			Chart It
		</button>
	</div>
</form>
