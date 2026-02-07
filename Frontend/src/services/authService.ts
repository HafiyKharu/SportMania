const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5235';

export interface LoginResponse {
  token: string;
  role: 'Admin' | 'User';
  email: string;
  expiresAt: string;
}

export async function login(username: string, password: string): Promise<LoginResponse> {
  const encodedUser = encodeURIComponent(username);
  const encodedPass = encodeURIComponent(password);

  const response = await fetch(`${API_BASE_URL}/api/login/${encodedUser}+${encodedPass}`, {
    method: 'POST',
  });

  if (!response.ok) {
    const errorBody = await response.text();
    throw new Error(errorBody || 'Login failed.');
  }

  return response.json();
}
