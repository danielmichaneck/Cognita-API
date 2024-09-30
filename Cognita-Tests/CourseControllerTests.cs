using Cognita.API.Services;
using Cognita_API;
using Cognita_API.Controllers;
using Cognita_API.Infrastructure.Data;
using Cognita_Infrastructure.Models.Dtos;
using Cognita_Infrastructure.Models.Entities;
using IntegrationTests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http.Headers;

namespace Cognita_Tests
{
    [Collection("DbCollection")]
    public class CourseControllerTests
    : IClassFixture<CustomWebApplicationFactory>
    {
        private HttpClient _httpClient;
        private CognitaDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private TestUtil _util;

        const string baseHttpAddress = "https://localhost:7147/api/";

        public CourseControllerTests(CustomWebApplicationFactory applicationFactory) {
            _httpClient = applicationFactory.CreateClient();
            _context = applicationFactory.Context;
            _userManager = applicationFactory.UserManager;
            _util = new TestUtil(_userManager, _httpClient);
        }

        [Fact]
        public async Task GetAllTest_Success() {
            // Arrange

            TokenDto token = await _util.LogInTestUserAsync();
            bool success = false;

            // Act

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/courses"))
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);

                var requestResult = await _httpClient.SendAsync(requestMessage);

                if (requestResult.IsSuccessStatusCode)
                {
                    success = true;
                }
            }

            // Assert

            Xunit.Assert.True(success);
        }

        [Fact]
        public async Task Get_All_Fail_Unautherized_Test()
        {
            // Act
            var response = await _httpClient.GetAsync("api/courses");

            // Assert
            Xunit.Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }
    }
}