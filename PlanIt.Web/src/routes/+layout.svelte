<script lang="ts">
	import './layout.css';
	import favicon from '$lib/assets/favicon.svg';
	import type { LayoutData } from './$types';
	import { QueryClientProvider } from '@sveltestack/svelte-query';
	import type { Snippet } from 'svelte';
	import { Toaster } from 'svelte-french-toast';
	import { onMount } from 'svelte';
	import { api } from '$lib/services/api';
	import { setAccessToken, setUser, setInitialized, isInitialized } from '$lib/stores/auth.svelte';
	import type { User } from '$lib/types/models/user';
	import type { TokenResponse } from '$lib/types/responses/tokenResponse';

	let { data, children }: { data: LayoutData; children: Snippet } = $props();

	onMount(async () => {
		try {
			const { data: token } = await api.get<TokenResponse>('/auth/refresh');
			setAccessToken(token.accessToken);
			const { data: user } = await api.get<User>('/me');
			setUser(user);
		} catch {
			// No valid refresh token — user stays logged out
		} finally {
			setInitialized();
		}
	});
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>
<QueryClientProvider client={data.queryClient}>
	{#if !isInitialized()}
		<div class="fixed inset-0 flex items-center justify-center bg-[#04060f]">
			<div class="h-8 w-8 animate-spin rounded-full border-2 border-white/10 border-t-blue-400"></div>
		</div>
	{:else}
		{@render children()}
	{/if}
	<Toaster />
</QueryClientProvider>
