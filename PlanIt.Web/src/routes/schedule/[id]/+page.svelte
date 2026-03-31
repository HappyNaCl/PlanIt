<script lang="ts">
	import { goto } from "$app/navigation";
	import { browser } from "$app/environment";
	import { page } from "$app/stores";
	import { resolve } from "$app/paths";
	import { isAuthenticated, isInitialized, getAuth } from "$lib/stores/auth.svelte";
	import { Query, useQueryClient } from "@sveltestack/svelte-query";
	import { api } from "$lib/services/api";
	import type { DetailedScheduleResponse } from "$lib/types/responses/detailedScheduleResponse";
	import type { Attraction } from "$lib/types/models/attraction";
	import Navbar from "$lib/components/general/Navbar.svelte";
	import UpdateScheduleDialog from "$lib/components/schedule/UpdateScheduleDialog.svelte";
	import DeleteScheduleDialog from "$lib/components/schedule/DeleteScheduleDialog.svelte";
	import CreateAttractionDialog from "$lib/components/attraction/CreateAttractionDialog.svelte";
	import AttractionCard from "$lib/components/attraction/AttractionCard.svelte";
	import AttractionCardSkeleton from "$lib/components/attraction/AttractionCardSkeleton.svelte";
	import { MapPin, Clock, ChevronLeft, Waypoints, Pencil, Trash2, Plus } from "@lucide/svelte";
	import toast from "svelte-french-toast";

	$effect(() => {
		if (browser && isInitialized() && !isAuthenticated()) {
			goto(resolve("/auth/login"));
		}
	});

	const id = $derived($page.params.id);
	const isAdmin = $derived(getAuth().user?.role === "ADMIN");
	const queryClient = useQueryClient();

	async function fetchSchedule(scheduleId: string) {
		try {
			const { data } = await api.get<DetailedScheduleResponse>(`/schedules/${scheduleId}`);
			scheduleTitle = data.name;
			return data;
		} catch (error) {
			toast.error(`Failed to load expedition: ${error}`);
			throw error;
		}
	}

	async function fetchAttractions(scheduleId: string) {
		try {
			const { data } = await api.get<Attraction[]>(`/schedules/${scheduleId}/attractions`);
			return data;
		} catch (error) {
			toast.error(`Failed to load waypoints: ${error}`);
			throw error;
		}
	}

	function formatTime(time: string) {
		return new Date(time).toLocaleTimeString([], { hour: "numeric", minute: "2-digit" });
	}

	function formatDate(time: string) {
		return new Date(time).toLocaleDateString(undefined, {
			weekday: "long",
			month: "long",
			day: "numeric",
			year: "numeric"
		});
	}

	let scheduleTitle = $state<string | null>(null);

	let updateOpen = $state(false);
	let deleteOpen = $state(false);
	let createAttractionOpen = $state(false);
</script>

<svelte:head><title>{scheduleTitle ? `${scheduleTitle} — PlanIt` : "PlanIt"}</title></svelte:head>

<div class="relative min-h-screen overflow-hidden bg-[#04060f]">
	<!-- Nebula blobs -->
	<div
		class="absolute top-0 left-0 h-150 w-150 -translate-x-1/3 -translate-y-1/3 rounded-full bg-purple-950/40 blur-[120px]"
	></div>
	<div
		class="absolute right-0 bottom-0 h-125 w-125 translate-x-1/3 translate-y-1/3 rounded-full bg-blue-950/50 blur-[100px]"
	></div>

	<Navbar />

	<main class="relative z-10 mx-auto max-w-3xl px-6 py-10">
		<a
			href={resolve("/")}
			class="mb-8 flex items-center gap-1.5 text-sm text-white/40 transition hover:text-white/70"
		>
			<ChevronLeft class="h-4 w-4" />
			Back to expeditions
		</a>

		<Query
			options={{
				queryKey: ["schedule", id],
				queryFn: () => fetchSchedule(id ?? ""),
				enabled: browser,
				staleTime: 20000
			}}
		>
			<div slot="query" let:queryResult>
				{#if queryResult.isLoading}
					<div class="flex items-center justify-center py-20">
						<div
							class="h-6 w-6 animate-spin rounded-full border-2 border-white/10 border-t-blue-400"
						></div>
					</div>
				{:else if queryResult.isError}
					<div
						class="flex flex-col items-center justify-center rounded-2xl border border-red-500/10 bg-red-500/5 py-16 text-center"
					>
						<MapPin class="mb-3 h-8 w-8 text-red-400/30" />
						<p class="text-sm text-red-400/60">Failed to load expedition. Try again later.</p>
					</div>
				{:else if queryResult.data}
					{@const schedule = queryResult.data}

					<!-- Header -->
					<div class="mb-8">
						<p class="mb-1 text-xs font-medium tracking-widest text-blue-400/60 uppercase">
							Expedition
						</p>
						<div class="mb-3 flex items-start justify-between gap-4">
							<h1 class="text-2xl font-bold tracking-wide text-white">{schedule.name}</h1>
							{#if isAdmin}
								<div class="flex shrink-0 items-center gap-2">
									<button
										onclick={() => (updateOpen = true)}
										class="flex items-center gap-1.5 rounded-lg border border-white/10 px-3 py-1.5 text-xs text-white/50 transition hover:bg-white/5 hover:text-white/80"
									>
										<Pencil class="h-3.5 w-3.5" />
										Edit
									</button>
									<button
										onclick={() => (deleteOpen = true)}
										class="flex items-center gap-1.5 rounded-lg border border-red-500/20 px-3 py-1.5 text-xs text-red-400/60 transition hover:border-red-500/40 hover:bg-red-500/10 hover:text-red-400"
									>
										<Trash2 class="h-3.5 w-3.5" />
										Delete
									</button>
								</div>
							{/if}
						</div>
						{#if schedule.description}
							<p class="mb-4 text-sm text-white/40">{schedule.description}</p>
						{/if}
						<div class="flex flex-wrap items-center gap-4 text-xs text-white/30">
							<span class="flex items-center gap-1.5">
								<MapPin class="h-3.5 w-3.5 shrink-0 text-purple-400/50" />
								{schedule.location}
							</span>
							<span class="flex items-center gap-1.5">
								<Clock class="h-3.5 w-3.5 shrink-0 text-blue-400/50" />
								{formatDate(schedule.startTime)} · {formatTime(schedule.startTime)} - {formatTime(
									schedule.endTime
								)}
							</span>
						</div>
					</div>

					<!-- Waypoints -->
					<div>
						<div class="mb-4 flex items-center justify-between gap-2">
							<div class="flex items-center gap-2">
								<Waypoints class="h-4 w-4 text-white/20" />
								<h2 class="text-sm font-semibold tracking-wide text-white/60 uppercase">
									Waypoints
								</h2>
							</div>
							{#if isAdmin}
								<button
									onclick={() => (createAttractionOpen = true)}
									class="flex items-center gap-1.5 rounded-lg border border-blue-500/30 bg-blue-500/10 px-3 py-1.5 text-xs font-medium text-blue-300 transition hover:border-blue-400/50 hover:bg-blue-500/20"
								>
									<Plus class="h-3.5 w-3.5" />
									Add Waypoint
								</button>
							{/if}
						</div>

						<Query
							options={{
								queryKey: ["attractions", id],
								queryFn: () => fetchAttractions(id ?? ""),
								enabled: browser
							}}
						>
							<div slot="query" let:queryResult={attractionsResult}>
								{#if attractionsResult.isLoading}
									<div class="space-y-3">
										{#each [1, 2, 3] as i (i)}
											<AttractionCardSkeleton />
										{/each}
									</div>
								{:else if attractionsResult.isError}
									<div
										class="flex flex-col items-center justify-center rounded-2xl border border-red-500/10 bg-red-500/5 py-10 text-center"
									>
										<p class="text-sm text-red-400/60">Failed to load waypoints.</p>
									</div>
								{:else if !attractionsResult.data?.length}
									<div
										class="flex flex-col items-center justify-center rounded-2xl border border-white/5 bg-white/2 py-16 text-center"
									>
										<Waypoints class="mb-3 h-8 w-8 text-white/10" />
										<p class="text-sm text-white/20">No waypoints on this expedition yet.</p>
									</div>
								{:else}
									<div class="space-y-3">
										{#each attractionsResult.data as attraction, i (attraction.id)}
											<AttractionCard
												{attraction}
												index={i}
												onJoin={() => queryClient.invalidateQueries(["attractions", id])}
											/>
										{/each}
									</div>
								{/if}
							</div>
						</Query>
					</div>

					<CreateAttractionDialog
						scheduleId={id ?? ""}
						bind:open={createAttractionOpen}
						onSuccess={() => queryClient.invalidateQueries(["attractions", id])}
					/>

					<UpdateScheduleDialog
						scheduleId={id ?? ""}
						{schedule}
						bind:open={updateOpen}
						onSuccess={() => queryClient.invalidateQueries(["schedule", id])}
					/>

					<DeleteScheduleDialog
						scheduleId={id ?? ""}
						bind:open={deleteOpen}
						onSuccess={() => goto(resolve("/"))}
					/>
				{/if}
			</div>
		</Query>
	</main>
</div>
