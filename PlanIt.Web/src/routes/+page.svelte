<script lang="ts">
	import { goto } from '$app/navigation';
	import { browser } from '$app/environment';
	import { isAuthenticated, isInitialized } from '$lib/stores/auth.svelte';
	import { resolve } from '$app/paths';
	import { Query } from '@sveltestack/svelte-query';
	import { api } from '$lib/services/api';
	import type { Schedule } from '$lib/types/models/schedule';
	import Navbar from '$lib/components/Navbar.svelte';
	import { Telescope, CalendarDays, MapPin, ChevronLeft, ChevronRight } from '@lucide/svelte';
	import ScheduleCard from '$lib/components/ScheduleCard.svelte';
	import toast from 'svelte-french-toast';

	$effect(() => {
		if (browser && isInitialized() && !isAuthenticated()) {
			goto(resolve('/auth/login'));
		}
	});

	let date = $state(new Date().toISOString().split('T')[0]);

	const utcOffsetMinutes = -new Date().getTimezoneOffset();

	async function fetchSchedules(dateStr: string) {
		try {
			const { data } = await api.get<Schedule[]>(
				`/schedules?date=${dateStr}&utcOffsetMinutes=${utcOffsetMinutes}`
			);
			return data;
		} catch (error) {
			toast.error(`Error fetching schedule: ${error}`);
			throw error;
		}
	}
</script>

<div class="relative min-h-screen overflow-hidden bg-[#04060f]">
	<!-- Nebula blobs -->
	<div
		class="absolute top-0 left-0 h-150 w-150 -translate-x-1/3 -translate-y-1/3 rounded-full bg-purple-950/40 blur-[120px]"
	></div>
	<div
		class="absolute right-0 bottom-0 h-125 w-125 translate-x-1/3 translate-y-1/3 rounded-full bg-blue-950/50 blur-[100px]"
	></div>

	<Navbar />

	<main class="relative z-10 mx-auto max-w-4xl px-6 py-10">
		<!-- Header + date picker -->
		<div class="mb-8 flex items-start justify-between">
			<div>
				<h1 class="mb-1 text-2xl font-bold tracking-wide text-white">Available Schedules</h1>
				<p class="text-sm text-white/40">
					Select an expedition and chart your course through the city.
				</p>
			</div>
			<div class="flex items-center gap-1">
				<button
					onclick={() => {
						const [y, m, d] = date.split('-').map(Number);
						const dt = new Date(y, m - 1, d - 1);
						date = `${dt.getFullYear()}-${String(dt.getMonth() + 1).padStart(2, '0')}-${String(dt.getDate()).padStart(2, '0')}`;
					}}
					class="rounded-lg border border-white/10 bg-white/5 p-2 text-white/40 transition hover:bg-white/10 hover:text-white/80"
				>
					<ChevronLeft class="h-4 w-4" />
				</button>
				<div class="flex items-center gap-2 rounded-lg border border-white/10 bg-white/5 px-3 py-2">
					<CalendarDays class="h-4 w-4 shrink-0 text-blue-300/70" />
					<input
						type="date"
						value={date}
						onchange={(e) => (date = e.currentTarget.value)}
						class="bg-transparent text-sm text-white/80 scheme-dark outline-none"
					/>
				</div>
				<button
					onclick={() => {
						const [y, m, d] = date.split('-').map(Number);
						const dt = new Date(y, m - 1, d + 1);
						date = `${dt.getFullYear()}-${String(dt.getMonth() + 1).padStart(2, '0')}-${String(dt.getDate()).padStart(2, '0')}`;
					}}
					class="rounded-lg border border-white/10 bg-white/5 p-2 text-white/40 transition hover:bg-white/10 hover:text-white/80"
				>
					<ChevronRight class="h-4 w-4" />
				</button>
			</div>
		</div>

		{#key date}
		<Query
			options={{
				queryKey: ['schedules', date],
				queryFn: () => fetchSchedules(date)
			}}
		>
			<div slot="query" let:queryResult>
				<!-- Loading -->
				{#if queryResult.isLoading}
					<div class="flex items-center justify-center py-20">
						<div
							class="h-6 w-6 animate-spin rounded-full border-2 border-white/10 border-t-blue-400"
						></div>
					</div>

					<!-- Error -->
				{:else if queryResult.isError}
					<div
						class="flex flex-col items-center justify-center rounded-2xl border border-red-500/10 bg-red-500/5 py-16 text-center"
					>
						<MapPin class="mb-3 h-8 w-8 text-red-400/30" />
						<p class="text-sm text-red-400/60">Failed to load schedules. Try again later.</p>
					</div>

					<!-- Empty -->
				{:else if !queryResult.data?.length}
					<div
						class="flex flex-col items-center justify-center rounded-2xl border border-white/5 bg-white/2 py-20 text-center"
					>
						<Telescope class="mb-4 h-10 w-10 text-white/10" />
						<p class="text-sm font-medium text-white/20">No expeditions charted yet</p>
						<p class="mt-1 text-xs text-white/10">
							Check back once your navigator has loaded the routes.
						</p>
					</div>

					<!-- Results -->
				{:else}
					<div class="space-y-3">
						{#each queryResult.data as schedule (schedule.id)}
							<ScheduleCard {schedule} />
						{/each}
					</div>
				{/if}
			</div>
		</Query>
		{/key}
	</main>
</div>
