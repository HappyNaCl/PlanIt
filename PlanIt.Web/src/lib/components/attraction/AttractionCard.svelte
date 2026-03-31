<script lang="ts">
	import { api } from "$lib/services/api";
	import { UserPlus } from "@lucide/svelte";
	import toast from "svelte-french-toast";
	import type { Attraction } from "$lib/types/models/attraction";

	type Props = {
		attraction: Attraction;
		index: number;
		onJoin: () => void;
	};

	let { attraction, index, onJoin }: Props = $props();

	const full = $derived(attraction.remainingCapacity === 0);
	const pct = $derived(Math.round((attraction.remainingCapacity / attraction.capacity) * 100));

	async function join() {
		try {
			await api.post(`/attractions/${attraction.id}/join`, {});
			toast.success("Spot reserved!");
			onJoin();
		} catch {
			toast.error("Failed to reserve spot");
		}
	}
</script>

<div class="overflow-hidden rounded-xl border border-white/5 bg-white/3">
	{#if attraction.imageUrl}
		<img src={attraction.imageUrl} alt={attraction.name} class="h-40 w-full object-cover" />
	{/if}
	<div class="p-4">
		<div class="flex items-start gap-4">
			<span
				class="mt-0.5 flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-blue-500/10 text-xs font-semibold text-blue-400/70"
			>
				{index + 1}
			</span>
			<div class="flex min-w-0 flex-1 items-start justify-between gap-4">
				<div class="min-w-0">
					<h3 class="mb-1 font-medium text-white/90">{attraction.name}</h3>
					{#if attraction.description}
						<p class="text-sm text-white/40">{attraction.description}</p>
					{/if}
				</div>

				<div class="flex shrink-0 flex-col items-end gap-2">
					<span
						class="rounded-md px-2.5 py-1 text-sm font-bold {full
							? 'bg-red-500/10 text-red-400'
							: pct <= 25
								? 'bg-amber-500/10 text-amber-400'
								: 'bg-emerald-500/10 text-emerald-400'}"
					>
						{attraction.remainingCapacity}/{attraction.capacity} spots
					</span>
					<button
						onclick={join}
						disabled={full}
						class="flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-medium transition {full
							? 'cursor-not-allowed border-white/5 text-white/20'
							: 'border-blue-500/30 bg-blue-500/10 text-blue-300 hover:border-blue-400/50 hover:bg-blue-500/20'}"
					>
						<UserPlus class="h-3.5 w-3.5" />
						{full ? "Fully Booked" : "Join"}
					</button>
				</div>
			</div>
		</div>
	</div>
</div>
