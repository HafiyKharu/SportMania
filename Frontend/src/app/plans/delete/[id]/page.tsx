'use client';

import { useState, useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { planService } from '@/services/planService';
import { LoadingSpinner } from '@/components/LoadingSpinner';
import type { PlanDto } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5235';

export default function PlanDeletePage() {
  const router = useRouter();
  const params = useParams();
  const id = params.id as string;

  const [plan, setPlan] = useState<PlanDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [isDeleting, setIsDeleting] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    loadPlan();
  }, [id]);

  async function loadPlan() {
    try {
      const data = await planService.getPlanById(id);
      setPlan(data);
      if (!data) setErrorMessage('Plan not found.');
    } catch {
      setErrorMessage('Failed to load plan.');
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete() {
    setIsDeleting(true);
    setErrorMessage('');

    try {
      await planService.deletePlan(id);
      router.push('/plans');
    } catch {
      setErrorMessage('Failed to delete plan.');
    } finally {
      setIsDeleting(false);
    }
  }

  if (loading) return <LoadingSpinner />;

  if (!plan) {
    return (
      <div className="max-w-3xl mx-auto text-center py-12">
        <p className="text-red-400">{errorMessage || 'Plan not found.'}</p>
        <button
          onClick={() => router.push('/plans')}
          className="mt-4 px-4 py-2 bg-sm-btn-sec border border-sm-btn-sec-border rounded text-sm-text-light hover:bg-sm-hover"
        >
          Back to Plans
        </button>
      </div>
    );
  }

  return (
    <div className="max-w-3xl mx-auto">
      <h1 className="text-3xl font-bold text-sm-text-light mb-6">Delete Plan</h1>

      <div className="bg-yellow-500/10 border border-yellow-500 text-yellow-300 px-4 py-3 rounded mb-6">
        Are you sure you want to delete this plan? This action cannot be undone.
      </div>

      {errorMessage && (
        <div className="bg-red-500/10 border border-red-500 text-red-400 px-4 py-3 rounded mb-4">
          {errorMessage}
        </div>
      )}

      <div className="bg-sm-card border border-sm-border rounded-lg overflow-hidden">
        <div className="flex flex-col md:flex-row">
          {plan.imageUrl && (
            <div className="md:w-1/3">
              <img
                src={`${API_BASE_URL}${plan.imageUrl}`}
                alt={plan.name}
                className="w-full h-48 md:h-full object-cover"
              />
            </div>
          )}

          <div className="flex-1 p-6">
            <h2 className="text-xl font-bold text-sm-text-light mb-2">{plan.name}</h2>
            <p className="text-sm-muted text-sm mb-3">{plan.description}</p>

            <div className="flex items-baseline gap-2 mb-4">
              <span className="text-lg font-bold text-sm-primary">RM{plan.price}</span>
              <span className="text-sm-muted text-sm">/ {plan.duration} days</span>
            </div>

            {plan.details && plan.details.length > 0 && (
              <ul className="space-y-1 mb-4">
                {plan.details.map((detail) => (
                  <li key={detail.planDetailsId} className="flex items-center text-sm text-sm-text">
                    <svg className="w-4 h-4 text-green-400 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                      <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                    </svg>
                    {detail.value}
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      </div>

      <div className="flex gap-3 mt-6">
        <button
          onClick={() => router.push('/plans')}
          disabled={isDeleting}
          className="px-4 py-2 bg-sm-btn-sec border border-sm-btn-sec-border rounded text-sm-text-light hover:bg-sm-hover disabled:opacity-50"
        >
          Cancel
        </button>
        <button
          onClick={handleDelete}
          disabled={isDeleting}
          className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 disabled:opacity-50"
        >
          {isDeleting ? (
            <span className="flex items-center gap-2">
              <span className="inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
              Deleting...
            </span>
          ) : (
            'Delete Plan'
          )}
        </button>
      </div>
    </div>
  );
}
