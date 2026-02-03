using BlazorApp.Dtos;

namespace BlazorApp.Services;

public interface IPlanService
{
    Task<List<PlanDto>> GetAllPlansAsync();
    Task<PlanDto?> GetPlanByIdAsync(Guid id);
    Task CreatePlanAsync(PlanDto plan);
    Task UpdatePlanAsync(Guid id, PlanDto plan);
    Task DeletePlanAsync(Guid id);
    Task<List<string>> GetMediaPathsAsync();
    Task<string?> SaveImageAsync(Stream fileStream, string fileName);
}
