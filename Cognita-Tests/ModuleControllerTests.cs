using Cognita_Infrastructure.Data;
using Cognita_Infrastructure.Models.Dtos;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Dtos.Course;
using Cognita_Shared.Dtos.Module;
using IntegrationTests;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Cognita_Tests
{
    //[DoNotParallelize]
    [Collection("DbCollection")]
    public class ModuleControllerTests
    {
        private HttpClient _httpClient;
        private CognitaDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private TestUtil _util;

        const string baseHttpAddress = "https://localhost:7147/api/";

        public ModuleControllerTests(CustomWebApplicationFactory applicationFactory)
        {
            _httpClient = applicationFactory.CreateClient();
            _context = applicationFactory.Context;
            _userManager = applicationFactory.UserManager;
            _util = new TestUtil(_userManager, _httpClient);
        }

        [Fact]
        public async Task Create_Module_Success_Test() {

            // Arrange

            TokenDto token = await _util.LogInTestTeacherAsync();

            var newModule = new ModuleForCreationDto() {
                ModuleName = "Test Module 1",
                Description = "Testing at Create_Module_Success_Test",
                StartDate = DateOnly.MinValue,
                EndDate = DateOnly.MaxValue
            };

            // Act

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var response = await _httpClient.PostAsJsonAsync("api/courses/1/modules", newModule);

            // Assert

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Edit_Module_Success_Test() {
            // Arrange

            TokenDto token = await _util.LogInTestTeacherAsync();

            var newModule = new ModuleForUpdateDto() {
                ModuleName = "Test-module-1",
                Description = "This is a test course generated in the Create_Module_Success_Test",
                StartDate = DateOnly.MinValue,
                EndDate = DateOnly.MaxValue
            };

            // Act

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var requestResult = await _httpClient.PutAsJsonAsync("api/modules/1", newModule);

            // Assert

            Assert.True(requestResult.IsSuccessStatusCode);
        }
    }
}