﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dto;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NationalParkController : Controller
{
	private INationalParkRepository _npRepo;
	private readonly IMapper _mapper;

	public NationalParkController(INationalParkRepository npRepo, IMapper mapper)
	{
		_npRepo = npRepo;
		_mapper = mapper;
	}

	[HttpGet]
	public IActionResult GetNaionalParks()
	{
		var objList = _npRepo.GetNationalParks();
		var objDto = new List<NationalParkDto>();

		foreach (var obj in objList)
		{
			objDto.Add(_mapper.Map<NationalParkDto>(obj));
		}

		return Ok(objDto);
	}

	[HttpGet("{nationalParkId:int}")]
	public IActionResult GetNationalPark(int nationalParkId)
	{
		var obj = _npRepo.GetNationalPark(nationalParkId);
		if (obj == null)
		{
			return NotFound();
		}

		var objDto = _mapper.Map<NationalParkDto>(obj);
		return Ok(objDto);
	}

	[HttpPost]
	public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
	{
		if (nationalParkDto == null)
		{
			return BadRequest(ModelState);
		}
		
		if(!ModelState.IsValid)
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

		return Ok();
	}
}
