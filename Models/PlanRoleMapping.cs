namespace SportMania.Models;

public class PlanRoleMapping
{
    public Guid MappingId { get; set; }
    public ulong GuildId { get; set; }
    public Guid PlanId { get; set; }
    public ulong RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public DiscordGuild? Guild { get; set; }
    public Plan? Plan { get; set; }
}