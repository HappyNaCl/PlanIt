<script lang="ts">
	import { resolve } from '$app/paths';
	import Brand from '$lib/components/general/Brand.svelte';
	import AuthFooter from '$lib/components/general/AuthFooter.svelte';
	import toast from 'svelte-french-toast';
	import { api } from '$lib/services/api';
	import type { AuthResponse } from '$lib/types/responses/authResponse';
	import type { User } from '$lib/types/models/user';
	import { setAccessToken, setUser } from '$lib/stores/auth.svelte';

	let username = $state('');
	let password = $state('');

	async function login() {
		const { data } = await api.post<AuthResponse>('/auth/login', {
			username,
			password
		});
		console.log(data);
		setAccessToken(data.accessToken);

		const { data: user } = await api.get<User>('/me');
		setUser(user);
	}

	function handleSubmit(e: Event) {
		e.preventDefault();
		toast.promise(login(), {
			loading: 'Logging in...',
			success: 'Logged in!',
			error: 'Invalid credentials...!'
		});
	}
</script>

<div class="mx-auto w-full max-w-md">
	<Brand />

	<!-- Card -->
	<div
		class="rounded-2xl border border-white/10 bg-white/4 p-8 shadow-2xl shadow-black/60 backdrop-blur-xl"
	>
		<h2 class="mb-1 text-xl font-semibold text-white">Welcome back, Explorer</h2>
		<p class="mb-7 text-sm text-white/40">Ready to continue your journey?</p>

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

			<button
				type="submit"
				class="mt-2 w-full rounded-lg bg-linear-to-r from-blue-600 to-purple-600 px-4 py-2.5 text-sm font-semibold text-white shadow-lg shadow-blue-900/40 transition-all duration-200 hover:from-blue-500 hover:to-purple-500 active:scale-[0.98]"
			>
				Login
			</button>
		</form>

		<div class="mt-6 border-t border-white/5 pt-6 text-center">
			<p class="text-sm text-white/40">
				New to the PlanIt?
				<a
					href={resolve('/auth/register')}
					class="font-medium text-blue-400 transition-colors hover:text-blue-300"
				>
					Create an account
				</a>
			</p>
		</div>
	</div>

	<AuthFooter />
</div>
