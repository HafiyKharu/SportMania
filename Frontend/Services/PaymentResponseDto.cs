namespace BlazorApp.Services;

using System.Text.Json.Serialization;

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
