using AutoMapper;
using Cognita_Shared.Dtos.Course;
using Cognita_Shared.Dtos.Document;
using Cognita_Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Service.AutomapperProfiles
{
    internal class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<DocumentDto, Document>().ReverseMap();
        }
    }
}
