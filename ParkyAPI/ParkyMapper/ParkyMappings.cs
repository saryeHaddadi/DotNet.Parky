using AutoMapper;
using ParkyAPI.Models;
using ParkyAPI.Models.Dto;

namespace ParkyAPI.ParkyMapper;

public class ParkyMappings : Profile
{
	public ParkyMappings()
	{
		CreateMap<NationalPark, NationalParkDto>().ReverseMap();
		CreateMap<Trail, TrailDto>().ReverseMap();
		CreateMap<Trail, TrailUpsertDto>().ReverseMap();
	}
}
