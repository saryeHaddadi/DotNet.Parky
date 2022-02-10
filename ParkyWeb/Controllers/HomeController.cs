using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.Interface;
using System.Diagnostics;
using System.Security.Claims;

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

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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

		var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
		identity.AddClaim(new Claim(ClaimTypes.Name, objUser.Username));
		identity.AddClaim(new Claim(ClaimTypes.Role, objUser.Role));
		var principal = new ClaimsPrincipal(identity);
		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal); // User signed in
		
		HttpContext.Session.SetString("JWToken", objUser.Token); // Needed for API calls
		TempData["alert"] = "Welcome " + objUser.Username;
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
		TempData["alert"] = "Registration Successful!";
		return RedirectToAction(nameof(Login));
	}

	[HttpGet]
	public async Task<IActionResult> Logout()
	{
		await HttpContext.SignOutAsync();
		HttpContext.Session.SetString("JWToken", "");
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public IActionResult AccessDenied()
	{
		return View();
	}
}
