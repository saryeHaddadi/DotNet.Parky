using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Repository;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class NationalParkRepository : INationalParkRepository

{
	private ApplicationDbContext _db;
	public NationalParkRepository(ApplicationDbContext db)
	{
		_db = db;
	}

	public bool CreateNationalPark(NationalPark nationalPark)
	{
		_db.NationalParks.Add(nationalPark);
		return Save();
	}

	public bool DeleteNationalPark(NationalPark nationalPark)
	{
		_db.NationalParks.Remove(nationalPark);
		return Save();
	}

	public NationalPark GetNationalPark(int nationalParkId)
	{
		return _db.NationalParks.FirstOrDefault(a => a.Id == nationalParkId);
	}

	public ICollection<NationalPark> GetNationalParks()
	{
		return _db.NationalParks.OrderBy(a => a.Name).ToList();
	}

	public bool NationalParkExists(string name)
	{
		return _db.NationalParks.Any(a => a.Name.Trim().ToLower() == name.Trim().ToLower());
	}

	public bool NationalParkExists(int id)
	{
		return _db.NationalParks.Any(a => a.Id == id);
	}

	public bool Save()
	{
		return _db.SaveChanges() >= 0 ? true : false;
	}

	public bool UpdateNationalPark(NationalPark nationalPark)
	{
		_db.NationalParks.Update(nationalPark); ;
		return Save();
	}
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member