'use client';

import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { clearAuth, getAuthToken, getUserEmail, getUserRole } from '@/lib/auth';

const adminLinks = [
  { href: '/plans', label: 'Plans', exact: false },
  { href: '/transactions', label: 'Transactions', exact: false },
];

export function Navbar() {
  const pathname = usePathname();
  const router = useRouter();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [role, setRole] = useState<string | null>(null);
  const [email, setEmail] = useState<string | null>(null);
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    setRole(getUserRole());
    setEmail(getUserEmail());
    setIsLoggedIn(!!getAuthToken());
  }, [pathname]);

  const isActive = (href: string, exact: boolean) =>
    exact ? pathname === '/' : pathname.startsWith(href);

  return (
    <nav className="sticky top-0 z-50 flex flex-wrap items-center min-h-14 bg-sm-bg/95 border-b border-sm-border px-4">
      <Link href="/" className="text-sm-text-light font-bold text-lg mr-8">
        SportMania
      </Link>

      <button
        className="sm:hidden ml-auto p-2 border border-white/10 rounded bg-white/10"
        onClick={() => setMobileOpen(!mobileOpen)}
        aria-label="Toggle navigation"
      >
        <svg className="w-5 h-5 text-white" viewBox="0 0 30 30" fill="none" stroke="currentColor">
          <path strokeLinecap="round" strokeWidth="2" d="M4 7h22M4 15h22M4 23h22" />
        </svg>
      </button>

      <div className="hidden sm:flex items-center gap-0">
        <Link
          href="/"
          className={`flex items-center h-14 px-4 text-sm transition-colors ${
            isActive('/', true)
              ? 'text-sm-primary border-b-[3px] border-sm-primary'
              : 'text-sm-muted hover:text-sm-text-light'
          }`}
        >
          Home
        </Link>

        {role === 'Admin' && adminLinks.map((link) => (
          <Link
            key={link.href}
            href={link.href}
            className={`flex items-center h-14 px-4 text-sm transition-colors ${
              isActive(link.href, link.exact)
                ? 'text-sm-primary border-b-[3px] border-sm-primary'
                : 'text-sm-muted hover:text-sm-text-light'
            }`}
          >
            {link.label}
          </Link>
        ))}

        {isLoggedIn && (
          <div className="flex items-center gap-3 ml-3 text-sm text-sm-muted">
            <span className="hidden md:inline">{email}</span>
            <button
              onClick={() => {
                clearAuth();
                router.replace('/login');
              }}
              className="text-sm-accent hover:underline"
            >
              Logout
            </button>
          </div>
        )}

        {!isLoggedIn && (
          <Link
            href="/login"
            className="flex items-center h-14 px-4 text-sm text-sm-accent hover:underline"
          >
            Login
          </Link>
        )}
      </div>

      {mobileOpen && (
        <div className="w-full sm:hidden border-t border-sm-border mt-2">
          <Link
            href="/"
            onClick={() => setMobileOpen(false)}
            className={`block py-3 px-4 text-sm ${
              isActive('/', true)
                ? 'text-sm-primary border-l-[3px] border-sm-primary'
                : 'text-sm-muted hover:text-sm-text-light'
            }`}
          >
            Home
          </Link>

          {role === 'Admin' && adminLinks.map((link) => (
            <Link
              key={link.href}
              href={link.href}
              onClick={() => setMobileOpen(false)}
              className={`block py-3 px-4 text-sm ${
                isActive(link.href, link.exact)
                  ? 'text-sm-primary border-l-[3px] border-sm-primary'
                  : 'text-sm-muted hover:text-sm-text-light'
              }`}
            >
              {link.label}
            </Link>
          ))}

          {isLoggedIn ? (
            <button
              onClick={() => {
                clearAuth();
                setMobileOpen(false);
                router.replace('/login');
              }}
              className="block w-full text-left py-3 px-4 text-sm text-sm-accent hover:text-sm-text-light"
            >
              Logout
            </button>
          ) : (
            <Link
              href="/login"
              onClick={() => setMobileOpen(false)}
              className="block py-3 px-4 text-sm text-sm-accent hover:text-sm-text-light"
            >
              Login
            </Link>
          )}
        </div>
      )}
    </nav>
  );
}
