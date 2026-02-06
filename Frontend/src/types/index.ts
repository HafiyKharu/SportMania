export interface PlanDto {
  planId: string;
  imageUrl: string;
  name: string;
  description: string;
  price: string;
  duration: string;
  details: PlanDetailDto[];
  createdAt: string | null;
  isDeleted: boolean;
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
  keyId: string | null;
  guildId: number;
  amount: string;
  paymentStatus: string;
  billCode: string | null;
  createdAt: string;
  updatedAt: string | null;
  isDeleted: boolean;
  customer: CustomerDto | null;
  plan: PlanDto | null;
  key: KeyDto | null;
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
  redeemedByUserId: number | null;
  redeemedAt: string | null;
  expiresAt: string | null;
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
