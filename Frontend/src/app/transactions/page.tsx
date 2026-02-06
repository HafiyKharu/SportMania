'use client';

import { useState, useEffect } from 'react';
import { transactionService } from '@/services/transactionService';
import { LoadingSpinner } from '@/components/LoadingSpinner';
import { StatusBadge } from '@/components/StatusBadge';
import type { TransactionDto } from '@/types';

export default function TransactionsPage() {
  const [transactions, setTransactions] = useState<TransactionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    loadTransactions();
  }, []);

  async function loadTransactions() {
    try {
      const data = await transactionService.getAllTransactions();
      setTransactions(data);
    } catch {
      setErrorMessage('Failed to load transactions.');
    } finally {
      setLoading(false);
    }
  }

  function formatDate(dateStr: string) {
    const date = new Date(dateStr);
    return date.toISOString().slice(0, 16).replace('T', ' ');
  }

  if (loading) return <LoadingSpinner />;

  return (
    <div className="max-w-7xl mx-auto">
      <h1 className="text-3xl font-bold text-sm-text-light mb-6">Transaction History</h1>

      {errorMessage && (
        <div className="bg-red-500/10 border border-red-500 text-red-400 px-4 py-3 rounded mb-4">
          {errorMessage}
        </div>
      )}

      <div className="bg-sm-card border border-sm-border rounded-lg overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-sm-border">
              <th className="text-left px-4 py-3 text-sm-muted font-medium">Transaction ID</th>
              <th className="text-left px-4 py-3 text-sm-muted font-medium">Customer Email</th>
              <th className="text-left px-4 py-3 text-sm-muted font-medium">Plan</th>
              <th className="text-left px-4 py-3 text-sm-muted font-medium">Amount</th>
              <th className="text-left px-4 py-3 text-sm-muted font-medium">Status</th>
              <th className="text-left px-4 py-3 text-sm-muted font-medium">Bill Code</th>
              <th className="text-left px-4 py-3 text-sm-muted font-medium">Guild ID</th>
              <th className="text-left px-4 py-3 text-sm-muted font-medium">Date</th>
            </tr>
          </thead>
          <tbody>
            {transactions.length === 0 ? (
              <tr>
                <td colSpan={8} className="text-center py-8 text-sm-muted">
                  No transactions found.
                </td>
              </tr>
            ) : (
              transactions.map((tx) => (
                <tr key={tx.transactionId} className="border-b border-sm-border hover:bg-sm-bg/50">
                  <td className="px-4 py-3 text-sm-text font-mono">
                    {tx.transactionId.substring(0, 8)}...
                  </td>
                  <td className="px-4 py-3 text-sm-text">
                    {tx.customer?.email || 'N/A'}
                  </td>
                  <td className="px-4 py-3 text-sm-text">
                    {tx.plan?.name || 'N/A'}
                  </td>
                  <td className="px-4 py-3 text-sm-text">
                    RM{tx.amount}
                  </td>
                  <td className="px-4 py-3">
                    <StatusBadge status={tx.paymentStatus} />
                  </td>
                  <td className="px-4 py-3 text-sm-text font-mono">
                    {tx.billCode || 'N/A'}
                  </td>
                  <td className="px-4 py-3 text-sm-text">
                    {tx.guildId || 'N/A'}
                  </td>
                  <td className="px-4 py-3 text-sm-text">
                    {formatDate(tx.createdAt)}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
