using AutoMapper;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Dtos.User;
using Cognita_Shared.Entities;

namespace Cognita_Service.AutomapperProfiles;

internal class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
        CreateMap<ApplicationUser, UserForRegistrationDto>().ReverseMap();
        CreateMap<ApplicationUser, UserForUpdateDto>().ReverseMap();
    }
}
