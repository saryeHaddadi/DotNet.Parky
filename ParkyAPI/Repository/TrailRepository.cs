using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Repository;

public class TrailRepository : ITrailRepository

{
	private ApplicationDbContext _db;
	public TrailRepository(ApplicationDbContext db)
	{
		_db = db;
	}

	public bool CreateTrail(Trail Trail)
	{
		_db.Trails.Add(Trail);
		return Save();
	}

	public bool DeleteTrail(Trail Trail)
	{
		_db.Trails.Remove(Trail);
		return Save();
	}

	public Trail GetTrail(int TrailId)
	{
		return _db.Trails.Include(c => c.NationalPark).FirstOrDefault(a => a.Id == TrailId);
	}

	public ICollection<Trail> GetTrails()
	{
		return _db.Trails.Include(c => c.NationalPark).OrderBy(a => a.Name).ToList();
	}

	public bool TrailExists(string name)
	{
		return _db.Trails.Any(a => a.Name.Trim().ToLower() == name.Trim().ToLower());
	}

	public bool TrailExists(int id)
	{
		return _db.Trails.Any(a => a.Id == id);
	}

	public bool Save()
	{
		return _db.SaveChanges() >= 0 ? true : false;
	}

	public bool UpdateTrail(Trail Trail)
	{
		_db.Trails.Update(Trail); ;
		return Save();
	}

	public ICollection<Trail> GetTrailsInNationalPark(int nationalParkId)
	{
		return _db.Trails.Include(c => c.NationalPark).Where(c => c.NationalParkId == nationalParkId).ToList();
	}
}