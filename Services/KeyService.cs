using SportMania.Models;
using SportMania.Repository.Interface;
using SportMania.Services.Interface;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace SportMania.Services;

public class KeyService : IKeyService
{
    private readonly IKeyRepository _keyRepository;
    private readonly IDiscordGuildRepository _guildRepository;
    private readonly ILogger<KeyService> _logger;

    public KeyService(IKeyRepository keyRepository, IDiscordGuildRepository guildRepository, ILogger<KeyService> logger)
    {
        _keyRepository = keyRepository;
        _guildRepository = guildRepository;
        _logger = logger;
    }
    
    public async Task<Key> GenerateKeyAsync(ulong guildId, Guid planId, int durationDays)
    {
        var guild = await _guildRepository.GetByIdAsync(guildId);
        if (guild == null)
        {
            guild = new DiscordGuild { GuildId = guildId };
            await _guildRepository.CreateAsync(guild);
        }

        string licenseKey;
        int maxRetries = 10;
        int retryCount = 0;
        do
        {
            if (retryCount >= maxRetries)
            {
                _logger.LogError("Failed to generate a unique license key after {MaxRetries} attempts.", maxRetries);
                throw new InvalidOperationException("Could not generate a unique license key.");
            }
            licenseKey = GenerateLicenseKey();
            retryCount++;
        } while (await _keyRepository.GetByLicenseKeyAsync(licenseKey) != null);

        var newKey = new Key
        {
            KeyId = Guid.NewGuid(),
            LicenseKey = licenseKey,
            GuildId = guildId,
            PlanId = planId,
            DurationDays = durationDays,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        return await _keyRepository.CreateAsync(newKey);
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