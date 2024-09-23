using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Entities  {
    public class Course : DocumentHolderBase {
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public required string Description { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        //Navigationprops
        public ICollection<Module> Modules { get; set; }
    }
}
