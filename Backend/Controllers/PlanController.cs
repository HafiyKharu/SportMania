using Microsoft.AspNetCore.Mvc;
using SportMania.Models;
using SportMania.Repository.Interface;

namespace SportMania.Controllers;

[ApiController]
[Route("api/plans")]
public class PlanController : ControllerBase
{
    private readonly IPlanRepository _planRepository;

    public PlanController(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Plan>>> GetAll()
    {
        var plans = await _planRepository.GetAllAsync();
        return Ok(plans);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Plan>> GetById(Guid id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return NotFound();
        }

        return Ok(plan);
    }

    [HttpPost]
    public async Task<ActionResult<Plan>> Create([FromBody] Plan plan)
    {
        await _planRepository.AddAsync(plan);
        return CreatedAtAction(nameof(GetById), new { id = plan.PlanId }, plan);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Plan plan)
    {
        if (id != plan.PlanId)
        {
            return BadRequest(new { error = "Mismatched plan id." });
        }

        await _planRepository.UpdateAsync(plan);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _planRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("media")]
    public ActionResult<IEnumerable<string>> GetMedia()
    {
        return Ok(GetMediaPaths());
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