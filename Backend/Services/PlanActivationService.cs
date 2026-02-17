using SportMania.Models;
using SportMania.Repository.Interface;
using SportMania.Services.Interface;

namespace SportMania.Services;

public class PlanActivationService(
    IPlanRepository _planRepository,
    IConfiguration _configuration,
    ILogger<PlanActivationService> _logger,
    IToyyibPayService _toyyibPayService)
{
    /// <summary>
    /// Refreshes plan activation status by checking if category codes exist in appsettings
    /// </summary>
    public async Task RefreshPlanActivationAsync()
    {
        var plans = await _planRepository.GetAllAsync();
        
        foreach (var plan in plans)
        {
            var categoryCode = _toyyibPayService.GetCategoryCode(plan.Name);
            var wasActivated = plan.IsActivated;
            plan.IsActivated = !string.IsNullOrWhiteSpace(categoryCode);

            if (wasActivated != plan.IsActivated)
            {
                await _planRepository.UpdateAsync(plan);
                _logger.LogInformation(
                    "Plan '{PlanName}' activation status changed from {OldStatus} to {NewStatus}",
                    plan.Name, wasActivated, plan.IsActivated);
            }
        }
    }

    /// <summary>
    /// Checks if a specific plan has a category code configured
    /// </summary>
    public bool IsPlanActivated(Plan plan)
    {
        var categoryCode = _configuration[$"Plans:{plan.Name}:CategoryCode"];
        return !string.IsNullOrWhiteSpace(categoryCode);
    }
}
