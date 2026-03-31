<script lang="ts">
	import { resolve } from '$app/paths';
	import Brand from '$lib/components/general/Brand.svelte';
	import AuthFooter from '$lib/components/general/AuthFooter.svelte';

	let username = $state('');
	let email = $state('');
	let password = $state('');
	let confirmPassword = $state('');

	let passwordMismatch = $derived(confirmPassword.length > 0 && password !== confirmPassword);

	function handleSubmit(e: Event) {
		e.preventDefault();
		if (password !== confirmPassword) return;
		// TODO: implement registration
	}
</script>

<svelte:head><title>Register — PlanIt</title></svelte:head>

<div class="mx-auto w-full max-w-md">
	<Brand />

	<!-- Card -->
	<div
		class="rounded-2xl border border-white/10 bg-white/4 p-8 shadow-2xl shadow-black/60 backdrop-blur-xl"
	>
		<h2 class="mb-1 text-xl font-semibold text-white">Begin your journey</h2>
		<p class="mb-7 text-sm text-white/40">Claim your place among the stars</p>

		<form onsubmit={handleSubmit} class="space-y-5">
			<div>
				<label
					for="username"
					class="mb-1.5 block text-xs font-medium tracking-wider text-blue-200/70 uppercase"
				>
					Username
				</label>
				<input
					id="username"
					type="text"
					bind:value={username}
					placeholder="Username"
					required
					class="w-full rounded-lg border border-white/10 bg-white/5 px-4 py-2.5 text-sm text-white placeholder-white/20 transition-all duration-200 focus:border-blue-500/50 focus:bg-white/10 focus:ring-1 focus:ring-blue-500/30 focus:outline-none"
				/>
			</div>

			<div>
				<label
					for="email"
					class="mb-1.5 block text-xs font-medium tracking-wider text-blue-200/70 uppercase"
				>
					Email
				</label>
				<input
					id="email"
					type="email"
					bind:value={email}
					placeholder="email@domain.com"
					required
					class="w-full rounded-lg border border-white/10 bg-white/5 px-4 py-2.5 text-sm text-white placeholder-white/20 transition-all duration-200 focus:border-blue-500/50 focus:bg-white/10 focus:ring-1 focus:ring-blue-500/30 focus:outline-none"
				/>
			</div>

			<div>
				<label
					for="password"
					class="mb-1.5 block text-xs font-medium tracking-wider text-blue-200/70 uppercase"
				>
					Password
				</label>
				<input
					id="password"
					type="password"
					bind:value={password}
					placeholder="Password"
					required
					class="w-full rounded-lg border border-white/10 bg-white/5 px-4 py-2.5 text-sm text-white placeholder-white/20 transition-all duration-200 focus:border-blue-500/50 focus:bg-white/10 focus:ring-1 focus:ring-blue-500/30 focus:outline-none"
				/>
			</div>

			<div>
				<label
					for="confirmPassword"
					class="mb-1.5 block text-xs font-medium tracking-wider text-blue-200/70 uppercase"
				>
					Confirm Password
				</label>
				<input
					id="confirmPassword"
					type="password"
					bind:value={confirmPassword}
					placeholder="Confirm Password"
					required
					class="w-full rounded-lg border bg-white/5 px-4 py-2.5 text-sm text-white placeholder-white/20 transition-all duration-200 focus:ring-1 focus:outline-none {passwordMismatch
						? 'border-red-500/50 focus:border-red-500/50 focus:ring-red-500/30'
						: 'border-white/10 focus:border-blue-500/50 focus:bg-white/10 focus:ring-blue-500/30'}"
				/>
				{#if passwordMismatch}
					<p class="mt-1.5 text-xs text-red-400/80">Passwords don't match</p>
				{/if}
			</div>

			<button
				type="submit"
				disabled={passwordMismatch}
				class="mt-2 w-full rounded-lg bg-linear-to-r from-blue-600 to-purple-600 px-4 py-2.5 text-sm font-semibold text-white shadow-lg shadow-blue-900/40 transition-all duration-200 hover:from-blue-500 hover:to-purple-500 active:scale-[0.98] disabled:cursor-not-allowed disabled:opacity-40"
			>
				Enter the Universe
			</button>
		</form>

		<div class="mt-6 border-t border-white/5 pt-6 text-center">
			<p class="text-sm text-white/40">
				Already have an account?
				<a
					href={resolve('/auth/login')}
					class="font-medium text-blue-400 transition-colors hover:text-blue-300"
				>
					Sign in
				</a>
			</p>
		</div>
	</div>

	<AuthFooter />
</div>
