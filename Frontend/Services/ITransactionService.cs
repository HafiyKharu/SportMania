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
    public Guid CustomerId { get; set; }
    public Guid PlanId { get; set; }
    public Guid? KeyId { get; set; }
    public ulong GuildId { get; set; }
    public string Amount { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = "Pending";
    public string? BillCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties (optional for display)
    public CustomerDto? Customer { get; set; }
    public PlanDto? Plan { get; set; }
}

public class CustomerDto
{
    public Guid CustomerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
