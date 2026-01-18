namespace SportMania.Models.Requests;

public class RequestTransaction
{
    public string Email { get; set; } = string.Empty;
    public Guid PlanId { get; set; }
    public ulong GuildId { get; set; }
    public int DurationDays { get; set; }
}