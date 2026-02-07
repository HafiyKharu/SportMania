export type UserRole = 'Admin' | 'User';

const TOKEN_KEY = 'sm_token';
const ROLE_KEY = 'sm_role';
const EMAIL_KEY = 'sm_email';

const isBrowser = () => typeof window !== 'undefined';

export function setAuth(token: string, role: UserRole, email: string) {
  if (!isBrowser()) return;
  sessionStorage.setItem(TOKEN_KEY, token);
  sessionStorage.setItem(ROLE_KEY, role);
  sessionStorage.setItem(EMAIL_KEY, email);
}

export function clearAuth() {
  if (!isBrowser()) return;
  sessionStorage.removeItem(TOKEN_KEY);
  sessionStorage.removeItem(ROLE_KEY);
  sessionStorage.removeItem(EMAIL_KEY);
}

export function getAuthToken(): string | null {
  return isBrowser() ? sessionStorage.getItem(TOKEN_KEY) : null;
}

export function getUserRole(): UserRole | null {
  const role = isBrowser() ? sessionStorage.getItem(ROLE_KEY) : null;
  return role === 'Admin' || role === 'User' ? role : null;
}

export function getUserEmail(): string | null {
  return isBrowser() ? sessionStorage.getItem(EMAIL_KEY) : null;
}

export function isLoggedIn(): boolean {
  return !!getAuthToken();
}

export function isAdmin(): boolean {
  return getUserRole() === 'Admin';
}
