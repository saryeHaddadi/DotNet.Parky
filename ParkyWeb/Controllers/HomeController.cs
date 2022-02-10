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
	private readonly IAccountRepository _accountRepo;
	private readonly ITrailRepository _trailRepo;

	public HomeController(ILogger<HomeController> logger, IAccountRepository accountRepo,
		ITrailRepository trailRepo, INationalParkRepository npRepo)
	{
		_logger = logger;
		_accountRepo = accountRepo;
		_trailRepo = trailRepo;
		_npRepo = npRepo;
	}


	public async Task<IActionResult> Index()
	{
		var obj = new IndexViewModel()
		{
			NationalParkList = await _npRepo.GetAllAsync(SD.NationalParkApiPath, HttpContext.Session.GetString("JWToken")),
			TrailList = await _trailRepo.GetAllAsync(SD.TrailApiPath, HttpContext.Session.GetString("JWToken"))
		};
		return View(obj);
	}

	[HttpGet]
	public IActionResult Login()
	{
		var user = new User();
		return View(user);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(User obj)
	{
		var objUser = await _accountRepo.LoginAsync(SD.AccountApiPath + "authenticate/", obj);
		if (objUser.Token is null)
		{
			return View();
		}
		HttpContext.Session.SetString("JWToken", objUser.Token);
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(User obj)
	{
		var registered = await _accountRepo.RegisterAsync(SD.AccountApiPath + "register/", obj);
		if (!registered)
		{
			return View();
		}
		return RedirectToAction(nameof(Login));
	}

	[HttpGet]
	public IActionResult Logout()
	{
		HttpContext.Session.SetString("JWToken", "");
		return RedirectToAction(nameof(Index));
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
