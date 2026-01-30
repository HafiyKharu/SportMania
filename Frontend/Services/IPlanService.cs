namespace BlazorApp.Services;

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
    public Guid PlanId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public List<PlanDetailDto> Details { get; set; } = new();
}

public class PlanDetailDto
{
    public Guid PlanDetailsId { get; set; }
    public Guid PlanId { get; set; }
    public string Value { get; set; } = string.Empty;
}
