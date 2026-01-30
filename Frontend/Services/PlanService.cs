namespace BlazorApp.Services;

public class PlanService : IPlanService
{
    private readonly HttpClient _httpClient;

    public PlanService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PlanDto>> GetAllPlansAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/plans");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<PlanDto>>() ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching plans: {ex.Message}");
            return new();
        }
    }

    public async Task<PlanDto?> GetPlanByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/plans/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<PlanDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching plan {id}: {ex.Message}");
            return null;
        }
    }

    public async Task CreatePlanAsync(PlanDto plan)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/plans", plan);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating plan: {ex.Message}");
        }
    }

    public async Task UpdatePlanAsync(Guid id, PlanDto plan)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/plans/{id}", plan);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating plan {id}: {ex.Message}");
        }
    }

    public async Task DeletePlanAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/plans/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting plan {id}: {ex.Message}");
        }
    }

    public async Task<List<string>> GetMediaPathsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/plans/media");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<string>>() ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching media: {ex.Message}");
            return new();
        }
    }
}
