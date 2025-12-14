using Microsoft.AspNetCore.Mvc;
using SportMania.Models;
using SportMania.Repository.Interface;

namespace SportMania.Controllers;

public class PlanController : Controller
{
    private readonly IPlanRepository _planRepository;

    public PlanController(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<IActionResult> Index()
    {
        var plans = await _planRepository.GetAllAsync();
        return View(plans);
    }

    public IActionResult Create()
    {
        ViewBag.MediaFiles = GetMediaPaths();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Plan plan)
    {
        if (ModelState.IsValid)
        {
            await _planRepository.AddAsync(plan);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.MediaFiles = GetMediaPaths();
        return View(plan);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return NotFound();
        }

        ViewBag.MediaFiles = GetMediaPaths();
        return View(plan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Plan plan)
    {
        if (id != plan.PlanId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _planRepository.UpdateAsync(plan);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.MediaFiles = GetMediaPaths();
        return View(plan);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return NotFound();
        }
        return View(plan);
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return NotFound();
        }
        return View(plan);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _planRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private static readonly string[] ImageExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp", ".svg" };

    private List<string> GetMediaPaths()
    {
        var root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Media");
        if (!Directory.Exists(root)) return new List<string>();

        return Directory.EnumerateFiles(root)
            .Where(f => ImageExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
            .Select(f => "/Media/" + Path.GetFileName(f))
            .OrderBy(x => x)
            .ToList();
    }
}