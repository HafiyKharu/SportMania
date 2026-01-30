using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMania.Data;
using SportMania.Models;

namespace SportMania.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var plans = await _db.Plans
            .AsNoTracking()
            .Include(p => p.Details)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(plans);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Plan() => RedirectToAction(nameof(Index));

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
