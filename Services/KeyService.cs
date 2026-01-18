using SportMania.Models;
using SportMania.Repository.Interface;
using SportMania.Services.Interface;
using System.Security.Cryptography;

namespace SportMania.Services;

public class KeyService : IKeyService
{
    private readonly IKeyRepository _keyRepository;
    private readonly IDiscordGuildRepository _guildRepository;

    public KeyService(IKeyRepository keyRepository, IDiscordGuildRepository guildRepository)
    {
        _keyRepository = keyRepository;
        _guildRepository = guildRepository;
    }
    
    public async Task<Key> GenerateKeyAsync(ulong guildId, Guid planId, int durationDays)
    {
        var guild = await _guildRepository.GetByIdAsync(guildId);
        if (guild == null)
        {
            guild = new DiscordGuild { GuildId = guildId };
            await _guildRepository.CreateAsync(guild);
        }

        var newKey = new Key
        {
            KeyId = Guid.NewGuid(),
            LicenseKey = GenerateLicenseKey(),
            GuildId = guildId,
            PlanId = planId,
            DurationDays = durationDays,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        return await _keyRepository.CreateAsync(newKey);
    }

    public async Task<IEnumerable<Key>> GenerateKeysAsync(ulong guildId, Guid planId, int amount, int durationDays)
    {
        var keys = new List<Key>();
        for (int i = 0; i < amount; i++)
        {
            var key = await GenerateKeyAsync(guildId, planId, durationDays);
            keys.Add(key);
        }
        return keys;
    }

    public async Task<Key?> GetByIdAsync(Guid id)
    {
        return await _keyRepository.GetByIdAsync(id);
    }

    public async Task<Key?> GetByLicenseKeyAsync(string licenseKey)
    {
        return await _keyRepository.GetByLicenseKeyAsync(licenseKey);
    }

    public async Task<Key?> GetByLicenseKeyAndGuildAsync(string licenseKey, ulong guildId)
    {
        return await _keyRepository.GetByLicenseKeyAndGuildAsync(licenseKey, guildId);
    }

    public async Task<IEnumerable<Key>> GetByGuildIdAsync(ulong guildId)
    {
        return await _keyRepository.GetByGuildIdAsync(guildId);
    }

    public async Task<IEnumerable<Key>> GetActiveKeysByGuildIdAsync(ulong guildId)
    {
        return await _keyRepository.GetActiveKeysByGuildIdAsync(guildId);
    }

    public async Task<Key?> GetByUserIdAndGuildAsync(ulong userId, ulong guildId)
    {
        return await _keyRepository.GetByUserIdAndGuildAsync(userId, guildId);
    }

    public async Task<Key?> RedeemKeyAsync(string licenseKey, ulong guildId, ulong userId)
    {
        var key = await _keyRepository.GetByLicenseKeyAndGuildAsync(licenseKey, guildId);
        
        if (key == null || !key.IsActive || key.RedeemedByUserId != null)
        {
            return null;
        }

        key.RedeemedByUserId = userId;
        key.RedeemedAt = DateTime.UtcNow;
        key.ExpiresAt = DateTime.UtcNow.AddDays(key.DurationDays);
        
        await _keyRepository.UpdateAsync(key);
        return key;
    }

    public async Task RevokeKeyAsync(Guid keyId)
    {
        var key = await _keyRepository.GetByIdAsync(keyId);
        if (key != null)
        {
            key.IsActive = false;
            await _keyRepository.UpdateAsync(key);
        }
    }

    public async Task DeleteKeyAsync(Guid id)
    {
        await _keyRepository.DeleteAsync(id);
    }

    public async Task DeleteExpiredKeysAsync()
    {
        await _keyRepository.DeleteExpiredKeysAsync();
    }

    private static string GenerateLicenseKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var segments = new string[4];
        
        for (int i = 0; i < 4; i++)
        {
            var segment = new char[5];
            for (int j = 0; j < 5; j++)
            {
                segment[j] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }
            segments[i] = new string(segment);
        }
        
        return string.Join("-", segments);
    }
}