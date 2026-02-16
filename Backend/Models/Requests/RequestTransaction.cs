namespace SportMania.Models.Requests;

public class RequestTransaction
{
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber {get; set;}
    public Guid PlanId { get; set; }
}