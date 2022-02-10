using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.Interface;
using System.Diagnostics;

namespace ParkyWeb.Controllers;
public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly INationalParkRepository _npRepo;
	private readonly ITrailRepository _trailRepo;

	public HomeController(ILogger<HomeController> logger,
		ITrailRepository trailRepo, INationalParkRepository npRepo)
	{
		_logger = logger;
		_trailRepo = trailRepo;
		_npRepo = npRepo;
	}


	public async Task<IActionResult> Index()
	{
		var obj = new IndexViewModel()
		{
			NationalParkList = await _npRepo.GetAllAsync(SD.NationalParkApiPath),
			TrailList = await _trailRepo.GetAllAsync(SD.TrailApiPath)
		};
		return View(obj);
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
