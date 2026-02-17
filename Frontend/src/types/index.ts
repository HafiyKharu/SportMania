export interface PlanDto {
  planId: string;
  imageUrl: string;
  name: string;
  description: string;
  price: string;
  duration: string;
  categoryCode: string;
  details: PlanDetailDto[];
}

export interface PlanDetailDto {
  planDetailsId: string;
  planId: string;
  value: string;
}

export interface TransactionDto {
  transactionId: string;
  customerId: string;
  planId: string;
  keyId: string;
  guildId: number;
  amount: string;
  paymentStatus: string;
  billCode?: string;
  customer?: CustomerDto;
  plan?: PlanDto;
  key?: KeyDto;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
}

export interface CustomerDto {
  customerId: string;
  userNameDiscord: string;
  email: string;
}

export interface KeyDto {
  keyId: string;
  licenseKey: string;
  guildId: number;
  planId: string;
  redeemedByUserId?: number;
  redeemedAt?: string;
  expiresAt?: string;
  durationDays: number;
  isActive: boolean;
  createdAt: string;
}

export interface PaymentResponseDto {
  redirectUrl: string | null;
}

export interface ErrorResponseDto {
  error: string | null;
}

export interface InitiatePaymentRequest {
  email: string;
  planId: string;
  phone: string;
}
