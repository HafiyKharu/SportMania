using Microsoft.EntityFrameworkCore;
using SportMania.Data;
using SportMania.Models;
using SportMania.Repository.Interface;

namespace SportMania.Repository;

public class DiscordGuildRepository : IDiscordGuildRepository
{
    private readonly ApplicationDbContext _context;

    public DiscordGuildRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DiscordGuild?> GetByIdAsync(ulong guildId)
    {
        return await _context.DiscordGuilds.FirstOrDefaultAsync(g => g.GuildId == guildId);
    }

    public async Task<DiscordGuild> CreateAsync(DiscordGuild guild)
    {
        await _context.DiscordGuilds.AddAsync(guild);
        await _context.SaveChangesAsync();
        return guild;
    }

    public async Task UpdateAsync(DiscordGuild guild)
    {
        _context.DiscordGuilds.Update(guild);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ulong guildId)
    {
        var guild = await GetByIdAsync(guildId);
        if (guild != null)
        {
            _context.DiscordGuilds.Remove(guild);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<DiscordGuild> GetOrCreateAsync(ulong guildId)
    {
        var guild = await GetByIdAsync(guildId);
        if (guild == null)
        {
            guild = new DiscordGuild { GuildId = guildId };
            await CreateAsync(guild);
        }
        return guild;
    }
}