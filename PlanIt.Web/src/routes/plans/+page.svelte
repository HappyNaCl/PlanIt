<script lang="ts">
	import { goto } from "$app/navigation";
	import { browser } from "$app/environment";
	import { resolve } from "$app/paths";
	import { isAuthenticated, isInitialized, getAuth } from "$lib/stores/auth.svelte";
	import { Query } from "@sveltestack/svelte-query";
	import { api } from "$lib/services/api";
	import type { MyAttraction } from "$lib/types/models/myAttraction";
	import Navbar from "$lib/components/general/Navbar.svelte";
	import { MapPin, Clock, ChevronRight, BookMarked, History, Rocket } from "@lucide/svelte";
	import toast from "svelte-french-toast";

	$effect(() => {
		if (browser && isInitialized() && !isAuthenticated()) {
			goto(resolve("/auth/login"));
		}
	});

	async function fetchMyAttractions() {
		try {
			const { data } = await api.get<MyAttraction[]>("/me/attractions");
			return data;
		} catch (error) {
			toast.error("Failed to load your plans");
			throw error;
		}
	}

	function isUpcoming(attraction: MyAttraction) {
		return new Date(attraction.scheduleEndTime) > new Date();
	}

	function formatDate(time: string) {
		return new Date(time).toLocaleDateString(undefined, {
			weekday: "short",
			month: "short",
			day: "numeric"
		});
	}

	function formatTime(time: string) {
		return new Date(time).toLocaleTimeString([], { hour: "numeric", minute: "2-digit" });
	}
</script>

<svelte:head><title>My Plans — PlanIt</title></svelte:head>

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
		<div class="mb-8">
			<p class="mb-1 text-xs font-medium tracking-widest text-blue-400/60 uppercase">Explorer</p>
			<h1 class="text-2xl font-bold tracking-wide text-white">My Plans</h1>
			<p class="mt-1 text-sm text-white/30">Waypoints you've enrolled in across all expeditions</p>
		</div>

		<Query
			options={{
				queryKey: ["my-attractions", getAuth().user?.id],
				queryFn: fetchMyAttractions,
				enabled: browser,
				staleTime: 10000
			}}
		>
			<div slot="query" let:queryResult>
				{#if queryResult.isLoading}
					<div class="space-y-3">
						{#each [1, 2, 3] as i (i)}
							<div class="animate-pulse rounded-2xl border border-white/5 bg-white/2 p-4">
								<div class="flex gap-4">
									<div class="h-16 w-16 shrink-0 rounded-xl bg-white/5"></div>
									<div class="flex-1 space-y-2">
										<div class="h-4 w-1/2 rounded bg-white/5"></div>
										<div class="h-3 w-1/3 rounded bg-white/5"></div>
										<div class="h-3 w-1/4 rounded bg-white/5"></div>
									</div>
								</div>
							</div>
						{/each}
					</div>
				{:else if queryResult.isError}
					<div
						class="flex flex-col items-center justify-center rounded-2xl border border-red-500/10 bg-red-500/5 py-16 text-center"
					>
						<p class="text-sm text-red-400/60">Failed to load your plans. Try again later.</p>
					</div>
				{:else if queryResult.data}
					{@const upcoming = queryResult.data.filter(isUpcoming)}
					{@const past = queryResult.data.filter((a) => !isUpcoming(a))}

					{#if queryResult.data.length === 0}
						<div
							class="flex flex-col items-center justify-center rounded-2xl border border-white/5 bg-white/2 py-20 text-center"
						>
							<BookMarked class="mb-3 h-8 w-8 text-white/10" />
							<p class="text-sm text-white/30">You haven't enrolled in any waypoints yet.</p>
							<a
								href={resolve("/")}
								class="mt-4 text-xs text-blue-400/60 transition hover:text-blue-400"
							>
								Browse expeditions →
							</a>
						</div>
					{:else}
						<!-- Upcoming -->
						{#if upcoming.length > 0}
							<section class="mb-8">
								<div class="mb-3 flex items-center gap-2">
									<Rocket class="h-3.5 w-3.5 text-blue-400/50" />
									<h2 class="text-xs font-semibold tracking-widest text-blue-400/60 uppercase">
										Upcoming — {upcoming.length}
									</h2>
								</div>
								<div class="space-y-3">
									{#each upcoming as attraction (attraction.attractionId)}
										{@render AttractionPlanCard(attraction)}
									{/each}
								</div>
							</section>
						{/if}

						<!-- Past -->
						{#if past.length > 0}
							<section>
								<div class="mb-3 flex items-center gap-2">
									<History class="h-3.5 w-3.5 text-white/20" />
									<h2 class="text-xs font-semibold tracking-widest text-white/25 uppercase">
										Past — {past.length}
									</h2>
								</div>
								<div class="space-y-3">
									{#each past as attraction (attraction.attractionId)}
										{@render AttractionPlanCard(attraction, true)}
									{/each}
								</div>
							</section>
						{/if}
					{/if}
				{/if}
			</div>
		</Query>
	</main>
</div>

{#snippet AttractionPlanCard(attraction: MyAttraction, past = false)}
	<div
		class="group flex items-center gap-4 rounded-2xl border {past
			? 'border-white/5 bg-white/[0.015]'
			: 'border-white/8 bg-white/2'} overflow-hidden p-4 transition-colors hover:bg-white/[0.04]"
	>
		<!-- Image -->
		<div class="relative h-16 w-16 shrink-0 overflow-hidden rounded-xl">
			<img
				src={attraction.imageUrl}
				alt={attraction.name}
				class="h-full w-full object-cover {past ? 'opacity-40 grayscale' : ''}"
			/>
		</div>

		<!-- Info -->
		<div class="min-w-0 flex-1">
			<p class="truncate text-sm font-semibold {past ? 'text-white/40' : 'text-white/90'}">
				{attraction.name}
			</p>
			<p class="mt-0.5 truncate text-xs text-white/30">{attraction.scheduleName}</p>
			<div class="mt-1.5 flex flex-wrap items-center gap-x-3 gap-y-1 text-xs text-white/25">
				<span class="flex items-center gap-1">
					<Clock class="h-3 w-3 shrink-0" />
					{formatDate(attraction.scheduleStartTime)} · {formatTime(
						attraction.scheduleStartTime
					)}–{formatTime(attraction.scheduleEndTime)}
				</span>
				<span class="flex items-center gap-1">
					<MapPin class="h-3 w-3 shrink-0" />
					{attraction.scheduleLocation}
				</span>
			</div>
		</div>

		<!-- Link -->
		<a
			href={resolve(`/schedule/${attraction.scheduleId}`)}
			class="shrink-0 rounded-lg border border-white/8 p-2 text-white/20 transition hover:border-white/20 hover:text-white/60"
			aria-label="View expedition"
		>
			<ChevronRight class="h-4 w-4" />
		</a>
	</div>
{/snippet}
