using Cognita_Infrastructure.Data;
using Cognita_Infrastructure.Models.Dtos;
using Cognita_Infrastructure.Models.Entities;
using IntegrationTests;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Http.Headers;

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
        public async Task Get_All_Modules_Success_Test()
        {
            // Arrange

            TokenDto token = await _util.LogInTestUserAsync();
            bool success = false;

            // Act

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/courses/1/modules"))
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

            Assert.True(success);
        }
    }
}