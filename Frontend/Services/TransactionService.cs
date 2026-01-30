namespace BlazorApp.Services;

public class TransactionService : ITransactionService
{
    private readonly HttpClient _httpClient;

    public TransactionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(bool IsSuccess, string? RedirectUrl, string? Error)> InitiatePaymentAsync(string email, Guid planId, string phone)
    {
        try
        {
            var request = new { email, planId, phone };
            var response = await _httpClient.PostAsJsonAsync("api/transactions/initiate-payment", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return (true, result?.redirectUrl?.ToString(), null);
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<dynamic>();
                return (false, null, error?.error?.ToString() ?? "Payment initiation failed.");
            }
        }
        catch (Exception ex)
        {
            return (false, null, $"Error: {ex.Message}");
        }
    }

    public async Task<TransactionDto?> GetTransactionAsync(Guid transactionId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/transactions/{transactionId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<TransactionDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching transaction {transactionId}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<TransactionDto>> GetAllTransactionsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/transactions");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<TransactionDto>>() ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching transactions: {ex.Message}");
            return new();
        }
    }
}
