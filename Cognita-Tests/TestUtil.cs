﻿using Cognita_Infrastructure.Models.Dtos;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Entities;
using Cognita_Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Net.Http.Json;

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
        private readonly HttpClient _httpClient;

        private bool _userSeeded = false;

        public TestUtil(UserManager<ApplicationUser> userManager, HttpClient httpClient) {
            _userManager = userManager;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Seeds test user based on constant if one does not exist.
        /// </summary>
        /// <returns></returns>
        private async Task SeedTestUserAsync() {
            if (_userSeeded) return;
            _userSeeded = true;

            var exists = await _userManager.FindByNameAsync(USER_SEED_EMAIL);
            if (exists != null) return;

            var user = new ApplicationUser {
                UserName = USER_SEED_EMAIL,
                Email = USER_SEED_EMAIL,
                Name = USER_SEED_NAME,
                CourseId = USER_SEED_COURSE_ID
            };

            try {
                await _userManager.CreateAsync(user, USER_SEED_PASSWORD);
            }
            catch (Exception ex) {
                return;
            }
        }

        /// <summary>
        /// Returns a dto for logging in a test user.
        /// </summary>
        /// <returns></returns>
        internal async Task<UserForAuthenticationDto> GetTestUserAuthenticationDtoAsync() {
            await SeedTestUserAsync();
            return new UserForAuthenticationDto() {
                UserName = USER_SEED_EMAIL,
                Password = USER_SEED_PASSWORD
            };
        }

        /// <summary>
        /// Gets a Token for a logged in test user.
        /// </summary>
        /// <returns></returns>
        internal async Task<TokenDto> LogInTestUserAsync() {
            await SeedTestUserAsync();
            var baseResponse = await _httpClient.PostAsJsonAsync("api/authentication/login", await GetTestUserAuthenticationDtoAsync());
            var jsonResponse = await baseResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenDto>(jsonResponse);
        }
    }
}
