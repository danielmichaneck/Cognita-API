using Cognita_Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Dtos.Activity {
    public class ActivityForUpdateDto {
        public required string ActivityName { get; set; }
        public required string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ActivityType? ActivityType { get; set; }
    }
}
