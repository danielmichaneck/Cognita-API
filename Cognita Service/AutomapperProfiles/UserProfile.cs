using System;
using AutoMapper;
using Cognita_Shared.Dtos.User;
using Cognita_Shared.Entities;

namespace Cognita_Service.AutomapperProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserForCreationDto>().ReverseMap();
        CreateMap<User, UserForUpdateDto>().ReverseMap();
    }
}
