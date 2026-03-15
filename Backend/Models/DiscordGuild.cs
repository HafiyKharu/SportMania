namespace SportMania.Models;

public class DiscordGuild
{
    public ulong GuildId { get; set; }
    public string? Prefix { get; set; }
    public ulong? LogChannelId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public ICollection<Key> Keys { get; set; } = new List<Key>();
    public ICollection<PlanRoleMapping> RoleMappings { get; set; } = new List<PlanRoleMapping>();
}