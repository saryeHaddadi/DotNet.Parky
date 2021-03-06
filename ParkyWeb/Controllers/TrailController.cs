using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.Interface;

namespace ParkyWeb.Controllers;

[Authorize]
public class TrailController : Controller
{
	private readonly INationalParkRepository _npRepo;
	private readonly ITrailRepository _trailRepo;
	public TrailController(ITrailRepository trailRepo, INationalParkRepository npRepo)
	{
		_trailRepo = trailRepo;
		_npRepo = npRepo;
	}
	public IActionResult Index()
	{
		return View(new Trail() { });
	}

	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Upsert(int? id)
	{
		var npList = await _npRepo.GetAllAsync(SD.NationalParkApiPath, HttpContext.Session.GetString("JWToken"));
		var objVM = new TrailViewModel()
		{
			NationalParkList = npList.Select(id => new SelectListItem
			{
				Text = id.Name,
				Value = id.Id.ToString()
			}),
			Trail = new Trail()
		};

		// Insert
		if (id is null)
		{
			return View(objVM);
		}

		// Update
		objVM.Trail = await _trailRepo.GetAsync(SD.TrailApiPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
		if (objVM.Trail is null)
		{
			return NotFound();
		}
		return View(objVM);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Upsert(TrailViewModel obj)
	{
		if (ModelState.IsValid)
		{
			if (obj.Trail.Id == 0)
			{
				await _trailRepo.CreateAsync(SD.TrailApiPath, obj.Trail, HttpContext.Session.GetString("JWToken"));
			}
			else
			{
				await _trailRepo.UpdateAsync(SD.TrailApiPath + obj.Trail.Id, obj.Trail, HttpContext.Session.GetString("JWToken"));
			}

			return RedirectToAction(nameof(Index));
		}
		else
		{
			var npList = await _npRepo.GetAllAsync(SD.NationalParkApiPath, HttpContext.Session.GetString("JWToken"));
			var objVM = new TrailViewModel()
			{
				NationalParkList = npList.Select(id => new SelectListItem
				{
					Text = id.Name,
					Value = id.Id.ToString()
				}),
				Trail = obj.Trail
			};
			return View(objVM);
		}
	}

	public async Task<IActionResult> GetAllTrails()
	{
		return Json(new { data = await _trailRepo.GetAllAsync(SD.TrailApiPath, HttpContext.Session.GetString("JWToken")) });
	}

	[HttpDelete]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int id)
	{
		var status = await _trailRepo.DeleteAsync(SD.TrailApiPath, id, HttpContext.Session.GetString("JWToken"));
		if (status)
		{
			return Json(new { success = true, message = "Delete Successful" });
		}
		return Json(new { success = false, message = "Delete Not Successful" });
	}
}
