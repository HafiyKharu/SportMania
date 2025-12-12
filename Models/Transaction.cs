using SportMania.Models.Interface;

namespace SportMania.Models;

public class Transaction : IHasAuditTimestamps
{
    public Guid TransactionId { get; set; }
    public string Amount { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public Key Key { get; set; } = new Key();
    public Customer Customer { get; set; } = new Customer();
    public Plan Plan { get; set; } = new Plan();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
}
public class Key
{
    public Guid KeyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsRedeemed { get; set; }
}