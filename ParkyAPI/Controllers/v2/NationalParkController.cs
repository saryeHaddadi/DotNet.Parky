using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dto;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers.v2;

[Route("api/v{version:apiVersion}/nationalparks")]
[ApiVersion("2.0")]
[ApiController]
//[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class NationalParkController : ControllerBase
{
	private INationalParkRepository _npRepo;
	private readonly IMapper _mapper;

	public NationalParkController(INationalParkRepository npRepo, IMapper mapper)
	{
		_npRepo = npRepo;
		_mapper = mapper;
	}

	/// <summary>
	/// Get the list of all National Parks
	/// </summary>
	/// <returns></returns>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NationalParkDto>))]
	public IActionResult GetNationalParks()
	{
		var obj = _npRepo.GetNationalParks().FirstOrDefault();
		return Ok(_mapper.Map<NationalParkDto>(obj));
	}

}
