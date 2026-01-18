namespace SportMania.Models;

public class Key
{
    public Guid KeyId { get; set; } = Guid.NewGuid();
    public string LicenseKey { get; set; } = string.Empty;
    public ulong GuildId { get; set; }
    public Guid PlanId { get; set; } // Link to Plan
    public ulong? RedeemedByUserId { get; set; }
    public DateTime? RedeemedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DiscordGuild? Guild { get; set; }
    public Plan? Plan { get; set; }
}