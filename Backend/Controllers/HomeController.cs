using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMania.Data;
using SportMania.Models;

namespace SportMania.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    [HttpGet("plans")]
    public async Task<ActionResult<IEnumerable<Plan>>> GetPlans()
    {
        var plans = await _db.Plans
            .AsNoTracking()
            .Include(p => p.Details)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return Ok(plans);
    }

    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "ok" });
}
