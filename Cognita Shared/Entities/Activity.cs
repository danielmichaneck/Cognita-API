using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Entities {
    public class Activity : DocumentHolderBase {
        public int ActivityId { get; set; }
        public required string Description { get; set; }
        public required string ActivityName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        //Foreign Key
        public int ModuleId { get; set; }
        public int ActivityTypeId { get; set; }
    }
}
