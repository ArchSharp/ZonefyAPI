using AutoMapper;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Entities;

namespace ZonefyDotnet.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<CreateUserDTO, User>();
            CreateMap<User, GetUserDto>();
        }
    }
}