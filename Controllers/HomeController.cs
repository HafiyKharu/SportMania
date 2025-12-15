using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SportMania.Data;
using SportMania.Models;
using SportMania.Repository.Interface;

namespace SportMania.Controllers;

public class HomeController : Controller
{
    private readonly IPlanRepository _planRepository;
    public HomeController(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    } 
    public async Task<IActionResult> Index()
    {
        var plans = await _planRepository.GetAllAsync();
        return View(plans);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
