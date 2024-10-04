using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Dtos.Document
{
    public class DocumentForCreationDto
    {
        public string Description { get; set; }
        public string DocumentName { get; set; }
        public int UserId { get; set; }
    }
}
