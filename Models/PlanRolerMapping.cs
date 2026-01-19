namespace SportMania.Models;

public class PlanRoleMapping
{
    public Guid MappingId { get; set; } = Guid.NewGuid();
    public ulong GuildId { get; set; }
    public Guid PlanId { get; set; }
    public ulong RoleId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DiscordGuild? Guild { get; set; }
    public Plan? Plan { get; set; }
}