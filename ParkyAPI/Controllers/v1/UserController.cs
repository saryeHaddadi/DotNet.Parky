using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers.v1;

[Authorize]
[Route("api/v{version:apiVersion}/user")]
[ApiVersion("1.0")]
[ApiController]
public class UserController : ControllerBase
{
	private readonly IUserRepository _userRepo;
	public UserController(IUserRepository userRepo)
	{
		_userRepo = userRepo;
	}

	[AllowAnonymous]
	[HttpPost("authenticate")]
	public IActionResult Authenticate([FromBody] User model)
	{
		var user = _userRepo.Authenticate(model.Username, model.Password);
		if (user == null)
		{
			return BadRequest(new { message = "Username of password is incorrect" });
		}
		return Ok(user);
	}

	[AllowAnonymous]
	[HttpPost("register")]
	public IActionResult Register([FromBody] User model)
	{
		bool isUserNameUnique = _userRepo.IsUniqueUser(model.Username);
		if(!isUserNameUnique)
		{
			return BadRequest(new { message = "Username already exists" });
		}

		var user = _userRepo.Register(model.Username, model.Password);
		if(user == null)
		{
			return BadRequest(new { message = "Error while registering" });
		}
		return Ok();
	}
}
