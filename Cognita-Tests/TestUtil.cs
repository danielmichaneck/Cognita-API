using Cognita_Infrastructure.Models.Dtos;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Entities;
using Cognita_Shared.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Tests
{
    internal class TestUtil
    {
        private const string USER_SEED_NAME = "Kalle Anka";
        private const string USER_SEED_EMAIL = "kalle@ankeborg.anka";
        private const string USER_SEED_PASSWORD = "KallesPassword123!";
        private const int USER_SEED_COURSE_ID = 1;
        private const UserRole USER_SEED_ROLE = UserRole.Student;

        private readonly UserManager<ApplicationUser> _userManager;
        private bool _userSeeded = false;

        public TestUtil(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        internal async Task SeedTestUserAsync() {
            if (_userSeeded) return;
            var user = new ApplicationUser {
                UserName = USER_SEED_EMAIL,
                Email = USER_SEED_EMAIL,
                User = new User {
                    Name = USER_SEED_NAME,
                    Role = USER_SEED_ROLE,
                    CourseId = USER_SEED_COURSE_ID
                }
            };

            _userSeeded = true;

            await _userManager.CreateAsync(user, USER_SEED_PASSWORD);
        }

        internal UserForAuthenticationDto GetTestUserAuthenticationDto() {
            return new UserForAuthenticationDto() {
                UserName = USER_SEED_EMAIL,
                Password = USER_SEED_PASSWORD
            };
        }
    }
}
