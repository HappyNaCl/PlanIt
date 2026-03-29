import { browser } from '$app/environment';
import { QueryClient } from '@sveltestack/svelte-query';

export async function load() {
	const queryClient = new QueryClient({
		defaultOptions: {
			queries: {
				enabled: browser, // don't run queries on the server
				staleTime: 60_000, // data stays fresh for 1 minute
				refetchOnWindowFocus: true
			}
		}
	});

	return { queryClient };
}
