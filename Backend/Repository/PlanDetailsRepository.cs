using Microsoft.EntityFrameworkCore;
using SportMania.Data;
using SportMania.Models;
using SportMania.Repository.Interface;

namespace SportMania.Repository;

public class PlanDetailsRepository : IPlanDetailsRepository
{
    private readonly ApplicationDbContext _db;

    public PlanDetailsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task UpsertForPlanAsync(Guid planId, IEnumerable<PlanDetails> incomingDetails)
    {
        var validIncoming = incomingDetails
            .Where(d => !string.IsNullOrWhiteSpace(d.Value))
            .ToList();

        var existingDetails = await _db.PlanDetails
            .Where(d => d.PlanId == planId)
            .ToListAsync();

        // 1. Remove details that are no longer present
        var detailsToRemove = existingDetails
            .Where(ed => !validIncoming.Any(id => id.PlanDetailsId == ed.PlanDetailsId))
            .ToList();

        if (detailsToRemove.Any())
        {
            _db.PlanDetails.RemoveRange(detailsToRemove);
        }

        // 2. Update existing or add new details
        foreach (var incoming in validIncoming)
        {
            var existing = existingDetails.FirstOrDefault(d => d.PlanDetailsId == incoming.PlanDetailsId);
            if (existing != null)
            {
                // Update
                existing.Value = incoming.Value;
            }
            else
            {
                // Add new
                await _db.PlanDetails.AddAsync(new PlanDetails
                {
                    PlanDetailsId = incoming.PlanDetailsId == Guid.Empty ? Guid.NewGuid() : incoming.PlanDetailsId,
                    Value = incoming.Value,
                    PlanId = planId
                });
            }
        }
    }
}