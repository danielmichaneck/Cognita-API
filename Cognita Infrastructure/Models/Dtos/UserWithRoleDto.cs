using Cognita_Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Infrastructure.Models.Dtos
{
    public class UserWithRoleDto
    {
        public int Id { get; set; }
        public UserRole Role { get; set; }
    }
}
