using System;
using AutoMapper;
using Cognita_Shared.Dtos.Module;
using Cognita_Shared.Entities;

namespace Cognita_Service.AutomapperProfiles;

internal class ModuleProfile : Profile
{
    public ModuleProfile()
    {
        CreateMap<Module, ModuleDto>().ReverseMap();
        CreateMap<Module, ModuleForCreationDto>().ReverseMap();
        CreateMap<Module, ModuleForUpdateDto>().ReverseMap();
    }
}
