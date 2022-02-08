using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dto;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class NationalParkController : Controller
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
		var objList = _npRepo.GetNationalParks();
		var objDto = new List<NationalParkDto>();

		foreach (var obj in objList)
		{
			objDto.Add(_mapper.Map<NationalParkDto>(obj));
		}

		return Ok(objDto);
	}

	/// <summary>
	/// Get an indivual National Park
	/// </summary>
	/// <param name="nationalParkId">The Id of the National Park</param>
	/// <returns></returns>
	[HttpGet("{nationalParkId:int}", Name = nameof(GetNationalPark))]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NationalParkDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesDefaultResponseType]
	public IActionResult GetNationalPark(int nationalParkId)
	{
		var obj = _npRepo.GetNationalPark(nationalParkId);
		if (obj == null)
		{
			return NotFound();
		}

		var objDto = _mapper.Map<NationalParkDto>(obj);
		//var objDto = new NationalParkDto()
		//{
		//	Id = obj.Id,
		//	Name = obj.Name,
		//	State = obj.State,
		//	Created = obj.Created,
		//};
		return Ok(objDto);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NationalParkDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
	{
		if (nationalParkDto == null)
		{
			return BadRequest(ModelState);
		}

		if(_npRepo.NationalParkExists(nationalParkDto.Name))
		{
			ModelState.AddModelError("", "This National Park already exists.");
			return StatusCode(404, ModelState);
		}

		var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
		if(!_npRepo.CreateNationalPark(nationalParkObj))
		{
			ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkObj.Name}");
			return StatusCode(500, ModelState);
		}

		return CreatedAtRoute(nameof(GetNationalPark), new { nationalParkId = nationalParkObj.Id }, nationalParkObj);
	}

	[HttpPatch("{nationalParkId:int}", Name = nameof(UpdateNationalPark))]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
	{
		if (nationalParkDto == null || nationalParkId != nationalParkDto.Id)
		{
			return BadRequest(ModelState);
		}

		var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
		if (!_npRepo.UpdateNationalPark(nationalParkObj))
		{
			ModelState.AddModelError("", $"Something went wrong when updating the record {nationalParkObj.Name}");
			return StatusCode(500, ModelState);
		}

		return NoContent();
	}

	[HttpDelete("{nationalParkId:int}", Name = nameof(DeleteNationalPark))]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public IActionResult DeleteNationalPark(int nationalParkId)
	{
		if (!_npRepo.NationalParkExists(nationalParkId))
		{
			return NotFound();
		}

		var nationalParkObj = _npRepo.GetNationalPark(nationalParkId);
		if (!_npRepo.DeleteNationalPark(nationalParkObj))
		{
			ModelState.AddModelError("", $"Something went wrong when deleting the record {nationalParkObj.Name}");
			return StatusCode(500, ModelState);
		}

		return NoContent();
	}
}
