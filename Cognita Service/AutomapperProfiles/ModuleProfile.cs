using System;
using System.Reflection;
using AutoMapper;
using Cognita_Shared.Dtos.Module;

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
