using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dto;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers.v1;

[Route("api/v{version:apiVersion}/trails")]
[ApiVersion("1.0")]
[ApiController]
//[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class TrailController : ControllerBase
{
	private ITrailRepository _trailRepo;
	private readonly IMapper _mapper;

	public TrailController(ITrailRepository trailRepo, IMapper mapper)
	{
		_trailRepo = trailRepo;
		_mapper = mapper;
	}

	/// <summary>
	/// Get the list of all Trails
	/// </summary>
	/// <returns></returns>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TrailDto>))]
	public IActionResult GetTrails()
	{
		var objList = _trailRepo.GetTrails();
		var objDto = new List<TrailDto>();

		foreach (var obj in objList)
		{
			objDto.Add(_mapper.Map<TrailDto>(obj));
		}

		return Ok(objDto);
	}

	/// <summary>
	/// Get an indivual Trail
	/// </summary>
	/// <param name="trailId">The Id of the Trail</param>
	/// <returns></returns>
	[HttpGet("{trailId:int}", Name = nameof(GetTrail))]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesDefaultResponseType]
	[Authorize(Roles = "Admin")]
	public IActionResult GetTrail(int trailId)
	{
		var obj = _trailRepo.GetTrail(trailId);
		if (obj == null)
		{
			return NotFound();
		}

		var objDto = _mapper.Map<TrailDto>(obj);
		//var objDto = new TrailDto()
		//{
		//	Id = obj.Id,
		//	Name = obj.Name,
		//	State = obj.State,
		//	Created = obj.Created,
		//};
		return Ok(objDto);
	}

	[HttpGet("[action]/{nationalParkId:int}", Name = nameof(GetTrailInNationalPark))]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TrailDto>))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesDefaultResponseType]
	public IActionResult GetTrailInNationalPark(int nationalParkId)
	{
		var objList = _trailRepo.GetTrailsInNationalPark(nationalParkId);
		if (objList == null)
		{
			return NotFound();
		}

		var objDtoList = new List<TrailDto>();
		foreach(var obj in objList)
		{
			objDtoList.Add(_mapper.Map<TrailDto>(obj));
		}

		return Ok(objDtoList);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TrailDto))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
	{
		if (trailDto == null)
		{
			return BadRequest(ModelState);
		}

		if(_trailRepo.TrailExists(trailDto.Name))
		{
			ModelState.AddModelError("", "This Trail already exists.");
			return StatusCode(404, ModelState);
		}

		var trailObj = _mapper.Map<Trail>(trailDto);
		if(!_trailRepo.CreateTrail(trailObj))
		{
			ModelState.AddModelError("", $"Something went wrong when saving the record {trailObj.Name}");
			return StatusCode(500, ModelState);
		}

		return CreatedAtRoute(nameof(GetTrail), new { trailId = trailObj.Id }, trailObj);
	}

	[HttpPatch("{trailId:int}", Name = nameof(UpdateTrail))]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpdateDto trailDto)
	{
		if (trailDto == null || trailId != trailDto.Id)
		{
			return BadRequest(ModelState);
		}

		var trailObj = _mapper.Map<Trail>(trailDto);
		if (!_trailRepo.UpdateTrail(trailObj))
		{
			ModelState.AddModelError("", $"Something went wrong when updating the record {trailObj.Name}");
			return StatusCode(500, ModelState);
		}

		return NoContent();
	}

	[HttpDelete("{trailId:int}", Name = nameof(DeleteTrail))]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public IActionResult DeleteTrail(int trailId)
	{
		if (!_trailRepo.TrailExists(trailId))
		{
			return NotFound();
		}

		var trailObj = _trailRepo.GetTrail(trailId);
		if (!_trailRepo.DeleteTrail(trailObj))
		{
			ModelState.AddModelError("", $"Something went wrong when deleting the record {trailObj.Name}");
			return StatusCode(500, ModelState);
		}

		return NoContent();
	}
}
