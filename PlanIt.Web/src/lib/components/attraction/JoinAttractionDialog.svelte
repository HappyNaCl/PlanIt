<script lang="ts">
	import { CheckCircle, XCircle, X } from '@lucide/svelte';
	import { Dialog } from 'bits-ui';

	type Props = {
		open: boolean;
		success: boolean;
		reason?: string;
	};

	let { open = $bindable(), success, reason }: Props = $props();
</script>

<Dialog.Root bind:open>
	<Dialog.Portal>
		<Dialog.Overlay
			class="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm data-[state=closed]:animate-[fade-out_150ms] data-[state=open]:animate-[fade-in_150ms]"
		/>
		<Dialog.Content
			class="fixed top-1/2 left-1/2 z-50 w-full max-w-sm -translate-x-1/2 -translate-y-1/2 rounded-2xl border border-white/10 bg-[#0a0d1a] p-6 shadow-2xl outline-none data-[state=closed]:animate-[fade-out_150ms] data-[state=open]:animate-[fade-in_150ms]"
		>
			<div class="mb-4 flex items-start justify-between">
				<Dialog.Title class="text-base font-semibold text-white">
					{success ? 'Spot Secured' : 'Unable to Join'}
				</Dialog.Title>
				<Dialog.Close
					class="rounded-lg p-1 text-white/30 transition hover:bg-white/10 hover:text-white/70"
				>
					<X class="h-4 w-4" />
				</Dialog.Close>
			</div>

			<div class="mb-6 flex flex-col items-center gap-3 py-2 text-center">
				{#if success}
					<CheckCircle class="h-10 w-10 text-emerald-400" />
					<p class="text-sm text-white/60">
						Your waypoint has been locked in. See you on the expedition.
					</p>
				{:else}
					<XCircle class="h-10 w-10 text-red-400" />
					<p class="text-sm text-white/60">
						{reason ?? 'Something went wrong. Please try again.'}
					</p>
				{/if}
			</div>

			<div class="flex justify-end">
				<Dialog.Close
					class="rounded-lg border border-white/10 px-4 py-2 text-sm text-white/50 transition hover:bg-white/5 hover:text-white/70"
				>
					Close
				</Dialog.Close>
			</div>
		</Dialog.Content>
	</Dialog.Portal>
</Dialog.Root>
