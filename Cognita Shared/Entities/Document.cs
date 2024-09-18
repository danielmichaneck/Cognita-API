using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Entities {
    public class Document {
        public int DocumentId { get; set; }
        public string Description { get; set; }
        public string DocumentName { get; set; }
        public DateTime UploadingTime { get; set; }
        public string FilePath { get; set; }
        public int UserId { get; set; }
        public int DocumentHolderId { get; set; }
    }
}
