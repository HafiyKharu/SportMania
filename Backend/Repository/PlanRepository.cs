using Microsoft.EntityFrameworkCore;
using SportMania.Data;
using SportMania.Models;
using SportMania.Repository.Interface;

namespace SportMania.Repository;

public class PlanRepository : IPlanRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IPlanDetailsRepository _detailsRepository;

    public PlanRepository(ApplicationDbContext db, IPlanDetailsRepository detailsRepository)
    {
        _db = db;
        _detailsRepository = detailsRepository;
    }

    public async Task<Plan?> GetByIdAsync(Guid id)
    {
        return await _db.Plans
            .Include(p => p.Details)
            .FirstOrDefaultAsync(p => p.PlanId == id);
    }

    public async Task<IEnumerable<Plan>> GetAllAsync()
    {
        return await _db.Plans.AsNoTracking()
            .Include(p => p.Details)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Plan plan)
    {
        plan.PlanId = Guid.NewGuid();
        plan.Details = plan.Details
            .Where(d => !string.IsNullOrWhiteSpace(d.Value))
            .Select(d => new PlanDetails { PlanDetailsId = Guid.NewGuid(), Value = d.Value })
            .ToList();

        await _db.Plans.AddAsync(plan);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Plan plan)
    {
        var existing = await _db.Plans.FindAsync(plan.PlanId);
        if (existing == null) return;

        // 1. Update scalar properties of the Plan
        _db.Entry(existing).CurrentValues.SetValues(plan);

        // 2. Delegate Details management to its own repository
        await _detailsRepository.UpsertForPlanAsync(plan.PlanId, plan.Details);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var plan = await GetByIdAsync(id);
        if (plan != null)
        {
            _db.Plans.Remove(plan);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _db.Plans.AnyAsync(p => p.PlanId == id);
    }
}