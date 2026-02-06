import type { Metadata } from 'next';
import './globals.css';
import { Navbar } from '@/components/Navbar';

export const metadata: Metadata = {
  title: 'SportMania',
  description: 'Stream Your Favorite Content',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className="bg-sm-bg text-sm-text min-h-screen">
        <Navbar />
        <main className="px-4 py-4">
          {children}
        </main>
      </body>
    </html>
  );
}
