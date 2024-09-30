using System;
using AutoMapper;
using Cognita_Shared.Dtos.Activity;
using Cognita_Shared.Dtos.Course;
using Cognita_Shared.Entities;

namespace Cognita_Service.AutomapperProfiles;

internal class ActivityProfile : Profile
{
    public ActivityProfile()
    {
        CreateMap<Activity, ActivityDto>().ReverseMap();
    }
}
