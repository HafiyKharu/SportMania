using SportMania.Models;

namespace SportMania.Services.Interface;

public interface IKeyService
{
    Task<Key> GenerateKeyAsync(ulong guildId, Guid planId, int durationDays);
    Task<IEnumerable<Key>> GenerateKeysAsync(ulong guildId, Guid planId, int amount, int durationDays);
    Task<Key?> GetByIdAsync(Guid id);
    Task<Key?> GetByLicenseKeyAsync(string licenseKey);
    Task<Key?> GetByLicenseKeyAndGuildAsync(string licenseKey, ulong guildId);
    Task<IEnumerable<Key>> GetByGuildIdAsync(ulong guildId);
    Task<IEnumerable<Key>> GetActiveKeysByGuildIdAsync(ulong guildId);
    Task<Key?> GetByUserIdAndGuildAsync(ulong userId, ulong guildId);
    Task<Key?> RedeemKeyAsync(string licenseKey, ulong guildId, ulong userId);
    Task RevokeKeyAsync(Guid keyId);
    Task DeleteKeyAsync(Guid id);
    Task DeleteExpiredKeysAsync();
}