using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Entities {
    public class DocumentHolderBase {
        public ICollection<Document> Documents { get; set; }
    }
}
