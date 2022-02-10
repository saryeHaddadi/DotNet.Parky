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

	public async Task<IActionResult> Upsert(int? id)
	{
		var obj = new NationalPark();

		// Insert
		if (id is null)
		{
			return View(obj);
		}

		// Update
		obj = await _npRepo.GetAsync(SD.NationalParkApiPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
		if (obj is null)
		{
			return NotFound();
		}
		return View(obj);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Upsert(NationalPark obj)
	{
		if (ModelState.IsValid)
		{
			var files = HttpContext.Request.Form.Files;
			if (files.Count > 0)
			{
				byte[] picture1 = null;
				using (var fs1 = files[0].OpenReadStream())
				{
					using (var ms1 = new MemoryStream())
					{
						fs1.CopyTo(ms1);
						picture1 = ms1.ToArray();
					}
				}
				obj.Picture = picture1;
			}
			else
			{
				var objFromDb = await _npRepo.GetAsync(SD.NationalParkApiPath, obj.Id, HttpContext.Session.GetString("JWToken"));
				obj.Picture = objFromDb.Picture;
			}

			if (obj.Id == 0)
			{
				await _npRepo.CreateAsync(SD.NationalParkApiPath, obj, HttpContext.Session.GetString("JWToken"));
			}
			else
			{
				await _npRepo.UpdateAsync(SD.NationalParkApiPath + obj.Id, obj, HttpContext.Session.GetString("JWToken"));
			}

			return RedirectToAction(nameof(Index));
		}
		else
		{
			return View(obj);
		}
	}

	public async Task<IActionResult> GetAllNationalParks()
	{
		return Json(new { data = await _npRepo.GetAllAsync(SD.NationalParkApiPath, HttpContext.Session.GetString("JWToken")) });
	}

	[HttpDelete]
	public async Task<IActionResult> Delete(int id)
	{
		var status = await _npRepo.DeleteAsync(SD.NationalParkApiPath, id, HttpContext.Session.GetString("JWToken"));
		if (status)
		{
			return Json(new { success = true, message = "Delete Successful" });
		}
		return Json(new { success = false, message = "Delete Not Successful" });
	}
}
