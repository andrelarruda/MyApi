using AutoMapper;
using MyApi.Models;
using MyApi.Models.DTOs;

namespace MyApi.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Member, MemberDto>()
                .ForMember(dest => dest.CPF, opt => opt.MapFrom((src, dest) => src.CPF)) // just an unnecessary example
                .ReverseMap();
        }
    }
}
