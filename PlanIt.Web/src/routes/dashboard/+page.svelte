<script lang="ts">
	import { goto } from "$app/navigation";
	import { browser } from "$app/environment";
	import { resolve } from "$app/paths";
	import { isAuthenticated, isInitialized, getAuth, isAdmin } from "$lib/stores/auth.svelte";
	import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
	import { PUBLIC_BACKEND_URL } from "$env/static/public";
	import Navbar from "$lib/components/general/Navbar.svelte";
	import { Users, CalendarDays, Waypoints, BookMarked, Radio } from "@lucide/svelte";

	$effect(() => {
		if (browser && isInitialized()) {
			if (!isAuthenticated()) goto(resolve("/auth/login"));
			else if (!isAdmin()) goto(resolve("/"));
		}
	});

	type Stats = {
		userCount: number;
		scheduleCount: number;
		attractionCount: number;
		registrantCount: number;
	};

	let stats = $state<Stats | null>(null);
	let connected = $state(false);

	$effect(() => {
		if (!browser) return;

		const conn = new HubConnectionBuilder()
			.withUrl(`${PUBLIC_BACKEND_URL}/hubs/admin`, {
				accessTokenFactory: () => getAuth().accessToken ?? ""
			})
			.configureLogging(LogLevel.Warning)
			.withAutomaticReconnect()
			.build();

		conn.on("StatsUpdated", (data: Stats) => {
			stats = data;
		});

		conn
			.start()
			.then(() => conn.invoke<Stats>("JoinDashboard"))
			.then((initial) => {
				stats = initial;
				connected = true;
			})
			.catch(console.error);

		return () => {
			conn.invoke("LeaveDashboard").catch(() => {});
			conn.stop();
		};
	});

	const cards = $derived(
		stats
			? [
					{
						label: "Explorers",
						sublabel: "Registered users",
						value: stats.userCount,
						icon: Users,
						color: "blue"
					},
					{
						label: "Expeditions",
						sublabel: "Active schedules",
						value: stats.scheduleCount,
						icon: CalendarDays,
						color: "purple"
					},
					{
						label: "Waypoints",
						sublabel: "Total attractions",
						value: stats.attractionCount,
						icon: Waypoints,
						color: "cyan"
					},
					{
						label: "Enrollments",
						sublabel: "Total registrations",
						value: stats.registrantCount,
						icon: BookMarked,
						color: "emerald"
					}
				]
			: []
	);

	const colorMap: Record<string, { icon: string; glow: string; border: string }> = {
		blue: {
			icon: "text-blue-400 bg-blue-500/10",
			glow: "bg-blue-950/50",
			border: "border-blue-500/10"
		},
		purple: {
			icon: "text-purple-400 bg-purple-500/10",
			glow: "bg-purple-950/50",
			border: "border-purple-500/10"
		},
		cyan: {
			icon: "text-cyan-400 bg-cyan-500/10",
			glow: "bg-cyan-950/40",
			border: "border-cyan-500/10"
		},
		emerald: {
			icon: "text-emerald-400 bg-emerald-500/10",
			glow: "bg-emerald-950/40",
			border: "border-emerald-500/10"
		}
	};
</script>

<svelte:head><title>Mission Control — PlanIt</title></svelte:head>

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
		<!-- Header -->
		<div class="mb-10 flex items-start justify-between">
			<div>
				<p class="mb-1 text-xs font-medium tracking-widest text-blue-400/60 uppercase">Admin</p>
				<h1 class="text-2xl font-bold tracking-wide text-white">Mission Control</h1>
				<p class="mt-1 text-sm text-white/30">Live telemetry across all expeditions</p>
			</div>
			<div class="flex items-center gap-2 rounded-full border border-white/10 px-3 py-1.5">
				<span
					class="h-1.5 w-1.5 rounded-full {connected
						? 'shadow-[0_0_6px_theme(colors-emerald-400)] bg-emerald-400'
						: 'bg-white/20'}"
				></span>
				<span class="text-xs text-white/40">{connected ? "Live" : "Connecting..."}</span>
				<Radio class="h-3 w-3 text-white/20" />
			</div>
		</div>

		<!-- Stat cards -->
		{#if !stats}
			<div class="grid grid-cols-2 gap-4 lg:grid-cols-4">
				{#each [1, 2, 3, 4] as i (i)}
					<div class="animate-pulse rounded-2xl border border-white/5 bg-white/2 p-6">
						<div class="mb-4 h-9 w-9 rounded-xl bg-white/5"></div>
						<div class="mb-2 h-8 w-16 rounded-lg bg-white/5"></div>
						<div class="h-3 w-20 rounded bg-white/5"></div>
					</div>
				{/each}
			</div>
		{:else}
			<div class="grid grid-cols-2 gap-4 lg:grid-cols-4">
				{#each cards as card (card.label)}
					{@const c = colorMap[card.color]}
					<div
						class="group relative overflow-hidden rounded-2xl border {c.border} bg-white/2 p-6 transition-colors hover:bg-white/3"
					>
						<div
							class="absolute -right-6 -bottom-6 h-24 w-24 rounded-full {c.glow} blur-2xl transition-opacity group-hover:opacity-70"
						></div>
						<div class="relative">
							<div class="mb-4 inline-flex rounded-xl p-2.5 {c.icon}">
								<card.icon class="h-4 w-4" />
							</div>
							<p class="mb-0.5 text-3xl font-bold text-white tabular-nums">
								{card.value.toLocaleString()}
							</p>
							<p class="text-sm font-medium text-white/60">{card.label}</p>
							<p class="mt-0.5 text-xs text-white/25">{card.sublabel}</p>
						</div>
					</div>
				{/each}
			</div>
		{/if}
	</main>
</div>
