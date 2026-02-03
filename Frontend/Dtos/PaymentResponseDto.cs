using System.Text.Json.Serialization;

namespace BlazorApp.Dtos;

public class PaymentResponseDto
{
    [JsonPropertyName("redirectUrl")]
    public string? RedirectUrl { get; set; }
}

public class ErrorResponseDto
{
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
