<script lang="ts">
	import { api, type ApiError } from "$lib/services/api";
	import { UserPlus, UserMinus } from "@lucide/svelte";
	import type { Attraction } from "$lib/types/models/attraction";
	import JoinAttractionDialog from "./JoinAttractionDialog.svelte";
	import type { AxiosError } from "axios";

	type Props = {
		attraction: Attraction;
		scheduleId: string;
		index: number;
		onJoin: () => void;
	};

	let { attraction, scheduleId, index, onJoin }: Props = $props();

	const full = $derived(attraction.remainingCapacity === 0);
	const pct = $derived(Math.round((attraction.remainingCapacity / attraction.capacity) * 100));

	let dialogOpen = $state(false);
	let joinSuccess = $state(false);
	let joinReason = $state<string | undefined>(undefined);
	let joining = $state(false);
	let leaving = $state(false);
	let joined = $state(attraction.hasJoined ?? false);
	let idempotencyKey = $state(crypto.randomUUID());

	async function join() {
		joining = true;
		try {
			await api.post(
				`/schedules/${scheduleId}/attractions/${attraction.id}/registrants`,
				{},
				{ headers: { "Idempotency-Key": idempotencyKey } }
			);
			idempotencyKey = crypto.randomUUID();
			joinSuccess = true;
			joinReason = undefined;
			joined = true;
			dialogOpen = true;
			onJoin();
		} catch (err) {
			const axiosErr = err as AxiosError<ApiError>;
			const errors = axiosErr.response?.data?.error;
			const reason = errors ? Object.values(errors).flat()[0] : undefined;
			joinSuccess = false;
			joinReason = reason;
			dialogOpen = true;
		} finally {
			joining = false;
		}
	}

	async function leave() {
		leaving = true;
		try {
			await api.delete(`/schedules/${scheduleId}/attractions/${attraction.id}/registrants`);
			joined = false;
			onJoin();
		} catch (err) {
			const axiosErr = err as AxiosError<ApiError>;
			const errors = axiosErr.response?.data?.error;
			const reason = errors ? Object.values(errors).flat()[0] : undefined;
			joinSuccess = false;
			joinReason = reason;
			dialogOpen = true;
		} finally {
			leaving = false;
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

					{#if joined}
						<button
							onclick={leave}
							disabled={leaving}
							class="flex items-center gap-1.5 rounded-lg border border-red-500/30 bg-red-500/10 px-3 py-1.5 text-xs font-medium text-red-300 transition hover:border-red-400/50 hover:bg-red-500/20 disabled:cursor-not-allowed disabled:opacity-50"
						>
							{#if leaving}
								<div
									class="h-3.5 w-3.5 animate-spin rounded-full border-2 border-red-300/30 border-t-red-300"
								></div>
							{:else}
								<UserMinus class="h-3.5 w-3.5" />
							{/if}
							Cancel
						</button>
					{:else}
						<button
							onclick={join}
							disabled={full || joining}
							class="flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-xs font-medium transition {full
								? 'cursor-not-allowed border-white/5 text-white/20'
								: 'border-blue-500/30 bg-blue-500/10 text-blue-300 hover:border-blue-400/50 hover:bg-blue-500/20 disabled:cursor-not-allowed disabled:opacity-50'}"
						>
							{#if joining}
								<div
									class="h-3.5 w-3.5 animate-spin rounded-full border-2 border-blue-300/30 border-t-blue-300"
								></div>
							{:else}
								<UserPlus class="h-3.5 w-3.5" />
							{/if}
							{full ? "Fully Booked" : "Join"}
						</button>
					{/if}
				</div>
			</div>
		</div>
	</div>
</div>

<JoinAttractionDialog bind:open={dialogOpen} success={joinSuccess} reason={joinReason} />
