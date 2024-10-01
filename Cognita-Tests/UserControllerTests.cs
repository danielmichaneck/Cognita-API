using System.Net.Http.Json;
using IntegrationTests;
using Cognita_API.Infrastructure.Data;
using System.Net;
using Cognita_Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Cognita_Infrastructure.Models.Entities;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace Cognita_Tests
{
    //[DoNotParallelize]
    [Collection("DbCollection")]
    public class UserControllerTests
    {
        private readonly HttpClient _httpClient;
        private readonly CognitaDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly TestUtil _util;

        const string baseHttpAddress = "https://localhost:7147/api/";

        public UserControllerTests(CustomWebApplicationFactory applicationFactory)
        {
            _httpClient = applicationFactory.CreateClient();
            _context = applicationFactory.Context;
            _userManager = applicationFactory.UserManager;
            _util = new TestUtil(_userManager, _httpClient);
        }

        [Fact]
        public async Task Get_All_Users_Test_Success()
        {
            // Arrange

            TokenDto token = await _util.LogInTestUserAsync();
            bool success = false;

            // Act

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/users"))
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

        [Fact]
        public async Task Get_Users_In_Course_Test_Success()
        {
            // Arrange

            TokenDto token = await _util.LogInTestUserAsync();
            bool success = false;

            // Act

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/courses/1/users"))
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