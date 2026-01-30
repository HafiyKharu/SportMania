namespace BlazorApp.Services;

public class PlanService : IPlanService
{
    private readonly HttpClient _httpClient;
    private readonly IWebHostEnvironment? _environment;

    public PlanService(HttpClient httpClient, IWebHostEnvironment? environment = null)
    {
        _httpClient = httpClient;
        _environment = environment;
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
            // Don't send CreatedAt or IsDeleted - Backend will handle these
            var payload = new 
            {
                planId = plan.PlanId,
                imageUrl = plan.ImageUrl,
                name = plan.Name,
                description = plan.Description,
                price = plan.Price,
                duration = plan.Duration,
                details = plan.Details.Select(d => new {
                    planDetailsId = d.PlanDetailsId,
                    planId = d.PlanId,
                    value = d.Value
                }).ToList()
            };
            
            var response = await _httpClient.PostAsJsonAsync("api/plans", payload);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error creating plan: {response.StatusCode} - {errorContent}");
                throw new HttpRequestException($"Failed to create plan: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating plan: {ex.Message}");
            throw;
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
        return await Task.Run(() =>
        {
            if (_environment == null)
                return new List<string>();

            var mediaPath = Path.Combine(_environment.WebRootPath, "Media");
            
            if (!Directory.Exists(mediaPath))
                return new List<string>();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
            
            return Directory.GetFiles(mediaPath)
                .Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .Select(f => $"Media/{Path.GetFileName(f)}")
                .OrderBy(x => x)
                .ToList();
        });
    }

    public async Task<string?> SaveImageAsync(Stream fileStream, string fileName)
    {
        try
        {
            if (_environment == null)
                return null;

            var mediaPath = Path.Combine(_environment.WebRootPath, "Media");
            
            if (!Directory.Exists(mediaPath))
                Directory.CreateDirectory(mediaPath);

            // Generate unique filename with original extension
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var newFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(mediaPath, newFileName);

            // Save file
            using (var fileStreamOut = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOut);
            }

            // Return the path format: Media/filename.ext
            return $"Media/{newFileName}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving image: {ex.Message}");
            return null;
        }
    }
}
