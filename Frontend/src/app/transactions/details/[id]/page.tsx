'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { transactionService } from '@/services/transactionService';
import { LoadingSpinner } from '@/components/LoadingSpinner';
import { StatusBadge } from '@/components/StatusBadge';
import type { TransactionDto } from '@/types';

export default function TransactionDetailPage() {
  const params = useParams();
  const router = useRouter();
  const transactionId = params.id as string;

  const [transaction, setTransaction] = useState<TransactionDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    loadTransaction();
  }, [transactionId]);

  async function loadTransaction() {
    try {
      const data = await transactionService.getTransaction(transactionId);
      if (!data) {
        setErrorMessage('Transaction not found.');
      } else {
        setTransaction(data);
      }
    } catch {
      setErrorMessage('Failed to load transaction.');
    } finally {
      setLoading(false);
    }
  }

  function formatDate(dateStr?: string) {
    if (!dateStr) return 'N/A';
    const date = new Date(dateStr);
    return date.toISOString().slice(0, 19).replace('T', ' ');
  }

  function copyToClipboard(text: string) {
    navigator.clipboard.writeText(text);
  }

  if (loading) return <LoadingSpinner />;

  if (errorMessage || !transaction) {
    return (
      <div className="max-w-4xl mx-auto">
        <button onClick={() => router.push('/transactions/details')} className="text-sm-accent hover:underline mb-4 inline-block">
          ← Back to Transactions
        </button>
        <div className="bg-red-500/10 border border-red-500 text-red-400 px-4 py-3 rounded">
          {errorMessage || 'Transaction not found.'}
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      <button onClick={() => router.push('/transactions')} className="text-sm-accent hover:underline mb-6 inline-block">
        ← Back to Transactions
      </button>

      <h1 className="text-3xl font-bold text-sm-text-light mb-6">Transaction Detail</h1>

      {/* Transaction Info */}
      <div className="bg-sm-card border border-sm-border rounded-lg p-6 mb-6">
        <h2 className="text-xl font-semibold text-sm-text-light mb-4">Transaction Info</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <p className="text-sm-muted text-sm">Transaction ID</p>
            <p className="text-sm-text font-mono text-sm break-all">{transaction.transactionId}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Status</p>
            <div className="mt-1">
              <StatusBadge status={transaction.paymentStatus} />
            </div>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Amount</p>
            <p className="text-sm-text text-lg font-semibold">{transaction.amount}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Guild ID</p>
            <p className="text-sm-text">{transaction.guildId || 'N/A'}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Created At</p>
            <p className="text-sm-text">{formatDate(transaction.createdAt)}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Updated At</p>
            <p className="text-sm-text">{formatDate(transaction.updatedAt)}</p>
          </div>
        </div>
      </div>

      {/* Customer Info */}
      <div className="bg-sm-card border border-sm-border rounded-lg p-6 mb-6">
        <h2 className="text-xl font-semibold text-sm-text-light mb-4">Customer</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <p className="text-sm-muted text-sm">Email</p>
            <p className="text-sm-text">{transaction.customer?.email || 'N/A'}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Discord Username</p>
            <p className="text-sm-text">{transaction.customer?.userNameDiscord || 'N/A'}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Customer ID</p>
            <p className="text-sm-text font-mono text-sm break-all">{transaction.customerId}</p>
          </div>
        </div>
      </div>

      {/* Plan Info */}
      <div className="bg-sm-card border border-sm-border rounded-lg p-6 mb-6">
        <h2 className="text-xl font-semibold text-sm-text-light mb-4">Plan</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <p className="text-sm-muted text-sm">Plan Name</p>
            <p className="text-sm-text">{transaction.plan?.name || 'N/A'}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Price</p>
            <p className="text-sm-text">{transaction.plan?.price || 'N/A'}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Duration</p>
            <p className="text-sm-text">{transaction.plan?.duration ? `${transaction.plan.duration} days` : 'N/A'}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Description</p>
            <p className="text-sm-text">{transaction.plan?.description || 'N/A'}</p>
          </div>
        </div>
      </div>

      {/* Key Info */}
      <div className="bg-sm-card border border-sm-border rounded-lg p-6">
        <h2 className="text-xl font-semibold text-sm-text-light mb-4">License Key</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <p className="text-sm-muted text-sm">License Key</p>
            <div className="flex items-center gap-2">
              <p className="text-sm-text font-mono text-lg">{transaction.key?.licenseKey || 'N/A'}</p>
              {transaction.key?.licenseKey && (
                <button
                  onClick={() => copyToClipboard(transaction.key!.licenseKey)}
                  className="text-sm-accent hover:text-sm-accent/80 text-sm"
                  title="Copy to clipboard"
                >
                  📋
                </button>
              )}
            </div>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Active</p>
            <p className={`text-sm font-semibold ${transaction.key?.isActive ? 'text-green-400' : 'text-red-400'}`}>
              {transaction.key?.isActive ? 'Yes' : 'No'}
            </p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Duration</p>
            <p className="text-sm-text">{transaction.key?.durationDays ? `${transaction.key.durationDays} days` : 'N/A'}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Guild ID</p>
            <p className="text-sm-text font-mono">{transaction.key?.guildId || 'N/A'}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Redeemed At</p>
            <p className="text-sm-text">{formatDate(transaction.key?.redeemedAt)}</p>
          </div>
          <div>
            <p className="text-sm-muted text-sm">Expires At</p>
            <p className="text-sm-text">{formatDate(transaction.key?.expiresAt)}</p>
          </div>
        </div>
      </div>
    </div>
  );
}