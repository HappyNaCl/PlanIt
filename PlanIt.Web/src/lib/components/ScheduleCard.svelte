<script lang="ts">
	import { MapPin, Clock, Waypoints } from '@lucide/svelte';
	import type { Schedule } from '$lib/types/models/schedule';

	let { schedule }: { schedule: Schedule } = $props();

	function formatTime(time: string) {
		return new Date(time).toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' });
	}
</script>

<div
	class="group cursor-pointer rounded-xl border border-white/5 bg-white/3 p-5 transition-all duration-200 hover:border-blue-500/20 hover:bg-white/5"
>
	<div class="mb-3 flex items-start justify-between gap-4">
		<div>
			<p class="mb-0.5 text-xs font-medium tracking-widest text-blue-400/60 uppercase">Expedition</p>
			<h3 class="font-semibold text-white">{schedule.name}</h3>
		</div>
		<div class="flex items-center gap-1.5 rounded-md border border-white/5 bg-white/5 px-2.5 py-1 text-xs text-white/40">
			<Waypoints class="h-3.5 w-3.5 shrink-0" />
			<span>{schedule.attractionCount} waypoint{schedule.attractionCount !== 1 ? 's' : ''}</span>
		</div>
	</div>

	{#if schedule.description}
		<p class="mb-3 text-sm text-white/40">{schedule.description}</p>
	{/if}

	<div class="flex flex-wrap items-center gap-3 text-xs text-white/30">
		<span class="flex items-center gap-1.5">
			<MapPin class="h-3.5 w-3.5 shrink-0 text-purple-400/50" />
			{schedule.location}
		</span>
		<span class="flex items-center gap-1.5">
			<Clock class="h-3.5 w-3.5 shrink-0 text-blue-400/50" />
			{formatTime(schedule.startTime)} – {formatTime(schedule.endTime)}
		</span>
	</div>
</div>
