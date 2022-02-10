using System.ComponentModel.DataAnnotations;

namespace ParkyAPI.Models.Dto;

public class UserAuthenticationDto
{
	[Required]
	public string Username { get; set; }
	[Required]
	public string Password { get; set; }
}
