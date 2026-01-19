using SportMania.Models;

namespace SportMania.Repository.Interface;

public interface IDiscordGuildRepository
{
    Task<DiscordGuild?> GetByIdAsync(ulong guildId);
    Task<DiscordGuild> CreateAsync(DiscordGuild guild);
    Task UpdateAsync(DiscordGuild guild);
    Task DeleteAsync(ulong guildId);
    Task<DiscordGuild> GetOrCreateAsync(ulong guildId);
}