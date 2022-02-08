using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;
using AutoMapper;
using ParkyAPI.ParkyMapper;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<INationalParkRepository, NationalParkRepository>();
builder.Services.AddScoped<ITrailRepository, TrailRepository>();
builder.Services.AddAutoMapper(typeof(ParkyMappings));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
	options.SwaggerDoc("ParkyOpenAPISpecNP",
		new Microsoft.OpenApi.Models.OpenApiInfo()
		{
			Title = "Parky API (National Parks)",
			Version = "1",
			Description = "Udemy Parky API",
			Contact = new Microsoft.OpenApi.Models.OpenApiContact()
			{
				Email = "sarye.dev@outlook.fr",
				Name = "Sarye HADDADI",
				Url = new Uri("https://www.sarye.com")
			},
			License = new Microsoft.OpenApi.Models.OpenApiLicense()
			{
				Name = "MIT License",
				Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
			}
		});
	options.SwaggerDoc("ParkyOpenAPISpecTrails",
		new Microsoft.OpenApi.Models.OpenApiInfo()
		{
			Title = "Parky API Trails",
			Version = "1",
			Description = "Udemy Parky API",
			Contact = new Microsoft.OpenApi.Models.OpenApiContact()
			{
				Email = "sarye.dev@outlook.fr",
				Name = "Sarye HADDADI",
				Url = new Uri("https://www.sarye.com")
			},
			License = new Microsoft.OpenApi.Models.OpenApiLicense()
			{
				Name = "MIT License",
				Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
			}
		});
	var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlCommentFileFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
	Console.WriteLine(xmlCommentFileFullPath);
	options.IncludeXmlComments(xmlCommentFileFullPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/ParkyOpenAPISpecNP/swagger.json", "Parky API NP");
		options.SwaggerEndpoint("/swagger/ParkyOpenAPISpecTrails/swagger.json", "Parky API Trails");
		options.RoutePrefix = "";
	});
}

app.UseAuthorization();

app.MapControllers();

app.Run();
