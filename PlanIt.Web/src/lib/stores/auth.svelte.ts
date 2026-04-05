import type { User } from "$lib/types/models/user";

interface AuthState {
	accessToken: string | null;
	user: User | null;
	initialized: boolean;
}

const auth = $state<AuthState>({
	accessToken: null,
	user: null,
	initialized: false
});

export function getAuth() {
	return auth;
}

export function setAccessToken(accessToken: string) {
	auth.accessToken = accessToken;
}

export function setUser(user: User) {
	auth.user = user;
}

export function clearAuth() {
	auth.accessToken = null;
	auth.user = null;
}

export function setInitialized() {
	auth.initialized = true;
}

export function isInitialized() {
	return auth.initialized;
}

export function isAuthenticated() {
	return auth.accessToken !== null;
}

export function isAdmin() {
	return auth.user?.role === "ADMIN";
}
