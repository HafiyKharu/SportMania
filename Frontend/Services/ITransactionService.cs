namespace BlazorApp.Services;

public interface ITransactionService
{
    Task<(bool IsSuccess, string? RedirectUrl, string? Error)> InitiatePaymentAsync(string email, Guid planId, string phone);
    Task<TransactionDto?> GetTransactionAsync(Guid transactionId);
    Task<List<TransactionDto>> GetAllTransactionsAsync();
}

public class TransactionDto
{
    public Guid TransactionId { get; set; }
    public string Email { get; set; } = string.Empty;
    public Guid PlanId { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
