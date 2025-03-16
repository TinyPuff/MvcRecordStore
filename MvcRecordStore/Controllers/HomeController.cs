using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Services;

namespace MvcRecordStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRecordService _recordService;

    public HomeController(ILogger<HomeController> logger, IRecordService recordService)
    {
        _logger = logger;
        _recordService = recordService;
    }

    public IActionResult Index()
    {
        return View(_recordService.GetHomePageViewModel(6));
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
