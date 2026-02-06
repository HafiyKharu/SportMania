import { apiFetch } from '@/lib/api';
import type { KeyDto } from '@/types';

export const keyService = {
  async getKeyById(keyId: string): Promise<KeyDto | null> {
    try {
      return await apiFetch<KeyDto>(`api/keys/${keyId}`);
    } catch (error) {
      console.error(`Error fetching key ${keyId}:`, error);
      return null;
    }
  },

  async getKeyByTransactionId(transactionId: string): Promise<KeyDto | null> {
    try {
      return await apiFetch<KeyDto>(`api/keys/transaction/${transactionId}`);
    } catch (error) {
      console.error(`Error fetching key for transaction ${transactionId}:`, error);
      return null;
    }
  },
};
