using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Dtos.Document
{
    public class DocumentDto
    {
        public string DocumentName { get; set; }
        public DateTime UploadingTime { get; set; }
        public string FilePath { get; set; }
    }
}
