<script lang="ts">
	import { goto } from '$app/navigation';
	import { resolve } from '$app/paths';
	import { page } from '$app/state';
	import { LogOut, User, CalendarDays, BookMarked } from '@lucide/svelte';
	import { getAuth, clearAuth } from '$lib/stores/auth.svelte';
	import { api } from '$lib/services/api';

	const auth = getAuth();

	async function logout() {
		try {
			await api.post('/auth/logout');
		} finally {
			clearAuth();
			goto(resolve('/auth/login'));
		}
	}

	const navLinks = [
		{ label: 'Schedules', href: resolve('/'), icon: CalendarDays },
		{ label: 'My Plans', href: resolve('/plans'), icon: BookMarked }
	];
</script>

<nav class="relative z-10 border-b border-white/5">
	<!-- Top row: brand + user -->
	<div class="mx-auto flex max-w-4xl items-center justify-between px-6 py-4">
		<div class="flex items-center gap-2">
			<svg
				class="h-5 w-5 text-blue-300"
				viewBox="0 0 24 24"
				fill="none"
				stroke="currentColor"
				stroke-width="1.5"
			>
				<circle cx="12" cy="12" r="4" />
				<ellipse cx="12" cy="12" rx="10" ry="4" transform="rotate(-30 12 12)" stroke-dasharray="3 2" />
				<path d="M12 2v2M12 20v2M2 12h2M20 12h2" stroke-linecap="round" />
			</svg>
			<span class="text-sm font-bold tracking-widest text-white uppercase">PlanIt</span>
		</div>
		<div class="flex items-center gap-4">
			<div class="flex items-center gap-1.5 text-sm text-white/40">
				<User class="h-3.5 w-3.5" />
				{auth.user?.username}
			</div>
			<button
				onclick={logout}
				class="flex items-center gap-1.5 rounded-lg border border-white/10 px-3 py-1.5 text-xs text-white/50 transition-colors hover:border-white/20 hover:text-white/80"
			>
				<LogOut class="h-3.5 w-3.5" />
				Abort Mission
			</button>
		</div>
	</div>

	<!-- Nav links row -->
	<div class="mx-auto max-w-4xl px-6">
		<div class="flex border-t border-white/5">
			{#each navLinks as link}
				{@const active = page.url.pathname === link.href}
				<a
					href={link.href}
					class="flex items-center gap-1.5 border-b-2 px-3 py-2.5 text-xs font-medium transition-colors {active
						? 'border-blue-400 text-blue-400'
						: 'border-transparent text-white/40 hover:text-white/70'}"
				>
					<link.icon class="h-3.5 w-3.5" />
					{link.label}
				</a>
			{/each}
		</div>
	</div>
</nav>
