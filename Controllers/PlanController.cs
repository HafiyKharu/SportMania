using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMania.Data;
using SportMania.Models;

namespace SportMania.Controllers;

public class PlanController : Controller
{
    private readonly ApplicationDbContext _db;
    public PlanController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Plan()
    {
        var plans = await _db.Plans
            .AsNoTracking()
            .Include(p => p.Details)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(plans);
    }
}