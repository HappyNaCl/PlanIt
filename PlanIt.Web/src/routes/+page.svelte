<script lang="ts">
	import { goto } from '$app/navigation';
	import { browser } from '$app/environment';
	import { isAuthenticated, isInitialized, getAuth } from '$lib/stores/auth.svelte';
	import { resolve } from '$app/paths';
	import { Query, useQueryClient } from '@sveltestack/svelte-query';
	import { api } from '$lib/services/api';
	import type { Schedule } from '$lib/types/models/schedule';
	import Navbar from '$lib/components/general/Navbar.svelte';
	import { Telescope, MapPin, ChevronLeft, ChevronRight, CalendarDays, Plus, X } from '@lucide/svelte';
	import ScheduleCard from '$lib/components/schedule/ScheduleCard.svelte';
	import CreateScheduleForm from '$lib/components/schedule/CreateScheduleForm.svelte';
	import toast from 'svelte-french-toast';
	import { DatePicker, Dialog } from 'bits-ui';
	import { CalendarDate, today, getLocalTimeZone } from '@internationalized/date';

	$effect(() => {
		if (browser && isInitialized() && !isAuthenticated()) {
			goto(resolve('/auth/login'));
		}
	});

	const localTz = getLocalTimeZone();
	let calDate = $state<CalendarDate>(today(localTz));

	const utcOffsetMinutes = -new Date().getTimezoneOffset();

	function toDateStr(d: CalendarDate) {
		return `${d.year}-${String(d.month).padStart(2, '0')}-${String(d.day).padStart(2, '0')}`;
	}

	let date = $derived(toDateStr(calDate));

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

	const isAdmin = $derived(getAuth().user?.role === 'ADMIN');

	const queryClient = useQueryClient();

	let createOpen = $state(false);

	function onCreateSuccess() {
		createOpen = false;
		queryClient.invalidateQueries(['schedules', date]);
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
		<div class="mb-8 flex items-center justify-between gap-4">
			<div>
				<h1 class="mb-1 text-2xl font-bold tracking-wide text-white">Available Schedules</h1>
				<p class="text-sm text-white/40">
					Select an expedition and chart your course through the city.
				</p>
			</div>

			<!-- Date picker + admin button -->
			<div class="flex shrink-0 items-center gap-1">
				<button
					onclick={() => (calDate = calDate.subtract({ days: 1 }))}
					class="rounded-lg border border-white/10 bg-white/5 p-2 text-white/40 transition hover:bg-white/10 hover:text-white/80"
				>
					<ChevronLeft class="h-4 w-4" />
				</button>

				<DatePicker.Root bind:value={calDate} granularity="day">
					<DatePicker.Trigger
						class="flex items-center gap-2 rounded-lg border border-white/10 bg-white/5 px-3 py-2 text-sm text-white/80 transition hover:bg-white/10"
					>
						<CalendarDays class="h-4 w-4 shrink-0 text-blue-300/70" />
						<span class="min-w-36 text-left">
							{new Date(calDate.year, calDate.month - 1, calDate.day).toLocaleDateString(
								undefined,
								{ month: 'short', day: 'numeric', year: 'numeric' }
							)}
						</span>
					</DatePicker.Trigger>

					<DatePicker.Portal>
						<DatePicker.Content
							class="z-50 mt-2 rounded-xl border border-white/10 bg-[#0a0d1a] p-4 shadow-2xl outline-none"
						>
							<DatePicker.Calendar>
								{#snippet children({ months, weekdays })}
									<DatePicker.Header class="mb-3 flex items-center justify-between">
										<DatePicker.PrevButton
											class="rounded-lg border border-white/10 bg-white/5 p-1.5 text-white/40 transition hover:bg-white/10 hover:text-white/80"
										>
											<ChevronLeft class="h-4 w-4" />
										</DatePicker.PrevButton>
										<DatePicker.Heading class="text-sm font-semibold text-white/80" />
										<DatePicker.NextButton
											class="rounded-lg border border-white/10 bg-white/5 p-1.5 text-white/40 transition hover:bg-white/10 hover:text-white/80"
										>
											<ChevronRight class="h-4 w-4" />
										</DatePicker.NextButton>
									</DatePicker.Header>

									{#each months as month (month)}
										<DatePicker.Grid class="w-full border-collapse">
											<DatePicker.GridHead>
												<DatePicker.GridRow class="mb-1 flex">
													{#each weekdays as day, i (i)}
														<DatePicker.HeadCell
															class="w-9 text-center text-xs font-medium text-white/25"
														>
															{day.slice(0, 2)}
														</DatePicker.HeadCell>
													{/each}
												</DatePicker.GridRow>
											</DatePicker.GridHead>
											<DatePicker.GridBody>
												{#each month.weeks as weekDates (weekDates)}
													<DatePicker.GridRow class="flex">
														{#each weekDates as d (d)}
															<DatePicker.Cell date={d} month={month.value} class="relative p-0">
																<DatePicker.Day
																	class="flex h-9 w-9 cursor-pointer items-center justify-center rounded-lg text-sm text-white/60 transition hover:bg-white/10 hover:text-white data-disabled:cursor-default data-disabled:opacity-30 data-outside-month:text-white/20 data-selected:bg-blue-600 data-selected:font-semibold data-selected:text-white data-today:font-semibold data-today:text-blue-300 data-selected:data-today:text-white"
																/>
															</DatePicker.Cell>
														{/each}
													</DatePicker.GridRow>
												{/each}
											</DatePicker.GridBody>
										</DatePicker.Grid>
									{/each}
								{/snippet}
							</DatePicker.Calendar>
						</DatePicker.Content>
					</DatePicker.Portal>
				</DatePicker.Root>

				<button
					onclick={() => (calDate = calDate.add({ days: 1 }))}
					class="rounded-lg border border-white/10 bg-white/5 p-2 text-white/40 transition hover:bg-white/10 hover:text-white/80"
				>
					<ChevronRight class="h-4 w-4" />
				</button>

				{#if isAdmin}
					<div class="mx-1 h-6 w-px bg-white/10"></div>
					<button
						onclick={() => (createOpen = true)}
						class="flex items-center gap-1.5 rounded-lg border border-blue-500/30 bg-blue-500/10 px-3 py-2 text-xs font-medium text-blue-300 transition hover:border-blue-400/50 hover:bg-blue-500/20"
					>
						<Plus class="h-3.5 w-3.5" />
						Chart Expedition
					</button>
				{/if}
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

<!-- Create Schedule Modal (admin only) -->
<Dialog.Root bind:open={createOpen}>
	<Dialog.Portal>
		<Dialog.Overlay
			class="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm data-[state=closed]:animate-[fade-out_150ms] data-[state=open]:animate-[fade-in_150ms]"
		/>
		<Dialog.Content
			class="fixed top-1/2 left-1/2 z-50 w-full max-w-md -translate-x-1/2 -translate-y-1/2 rounded-2xl border border-white/10 bg-[#0a0d1a] p-6 shadow-2xl outline-none data-[state=closed]:animate-[fade-out_150ms] data-[state=open]:animate-[fade-in_150ms]"
		>
			<div class="mb-5 flex items-start justify-between">
				<div>
					<Dialog.Title class="text-base font-semibold text-white">Chart Expedition</Dialog.Title>
					<Dialog.Description class="mt-0.5 text-xs text-white/40">
						Departure: {new Date(`${date}T00:00`).toLocaleDateString(undefined, {
							weekday: 'long',
							year: 'numeric',
							month: 'long',
							day: 'numeric'
						})}
					</Dialog.Description>
				</div>
				<Dialog.Close
					class="rounded-lg p-1 text-white/30 transition hover:bg-white/10 hover:text-white/70"
				>
					<X class="h-4 w-4" />
				</Dialog.Close>
			</div>

			<CreateScheduleForm {date} onSuccess={onCreateSuccess} />
		</Dialog.Content>
	</Dialog.Portal>
</Dialog.Root>
