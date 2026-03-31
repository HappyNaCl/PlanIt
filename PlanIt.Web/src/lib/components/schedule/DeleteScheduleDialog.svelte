<script lang="ts">
	import { api } from '$lib/services/api';
	import { Trash2, X } from '@lucide/svelte';
	import { Dialog } from 'bits-ui';
	import toast from 'svelte-french-toast';
	import { useQueryClient } from '@sveltestack/svelte-query';

	type Props = {
		scheduleId: string;
		open: boolean;
		onSuccess: () => void;
	};

	let { scheduleId, open = $bindable(), onSuccess }: Props = $props();

	const queryClient = useQueryClient();
	let deleting = $state(false);

	async function confirm() {
		deleting = true;
		try {
			await api.delete(`/schedules/${scheduleId}`);
			toast.success('Expedition deleted');
			open = false;
			queryClient.invalidateQueries(['schedules']);
			onSuccess();
		} catch {
			toast.error('Failed to delete expedition');
			deleting = false;
		}
	}
</script>

<Dialog.Root bind:open>
	<Dialog.Portal>
		<Dialog.Overlay
			class="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm data-[state=closed]:animate-[fade-out_150ms] data-[state=open]:animate-[fade-in_150ms]"
		/>
		<Dialog.Content
			class="fixed top-1/2 left-1/2 z-50 w-full max-w-sm -translate-x-1/2 -translate-y-1/2 rounded-2xl border border-white/10 bg-[#0a0d1a] p-6 shadow-2xl outline-none data-[state=closed]:animate-[fade-out_150ms] data-[state=open]:animate-[fade-in_150ms]"
		>
			<div class="mb-1 flex items-start justify-between">
				<Dialog.Title class="text-base font-semibold text-white">Delete Expedition</Dialog.Title>
				<Dialog.Close
					class="rounded-lg p-1 text-white/30 transition hover:bg-white/10 hover:text-white/70"
				>
					<X class="h-4 w-4" />
				</Dialog.Close>
			</div>
			<Dialog.Description class="mb-6 text-sm text-white/40">
				This expedition will be permanently removed. This action cannot be undone.
			</Dialog.Description>
			<div class="flex justify-end gap-2">
				<Dialog.Close
					class="rounded-lg border border-white/10 px-4 py-2 text-sm text-white/50 transition hover:bg-white/5 hover:text-white/70"
				>
					Cancel
				</Dialog.Close>
				<button
					onclick={confirm}
					disabled={deleting}
					class="flex items-center gap-1.5 rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-red-500 disabled:cursor-not-allowed disabled:opacity-50"
				>
					{#if deleting}
						<div class="h-3.5 w-3.5 animate-spin rounded-full border-2 border-white/30 border-t-white"></div>
					{:else}
						<Trash2 class="h-3.5 w-3.5" />
					{/if}
					Delete
				</button>
			</div>
		</Dialog.Content>
	</Dialog.Portal>
</Dialog.Root>
