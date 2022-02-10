using Microsoft.EntityFrameworkCore;
using ParkyAPI.Models;

namespace ParkyAPI.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{

	}

	public DbSet<User> Users { get; set; }
	public DbSet<NationalPark> NationalParks { get; set; }
	public DbSet<Trail> Trails { get; set; }
}
