<script lang="ts">
	import { api } from "$lib/services/api";
	import { Pencil, X } from "@lucide/svelte";
	import { Dialog } from "bits-ui";
	import toast from "svelte-french-toast";
	import axios from "axios";
	import type { DetailedScheduleResponse } from "$lib/types/responses/detailedScheduleResponse";

	type Props = {
		scheduleId: string;
		schedule: DetailedScheduleResponse;
		open: boolean;
		onSuccess: () => void;
	};

	let { scheduleId, schedule, open = $bindable(), onSuccess }: Props = $props();

	let form = $state({
		name: schedule.name,
		description: schedule.description,
		location: schedule.location
	});
	let errors = $state<Record<string, string>>({});
	let saving = $state(false);

	$effect(() => {
		if (open) {
			form = {
				name: schedule.name,
				description: schedule.description,
				location: schedule.location
			};
			errors = {};
		}
	});

	async function submit() {
		saving = true;
		errors = {};
		try {
			await api.put(`/schedules/${scheduleId}`, form);
			toast.success("Expedition updated");
			open = false;
			onSuccess();
		} catch (error) {
			if (axios.isAxiosError(error)) {
				const e = error?.response?.data?.error;
				if (e && typeof e === "object") {
					errors = e;
					if (e["general"]) toast.error(e["general"]);
				} else {
					toast.error("Failed to update expedition");
				}
			}
		} finally {
			saving = false;
		}
	}
</script>

<Dialog.Root bind:open>
	<Dialog.Portal>
		<Dialog.Overlay
			class="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm data-[state=closed]:animate-[fade-out_150ms] data-[state=open]:animate-[fade-in_150ms]"
		/>
		<Dialog.Content
			class="fixed top-1/2 left-1/2 z-50 w-full max-w-md -translate-x-1/2 -translate-y-1/2 rounded-2xl border border-white/10 bg-[#0a0d1a] p-6 shadow-2xl outline-none data-[state=closed]:animate-[fade-out_150ms] data-[state=open]:animate-[fade-in_150ms]"
		>
			<div class="mb-5 flex items-start justify-between">
				<Dialog.Title class="text-base font-semibold text-white">Edit Expedition</Dialog.Title>
				<Dialog.Close
					class="rounded-lg p-1 text-white/30 transition hover:bg-white/10 hover:text-white/70"
				>
					<X class="h-4 w-4" />
				</Dialog.Close>
			</div>

			<form
				onsubmit={(e) => {
					e.preventDefault();
					submit();
				}}
				class="space-y-4"
			>
				<div>
					<label class="mb-1.5 block text-xs font-medium text-white/50" for="update-name">
						Expedition Name
					</label>
					<input
						id="update-name"
						type="text"
						bind:value={form.name}
						required
						class="w-full rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 transition outline-none focus:bg-white/8 {errors.name
							? 'border-red-500/60 focus:border-red-500/80'
							: 'border-white/10 focus:border-blue-500/40'}"
					/>
					{#if errors.name}
						<p class="mt-1 text-xs text-red-400">{errors.name}</p>
					{/if}
				</div>

				<div>
					<label class="mb-1.5 block text-xs font-medium text-white/50" for="update-description">
						Description
					</label>
					<textarea
						id="update-description"
						bind:value={form.description}
						rows="2"
						class="w-full resize-none rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 transition outline-none focus:bg-white/8 {errors.description
							? 'border-red-500/60 focus:border-red-500/80'
							: 'border-white/10 focus:border-blue-500/40'}"
					></textarea>
					{#if errors.description}
						<p class="mt-1 text-xs text-red-400">{errors.description}</p>
					{/if}
				</div>

				<div>
					<label class="mb-1.5 block text-xs font-medium text-white/50" for="update-location">
						Location
					</label>
					<input
						id="update-location"
						type="text"
						bind:value={form.location}
						required
						class="w-full rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 transition outline-none focus:bg-white/8 {errors.location
							? 'border-red-500/60 focus:border-red-500/80'
							: 'border-white/10 focus:border-blue-500/40'}"
					/>
					{#if errors.location}
						<p class="mt-1 text-xs text-red-400">{errors.location}</p>
					{/if}
				</div>

				<div class="flex justify-end gap-2 pt-1">
					<Dialog.Close
						class="rounded-lg border border-white/10 px-4 py-2 text-sm text-white/50 transition hover:bg-white/5 hover:text-white/70"
					>
						Cancel
					</Dialog.Close>
					<button
						type="submit"
						disabled={saving}
						class="flex items-center gap-1.5 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-50"
					>
						{#if saving}
							<div
								class="h-3.5 w-3.5 animate-spin rounded-full border-2 border-white/30 border-t-white"
							></div>
						{:else}
							<Pencil class="h-3.5 w-3.5" />
						{/if}
						Save Changes
					</button>
				</div>
			</form>
		</Dialog.Content>
	</Dialog.Portal>
</Dialog.Root>
