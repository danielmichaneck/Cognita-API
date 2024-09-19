using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Entities {
    public class Module : DocumentHolderBase {
        public int ModuleId { get; set; }
        public required string ModuleName { get; set; }
        public required string Description { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        // Foreign Key
        public int CourseId { get; set; }

        //Navigationprops
        public ICollection<Activity> Activities { get; set; }
    }
}
