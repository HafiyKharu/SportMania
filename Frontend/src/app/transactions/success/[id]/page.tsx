'use client';

import { useState, useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { transactionService } from '@/services/transactionService';
import { LoadingSpinner } from '@/components/LoadingSpinner';
import type { TransactionDto } from '@/types';

export default function PaymentSuccessPage() {
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
      <div className="bg-sm-card border-2 border-green-500 rounded-lg p-8 text-center">
        {/* Success Icon */}
        <div className="flex items-center justify-center mb-6">
          <div className="w-20 h-20 rounded-full bg-green-500/20 flex items-center justify-center">
            <svg className="w-10 h-10 text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 13l4 4L19 7" />
            </svg>
          </div>
        </div>

        <h1 className="text-2xl font-bold text-green-400 mb-2">Payment Successful!</h1>
        <p className="text-sm-muted mb-6">Your transaction has been completed successfully.</p>

        {transaction && (
          <div className="bg-sm-bg rounded-lg p-4 mb-6 text-left">
            <h3 className="text-sm font-semibold text-sm-text-light mb-3">Transaction Details</h3>
            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-sm-muted">Plan</span>
                <span className="text-sm-text-light">{transaction.plan?.name || 'N/A'}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-sm-muted">Amount</span>
                <span className="text-sm-text-light">RM{transaction.amount}</span>
              </div>
            </div>

            {transaction.key?.licenseKey && (
              <div className="mt-4 p-3 bg-green-500/10 border border-green-500/30 rounded">
                <p className="text-xs text-sm-muted mb-1">Your License Key</p>
                <p className="text-sm font-mono text-green-400 break-all">
                  {transaction.key.licenseKey}
                </p>
                <p className="text-xs text-sm-muted mt-2">
                  Please save this key. You will need it to activate your subscription.
                </p>
              </div>
            )}
          </div>
        )}

        <button
          onClick={() => router.push('/')}
          className="w-full py-3 px-4 bg-sm-primary text-white rounded-lg hover:bg-blue-600 font-semibold"
        >
          Back to Home
        </button>
      </div>
    </div>
  );
}
