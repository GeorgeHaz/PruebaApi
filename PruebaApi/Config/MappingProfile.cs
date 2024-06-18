using AutoMapper;
using PruebaApi.DTO;
using PruebaApi.Modelo;

namespace PruebaApi.Config
{
    public class MappingProfile : Profile 
    { 
        public MappingProfile() 
        {
            CreateMap<UserRegistrationDto, User>();
            CreateMap<UserLoginDto, User>();
        }
    }
}
