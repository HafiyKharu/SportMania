namespace SportMania.Models.Requests;

public class RequestTransaction
{
    public Guid PlanId { get; set; }
    public string Email { get; set; } = string.Empty;
}