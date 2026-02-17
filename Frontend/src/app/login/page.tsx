'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { login } from '@/services/authService';
import { setAuth } from '@/lib/auth';

export default function LoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  async function handleLogin(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const result = await login(email, password);
      setAuth(result.token, result.role, result.email);
      router.replace(result.role === 'Admin' ? '/plans' : '/');
    } catch (err) {
      setError((err as Error).message || 'Login failed.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="max-w-md mx-auto mt-10 bg-sm-card border border-sm-border rounded-lg p-6 animate-scale-in">
      <h1 className="text-2xl font-bold text-sm-text-light mb-2">Sign In</h1>
      <p className="text-sm-muted mb-6">Enter your credentials to continue.</p>

      {error && (
        <div className="bg-red-500/10 border border-red-500 text-red-400 px-3 py-2 rounded mb-4 text-sm">
          {error}
        </div>
      )}

      <form onSubmit={handleLogin}>
        <div className="mb-4">
          <label className="block text-sm text-sm-muted mb-1">Email</label>
          <input
            type="email"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full px-3 py-2 bg-sm-bg border border-sm-border rounded text-sm-text focus:outline-none focus:border-sm-primary focus:ring-1 focus:ring-sm-primary transition-all duration-200"
            placeholder="admin@gmail.com"
          />
        </div>

        <div className="mb-6">
          <label className="block text-sm text-sm-muted mb-1">Password</label>
          <input
            type="password"
            required
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full px-3 py-2 bg-sm-bg border border-sm-border rounded text-sm-text focus:outline-none focus:border-sm-primary focus:ring-1 focus:ring-sm-primary transition-all duration-200"
            placeholder="Your password"
          />
        </div>

        <button
          type="submit"
          disabled={loading}
          className="w-full py-2 px-4 bg-sm-primary text-white rounded hover:bg-blue-600 disabled:opacity-50"
        >
          {loading ? 'Signing in...' : 'Sign In'}
        </button>
      </form>
    </div>
  );
}
