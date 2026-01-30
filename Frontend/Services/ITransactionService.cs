namespace BlazorApp.Services;

using System.Text.Json.Serialization;

public interface ITransactionService
{
    Task<(bool IsSuccess, string? RedirectUrl, string? Error)> InitiatePaymentAsync(string email, Guid planId, string phone);
    Task<TransactionDto?> GetTransactionAsync(Guid transactionId);
    Task<List<TransactionDto>> GetAllTransactionsAsync();
}

public class TransactionDto
{
    [JsonPropertyName("transactionId")]
    public Guid TransactionId { get; set; }
    
    [JsonPropertyName("customerId")]
    public Guid CustomerId { get; set; }
    
    [JsonPropertyName("planId")]
    public Guid PlanId { get; set; }
    
    [JsonPropertyName("keyId")]
    public Guid? KeyId { get; set; }
    
    [JsonPropertyName("guildId")]
    public ulong GuildId { get; set; }
    
    [JsonPropertyName("amount")]
    public string Amount { get; set; } = string.Empty;
    
    [JsonPropertyName("paymentStatus")]
    public string PaymentStatus { get; set; } = "Pending";
    
    [JsonPropertyName("billCode")]
    public string? BillCode { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
    
    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }
    
    // Navigation properties
    [JsonPropertyName("customer")]
    public CustomerDto? Customer { get; set; }
    
    [JsonPropertyName("plan")]
    public PlanDto? Plan { get; set; }
    
    [JsonPropertyName("key")]
    public KeyDto? Key { get; set; }
}

public class CustomerDto
{
    [JsonPropertyName("customerId")]
    public Guid CustomerId { get; set; }
    
    [JsonPropertyName("userNameDiscord")]
    public string UserNameDiscord { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
