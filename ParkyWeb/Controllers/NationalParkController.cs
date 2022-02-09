using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.Interface;

namespace ParkyWeb.Controllers;
public class NationalParkController : Controller
{
	private readonly INationalParkRepository _npRepo;
	public NationalParkController(INationalParkRepository npRepo)
	{
		_npRepo = npRepo;
	}
	public IActionResult Index()
	{
		return View(new NationalPark() { });
	}

	public async Task<IActionResult> GetAllNationalPark()
	{
		return Json(new { data = await _npRepo.GetAllAsync(SD.NationalParkApiPath) });
	}
}
