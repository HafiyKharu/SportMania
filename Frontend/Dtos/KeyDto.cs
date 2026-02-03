using System.Text.Json.Serialization;

namespace BlazorApp.Dtos;

public class KeyDto
{
    [JsonPropertyName("keyId")]
    public Guid KeyId { get; set; }

    [JsonPropertyName("licenseKey")]
    public string LicenseKey { get; set; } = string.Empty;

    [JsonPropertyName("guildId")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("planId")]
    public Guid PlanId { get; set; }

    [JsonPropertyName("redeemedByUserId")]
    public ulong? RedeemedByUserId { get; set; }

    [JsonPropertyName("redeemedAt")]
    public DateTime? RedeemedAt { get; set; }

    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; set; }

    [JsonPropertyName("durationDays")]
    public int DurationDays { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}
