<script lang="ts">
	import { onMount } from 'svelte';
	import type { Snippet } from 'svelte';
	import { goto } from '$app/navigation';
	import { resolve } from '$app/paths';
	import { browser } from '$app/environment';
	import { isAuthenticated, isInitialized } from '$lib/stores/auth.svelte';

	let { children }: { children: Snippet } = $props();

	$effect(() => {
		if (browser && isInitialized() && isAuthenticated()) {
			goto(resolve('/'));
		}
	});

	interface Star {
		x: number;
		y: number;
		size: number;
		opacity: number;
		duration: number;
		delay: number;
	}

	let stars = $state<Star[]>([]);

	onMount(() => {
		stars = Array.from({ length: 220 }, () => ({
			x: Math.random() * 100,
			y: Math.random() * 100,
			size: Math.random() * 2 + 0.5,
			opacity: Math.random() * 0.6 + 0.2,
			duration: Math.random() * 4 + 2,
			delay: Math.random() * 6
		}));
	});
</script>

<div
	class="relative flex min-h-screen items-center justify-center overflow-hidden bg-[#04060f] px-4 py-12"
>
	<!-- Nebula blobs -->
	<div
		class="absolute top-0 left-0 h-150 w-150 -translate-x-1/3 -translate-y-1/3 rounded-full bg-purple-950/60 blur-[120px]"
	></div>
	<div
		class="absolute right-0 bottom-0 h-125 w-125 translate-x-1/3 translate-y-1/3 rounded-full bg-blue-950/70 blur-[100px]"
	></div>
	<div
		class="absolute top-1/2 left-1/2 h-75 w-100 -translate-x-1/2 -translate-y-1/2 rounded-full bg-indigo-900/30 blur-[80px]"
	></div>

	<!-- Starfield -->
	{#each stars as star, i (i)}
		<div
			class="pointer-events-none absolute rounded-full bg-white"
			style="left:{star.x}%;top:{star.y}%;width:{star.size}px;height:{star.size}px;opacity:{star.opacity};animation:twinkle {star.duration}s {star.delay}s ease-in-out infinite alternate"
		></div>
	{/each}

	<!-- Page content -->
	<div class="relative z-10 w-full">
		{@render children()}
	</div>
</div>

<style>
	@keyframes twinkle {
		from {
			opacity: 0.2;
			transform: scale(1);
		}
		to {
			opacity: 1;
			transform: scale(1.4);
		}
	}
</style>
