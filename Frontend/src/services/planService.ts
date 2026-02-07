import { apiFetch } from '@/lib/api';
import { getAuthToken } from '@/lib/auth';
import type { PlanDto } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5235';

export const planService = {
  async getAllPlans(): Promise<PlanDto[]> {
    try {
      return await apiFetch<PlanDto[]>('api/plans');
    } catch (error) {
      console.error('Error fetching plans:', error);
      return [];
    }
  },

  async getPlanById(id: string): Promise<PlanDto | null> {
    try {
      return await apiFetch<PlanDto>(`api/plans/${id}`);
    } catch (error) {
      console.error(`Error fetching plan ${id}:`, error);
      return null;
    }
  },

  async createPlan(plan: PlanDto): Promise<void> {
    await apiFetch('api/plans', {
      method: 'POST',
      body: JSON.stringify(plan),
    });
  },

  async updatePlan(id: string, plan: PlanDto): Promise<void> {
    await apiFetch(`api/plans/${id}`, {
      method: 'PUT',
      body: JSON.stringify(plan),
    });
  },

  async deletePlan(id: string): Promise<void> {
    await apiFetch(`api/plans/${id}`, { method: 'DELETE' });
  },

  async getMediaPaths(): Promise<string[]> {
    try {
      return await apiFetch<string[]>('api/plans/media');
    } catch (error) {
      console.error('Error fetching media paths:', error);
      return [];
    }
  },

  async uploadImage(file: File): Promise<string | null> {
    const formData = new FormData();
    formData.append('file', file);
    try {
      const token = getAuthToken();
      const response = await fetch(`${API_BASE_URL}/api/plans/media`, {
        method: 'POST',
        headers: token ? { Authorization: `Bearer ${token}` } : undefined,
        body: formData,
      });
      if (!response.ok) throw new Error('Upload failed');
      const data = await response.json();
      return data.path;
    } catch (error) {
      console.error('Error uploading image:', error);
      return null;
    }
  },
};
