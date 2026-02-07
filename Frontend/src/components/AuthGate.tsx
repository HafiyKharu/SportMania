'use client';

import { usePathname, useRouter } from 'next/navigation';
import { isLoggedIn, isAdmin } from '@/lib/auth';
import { useEffect } from 'react';

const PUBLIC_ROUTES = ['/', '/login'];

export function AuthGate({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();

  useEffect(() => {
    if (PUBLIC_ROUTES.includes(pathname)) return;

    if (!isLoggedIn()) {
      router.replace('/login');
      return;
    }

    if (!isAdmin()) {
      router.replace('/');
    }
  }, [pathname, router]);

  return <>{children}</>;
}
