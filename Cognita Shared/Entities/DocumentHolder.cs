using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Entities
{
    public class DocumentHolder
    {
        public int DocumentHolderId { get; set; }
        public ICollection<Document> Docs { get; set; }
    }
}
