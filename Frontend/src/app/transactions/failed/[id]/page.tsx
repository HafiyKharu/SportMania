'use client';

import { useState, useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { transactionService } from '@/services/transactionService';
import { LoadingSpinner } from '@/components/LoadingSpinner';
import type { TransactionDto } from '@/types';

export default function PaymentFailedPage() {
  const router = useRouter();
  const params = useParams();
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
      setTransaction(data);
      if (!data) setErrorMessage('Transaction not found.');
    } catch {
      setErrorMessage('Failed to load transaction.');
    } finally {
      setLoading(false);
    }
  }

  if (loading) return <LoadingSpinner />;

  return (
    <div className="max-w-lg mx-auto py-12">
      <div className="bg-sm-card border-2 border-red-500 rounded-lg p-8 text-center">
        {/* Error Icon */}
        <div className="flex items-center justify-center mb-6">
          <div className="w-20 h-20 rounded-full bg-red-500/20 flex items-center justify-center">
            <svg className="w-10 h-10 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </div>
        </div>

        <h1 className="text-2xl font-bold text-red-400 mb-2">Payment Failed</h1>
        <p className="text-sm-muted mb-6">Unfortunately, your payment could not be processed.</p>

        {transaction && (
          <div className="bg-sm-bg rounded-lg p-4 mb-6 text-left">
            <h3 className="text-sm font-semibold text-sm-text-light mb-3">Transaction Details</h3>
            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-sm-muted">Transaction ID</span>
                <span className="text-sm-text-light font-mono">{transaction.transactionId.substring(0, 8)}...</span>
              </div>
              <div className="flex justify-between">
                <span className="text-sm-muted">Plan</span>
                <span className="text-sm-text-light">{transaction.plan?.name || 'N/A'}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-sm-muted">Amount</span>
                <span className="text-sm-text-light">RM{transaction.amount}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-sm-muted">Status</span>
                <span className="text-red-400">{transaction.paymentStatus}</span>
              </div>
            </div>
          </div>
        )}

        {/* Possible Reasons */}
        <div className="bg-yellow-500/10 border border-yellow-500/30 rounded p-4 mb-6 text-left">
          <h4 className="text-sm font-semibold text-yellow-300 mb-2">Possible Reasons</h4>
          <ul className="text-sm text-sm-muted space-y-1 list-disc list-inside">
            <li>Insufficient funds in your account</li>
            <li>Payment was cancelled by the user</li>
            <li>Your bank declined the transaction</li>
            <li>Network issues during payment processing</li>
          </ul>
        </div>

        <div className="flex gap-3">
          <button
            onClick={() => router.push('/')}
            className="flex-1 py-3 px-4 bg-red-600 text-white rounded-lg hover:bg-red-700 font-semibold"
          >
            Try Again
          </button>
          <button
            onClick={() => router.push('/')}
            className="flex-1 py-3 px-4 bg-sm-btn-sec border border-sm-btn-sec-border text-sm-text-light rounded-lg hover:bg-sm-hover font-semibold"
          >
            Back to Home
          </button>
        </div>

        <p className="text-sm text-sm-muted mt-6">
          Need help? Contact us at{' '}
          <a href="mailto:support@sportmania.com" className="text-sm-primary hover:underline">
            support@sportmania.com
          </a>
        </p>
      </div>
    </div>
  );
}
