'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { planService } from '@/services/planService';
import { LoadingSpinner } from '@/components/LoadingSpinner';
import type { PlanDto } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5235';

export default function PlansPage() {
  const router = useRouter();
  const [plans, setPlans] = useState<PlanDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');

  useEffect(() => {
    loadPlans();
  }, []);

  async function loadPlans() {
    try {
      const data = await planService.getAllPlans();
      setPlans(data);
    } catch {
      setErrorMessage('Failed to load plans.');
    } finally {
      setLoading(false);
    }
  }

  if (loading) return <LoadingSpinner />;

  return (
    <div className="max-w-6xl mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-sm-text-light">Available Plans</h1>
        <button
          onClick={() => router.push('/plans/create')}
          className="flex items-center gap-2 bg-sm-primary text-white px-4 py-2 rounded hover:bg-blue-600 transition-colors"
        >
          <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clipRule="evenodd" />
          </svg>
          Create New Plan
        </button>
      </div>

      {errorMessage && (
        <div className="bg-red-500/10 border border-red-500 text-red-400 px-4 py-3 rounded mb-4">
          {errorMessage}
        </div>
      )}

      {successMessage && (
        <div className="bg-green-500/10 border border-green-500 text-green-400 px-4 py-3 rounded mb-4">
          {successMessage}
        </div>
      )}

      {plans.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-sm-muted text-lg">No plans available. Create your first plan!</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {plans.map((plan) => (
            <div
              key={plan.planId}
              className="bg-sm-card border border-sm-border rounded-lg overflow-hidden flex flex-col sm:flex-row"
            >
              {plan.imageUrl && (
                <div
                  className="sm:w-48 h-48 sm:h-auto flex-shrink-0 cursor-pointer"
                  onClick={() => router.push(`/plans/details/${plan.planId}`)}
                >
                  <img
                    src={`${API_BASE_URL}${plan.imageUrl}`}
                    alt={plan.name}
                    className="w-full h-full object-cover"
                  />
                </div>
              )}

              <div className="flex flex-col flex-1 p-4">
                <h3 className="text-lg font-bold text-sm-text-light mb-1">{plan.name}</h3>
                <p className="text-sm-muted text-sm mb-3">{plan.description}</p>

                <div className="flex items-baseline gap-2 mb-4">
                  <span className="text-xl font-bold text-sm-primary">RM{plan.price}</span>
                  <span className="text-sm-muted text-sm">/ {plan.duration} days</span>
                </div>

                <div className="flex gap-2 mt-auto">
                  <button
                    onClick={() => router.push(`/plans/details/${plan.planId}`)}
                    className="px-3 py-1.5 text-sm bg-sm-primary text-white rounded hover:bg-blue-600"
                  >
                    Details
                  </button>
                  <button
                    onClick={() => router.push(`/plans/edit/${plan.planId}`)}
                    className="px-3 py-1.5 text-sm bg-yellow-600 text-white rounded hover:bg-yellow-700"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => router.push(`/plans/delete/${plan.planId}`)}
                    className="px-3 py-1.5 text-sm bg-red-600 text-white rounded hover:bg-red-700"
                  >
                    Delete
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
