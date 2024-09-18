using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Shared.Entities {
    public class User {
        public int UserId { get; set; }
        public required string Name { get; set; }

        // Foreign Key
        public int CourseId { get; set; }
    }
}
