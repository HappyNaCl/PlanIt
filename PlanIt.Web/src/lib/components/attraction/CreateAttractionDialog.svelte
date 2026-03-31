<script lang="ts">
	import { api } from "$lib/services/api";
	import { Plus, X, ImagePlus, AlertCircle } from "@lucide/svelte";
	import { Dialog } from "bits-ui";
	import toast from "svelte-french-toast";
	import axios from "axios";

	type Props = {
		scheduleId: string;
		open: boolean;
		onSuccess: () => void;
	};

	let { scheduleId, open = $bindable(), onSuccess }: Props = $props();

	let form = $state({ name: "", description: "", capacity: 1 });
	let errors = $state<Record<string, string>>({});
	let saving = $state(false);

	let imageFile = $state<File | null>(null);
	let imagePreview = $state<string | null>(null);
	let imageError = $state<string | null>(null);

	$effect(() => {
		if (open) {
			form = { name: "", description: "", capacity: 1 };
			errors = {};
			imageFile = null;
			imagePreview = null;
			imageError = null;
		}
	});

	function onFileChange(e: Event) {
		const input = e.currentTarget as HTMLInputElement;
		const file = input.files?.[0] ?? null;
		imageError = null;

		if (!file) {
			imageFile = null;
			imagePreview = null;
			return;
		}

		if (!file.type.startsWith("image/")) {
			imageError = "Only image files are accepted.";
			imageFile = null;
			imagePreview = null;
			input.value = "";
			return;
		}

		imageFile = file;
		imagePreview = URL.createObjectURL(file);
	}

	function removeImage() {
		imageFile = null;
		if (imagePreview) URL.revokeObjectURL(imagePreview);
		imagePreview = null;
		imageError = null;
	}

	async function submit() {
		saving = true;
		errors = {};
		try {
			const body = new FormData();
			body.append("name", form.name);
			body.append("description", form.description);
			body.append("capacity", String(form.capacity));
			if (imageFile) body.append("imageFile", imageFile);

			await api.post(`/schedules/${scheduleId}/attractions`, body, {
				headers: { "Content-Type": "multipart/form-data" }
			});
			toast.success("Waypoint added");
			open = false;
			onSuccess();
		} catch (error) {
			if (axios.isAxiosError(error)) {
				const e = error?.response?.data?.error;
				if (e && typeof e === "object") {
					errors = e;
					if (e["general"]) toast.error(e["general"]);
				} else {
					toast.error("Failed to add waypoint");
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
				<Dialog.Title class="text-base font-semibold text-white">Add Waypoint</Dialog.Title>
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
					<label class="mb-1.5 block text-xs font-medium text-white/50" for="attr-name">
						Name
					</label>
					<input
						id="attr-name"
						type="text"
						bind:value={form.name}
						required
						placeholder="City Creek Canyon"
						class="w-full rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 placeholder-white/20 transition outline-none focus:bg-white/8 {errors.name
							? 'border-red-500/60 focus:border-red-500/80'
							: 'border-white/10 focus:border-blue-500/40'}"
					/>
					{#if errors.name}
						<p class="mt-1 text-xs text-red-400">{errors.name}</p>
					{/if}
				</div>

				<div>
					<label class="mb-1.5 block text-xs font-medium text-white/50" for="attr-description">
						Description
					</label>
					<textarea
						id="attr-description"
						bind:value={form.description}
						rows="2"
						placeholder="A brief description of this stop..."
						class="w-full resize-none rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 placeholder-white/20 transition outline-none focus:bg-white/8 {errors.description
							? 'border-red-500/60 focus:border-red-500/80'
							: 'border-white/10 focus:border-blue-500/40'}"
					></textarea>
					{#if errors.description}
						<p class="mt-1 text-xs text-red-400">{errors.description}</p>
					{/if}
				</div>

				<div>
					<label class="mb-1.5 block text-xs font-medium text-white/50" for="attr-capacity">
						Capacity
					</label>
					<input
						id="attr-capacity"
						type="number"
						bind:value={form.capacity}
						min="1"
						required
						class="w-full rounded-lg border bg-white/5 px-3 py-2 text-sm text-white/80 transition outline-none focus:bg-white/8 {errors.capacity
							? 'border-red-500/60 focus:border-red-500/80'
							: 'border-white/10 focus:border-blue-500/40'}"
					/>
					{#if errors.capacity}
						<p class="mt-1 text-xs text-red-400">{errors.capacity}</p>
					{/if}
				</div>

				<!-- Image upload -->
				<div>
					<p class="mb-1.5 block text-xs font-medium text-white/50">Image</p>

					{#if imagePreview}
						<div class="relative overflow-hidden rounded-lg border border-white/10">
							<img src={imagePreview} alt="Preview" class="h-40 w-full object-cover" />
							<button
								type="button"
								onclick={removeImage}
								class="absolute top-2 right-2 rounded-lg bg-black/60 p-1 text-white/70 transition hover:bg-black/80 hover:text-white"
							>
								<X class="h-4 w-4" />
							</button>
						</div>
					{:else}
						<label
							for="attr-image"
							class="flex cursor-pointer flex-col items-center justify-center gap-2 rounded-lg border border-dashed bg-white/3 px-4 py-8 transition hover:bg-white/5 {imageError
								? 'border-red-500/40'
								: 'border-white/10'}"
						>
							<ImagePlus class="h-6 w-6 text-white/20" />
							<span class="text-xs text-white/30">Click to upload an image</span>
							<span class="text-xs text-white/20">PNG, JPG, WEBP, etc.</span>
						</label>
						<input
							id="attr-image"
							type="file"
							accept="image/*"
							onchange={onFileChange}
							class="sr-only"
						/>
					{/if}

					{#if imageError}
						<p class="mt-1 flex items-center gap-1 text-xs text-red-400">
							<AlertCircle class="h-3.5 w-3.5 shrink-0" />
							{imageError}
						</p>
					{/if}
					{#if errors.imageFile}
						<p class="mt-1 text-xs text-red-400">{errors.imageFile}</p>
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
							<Plus class="h-3.5 w-3.5" />
						{/if}
						Add Waypoint
					</button>
				</div>
			</form>
		</Dialog.Content>
	</Dialog.Portal>
</Dialog.Root>
