using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.Interface;

namespace ParkyWeb.Controllers;
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

	public async Task<IActionResult> Upsert(int? id)
	{
		var npList = await _npRepo.GetAllAsync(SD.NationalParkApiPath);
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
		objVM.Trail = await _trailRepo.GetAsync(SD.TrailApiPath, id.GetValueOrDefault());
		if (objVM.Trail is null)
		{
			return NotFound();
		}
		return View(objVM);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Upsert(TrailViewModel obj)
	{
		if (ModelState.IsValid)
		{
			if (obj.Trail.Id == 0)
			{
				await _trailRepo.CreateAsync(SD.TrailApiPath, obj.Trail);
			}
			else
			{
				await _trailRepo.UpdateAsync(SD.TrailApiPath + obj.Trail.Id, obj.Trail);
			}

			return RedirectToAction(nameof(Index));
		}
		else
		{
			var npList = await _npRepo.GetAllAsync(SD.NationalParkApiPath);
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
		return Json(new { data = await _trailRepo.GetAllAsync(SD.TrailApiPath) });
	}

	[HttpDelete]
	public async Task<IActionResult> Delete(int id)
	{
		var status = await _trailRepo.DeleteAsync(SD.TrailApiPath, id);
		if (status)
		{
			return Json(new { success = true, message = "Delete Successful" });
		}
		return Json(new { success = false, message = "Delete Not Successful" });
	}
}
