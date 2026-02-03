using BlazorApp.Dtos;

namespace BlazorApp.Services;

public interface ITransactionService
{
    Task<(bool IsSuccess, string? RedirectUrl, string? Error)> InitiatePaymentAsync(string email, Guid planId, string phone);
    Task<TransactionDto?> GetTransactionAsync(Guid transactionId);
    Task<List<TransactionDto>> GetAllTransactionsAsync();
}
