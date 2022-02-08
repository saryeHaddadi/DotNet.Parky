using System.ComponentModel.DataAnnotations;
using static ParkyAPI.Models.Trail;

namespace ParkyAPI.Models.Dto;

public class TrailCreateDto
{
	[Required]
	public string Name { get; set; }

	[Required]
	public double Distance { get; set; }

	public DifficultyType Difficulty { get; set; }

	public int NationalParkId { get; set; }

}
