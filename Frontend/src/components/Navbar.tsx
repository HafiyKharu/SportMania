'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useState } from 'react';

const navLinks = [
  { href: '/', label: 'Home', exact: true },
  { href: '/plans', label: 'Plans', exact: false },
  { href: '/transactions', label: 'Transactions', exact: false },
];

export function Navbar() {
  const pathname = usePathname();
  const [mobileOpen, setMobileOpen] = useState(false);

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
        {navLinks.map((link) => (
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
      </div>

      {mobileOpen && (
        <div className="w-full sm:hidden border-t border-sm-border mt-2">
          {navLinks.map((link) => (
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
        </div>
      )}
    </nav>
  );
}
