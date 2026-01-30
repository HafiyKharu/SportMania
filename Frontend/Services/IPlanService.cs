namespace BlazorApp.Services;

using System.Text.Json.Serialization;

public interface IPlanService
{
    Task<List<PlanDto>> GetAllPlansAsync();
    Task<PlanDto?> GetPlanByIdAsync(Guid id);
    Task CreatePlanAsync(PlanDto plan);
    Task UpdatePlanAsync(Guid id, PlanDto plan);
    Task DeletePlanAsync(Guid id);
    Task<List<string>> GetMediaPathsAsync();
}

public class PlanDto
{
    [JsonPropertyName("planId")]
    public Guid PlanId { get; set; }
    
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;
    
    [JsonPropertyName("duration")]
    public string Duration { get; set; } = string.Empty;
    
    [JsonPropertyName("details")]
    public List<PlanDetailDto> Details { get; set; } = new();
    
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }
}

public class PlanDetailDto
{
    [JsonPropertyName("planDetailsId")]
    public Guid PlanDetailsId { get; set; }
    
    [JsonPropertyName("planId")]
    public Guid PlanId { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}
