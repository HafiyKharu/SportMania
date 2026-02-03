using BlazorApp.Dtos;

namespace BlazorApp.Services;

public class KeyService : IKeyService
{
    private readonly HttpClient _httpClient;

    public KeyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<KeyDto?> GetKeyByIdAsync(Guid keyId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/keys/{keyId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<KeyDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching key {keyId}: {ex.Message}");
            return null;
        }
    }

    public async Task<KeyDto?> GetKeyByTransactionIdAsync(Guid transactionId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/keys/transaction/{transactionId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<KeyDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching key for transaction {transactionId}: {ex.Message}");
            return null;
        }
    }
}
