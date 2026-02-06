namespace SportMania.Models.Requests;

public class RequestTransaction
{
    public string Email { get; set; } = string.Empty;
    public Guid PlanId { get; set; }
}