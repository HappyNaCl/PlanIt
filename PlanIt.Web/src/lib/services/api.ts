import { PUBLIC_BACKEND_URL } from '$env/static/public';
import axios from 'axios';
import { getAuth, setAccessToken, setUser, clearAuth } from '$lib/stores/auth.svelte';
import type { User } from '$lib/types/models/user';
import type { TokenResponse } from '$lib/types/responses/tokenResponse';

export const api = axios.create({
	baseURL: PUBLIC_BACKEND_URL,
	withCredentials: true
});

// Attach access token to every request
api.interceptors.request.use((config) => {
	const { accessToken } = getAuth();
	console.log('Hello i get access token, ' + accessToken);
	if (accessToken) {
		config.headers['Authorization'] = `Bearer ${accessToken}`;
	}
	return config;
});

// 401 handler — attempt token refresh then retry
let isRefreshing = false;
let queue: Array<(token: string) => void> = [];

api.interceptors.response.use(
	(response) => {
		response.data = response.data.data;
		return response;
	},
	async (error) => {
		const original = error.config;

		const isAuthEndpoint = original.url?.startsWith('/auth/');
		if (error.response?.status !== 401 || original._retry || isAuthEndpoint) {
			return Promise.reject(error);
		}

		if (isRefreshing) {
			return new Promise((resolve) => {
				queue.push((token) => {
					original.headers['Authorization'] = `Bearer ${token}`;
					resolve(api(original));
				});
			});
		}

		original._retry = true;
		isRefreshing = true;

		try {
			// Refresh cookie sent automatically via withCredentials
			const { data } = await api.get<TokenResponse>('/auth/refresh');
			setAccessToken(data.accessToken);

			// Re-fetch user info with the new token
			const { data: user } = await api.get<User>('/me');
			setUser(user);

			queue.forEach((callback) => callback(data.accessToken));
			original.headers['Authorization'] = `Bearer ${data.accessToken}`;
			return api(original);
		} catch {
			clearAuth();
			// window.location.href = '/auth/login';
			return Promise.reject(error);
		} finally {
			isRefreshing = false;
			queue = [];
		}
	}
);

export interface ApiError {
	error: Record<string, string>;
}
