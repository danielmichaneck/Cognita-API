using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Entities {
    public class ActivityType {
        public int ActivityTypeId { get; set; }
        public required string Title { get; set; }

        // Bool for deadline?
    }
}
