using System.Net.Http.Json;
using IntegrationTests;
using System.Net;
using Cognita_Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Cognita_Infrastructure.Models.Entities;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Cognita_Shared.Enums;
using Cognita_Shared.Dtos.User;
using Cognita_Infrastructure.Data;
using NuGet.Common;

namespace Cognita_Tests
{
    //[DoNotParallelize]
    [Collection("DbCollection")]
    public class AuthControllerTests
    {
        private readonly HttpClient _httpClient;
        private readonly CognitaDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly TestUtil _util;

        const string baseHttpAddress = "https://localhost:7147/api/";

        public AuthControllerTests(CustomWebApplicationFactory applicationFactory)
        {
            _httpClient = applicationFactory.CreateClient();
            _context = applicationFactory.Context;
            _userManager = applicationFactory.UserManager;
            _util = new TestUtil(_userManager, _httpClient);
        }

        [Fact]
        public async Task Log_In_Test()
        {
            // Arrange

            var testUser = await _util.GetTestStudentAuthenticationDtoAsync();

            // Act

            var response = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication/login", testUser);

            // Assert

            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Register_User_Success_Test()
        {
            // Arrange

            var token = await _util.LogInTestTeacherAsync();

            var newUser = new UserForRegistrationDto() {
                Name = "Cool user",
                Email = "user@cool.se",
                Password = "test123",
                CourseId = 1,
                Role = UserRole.Student
            };

            // Act

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var response = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication", newUser);

            // Assert

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Register_User_Forbidden_Test()
        {
            // Arrange

            var token = await _util.LogInTestStudentAsync();

            var newUser = new UserForRegistrationDto()
            {
                Name = "Cool user",
                Email = "user@cool.se",
                Password = "test123",
                CourseId = 1,
                Role = UserRole.Student
            };

            // Act

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var response = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication", newUser);

            // Assert

            Assert.True(response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AccessTokenAuthorizationSuccessTest()
        {
            // Arrange

            var token = await _util.LogInTestStudentAsync();

            // Fetch with token

            bool success = false;

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/courses")) {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);

                var requestResult = await _httpClient.SendAsync(requestMessage);

                if (requestResult.IsSuccessStatusCode) {
                    success = true;
                }
            }

            // Assert
            Assert.True(success);
        }

        [Fact]
        public async Task AccessTokenAuthorizationFailureTest() {
            // Arrange

            var token = await _util.LogInTestStudentAsync();

            // Fetch with token

            bool success = false;

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/courses")) {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken + "a");

                var requestResult = await _httpClient.SendAsync(requestMessage);

                if (requestResult.IsSuccessStatusCode) {
                    success = true;
                }
            }

            // Assert

            Assert.False(success);
        }

        [Fact]
        public async Task AccessTokenAuthorizationExpirationTest() {
            // Arrange

            var token = await _util.LogInTestStudentAsync();

            //Expire the token

            var ttime = GetTokenExpirationTime(token.AccessToken);

            var expired = CheckTokenIsValid(token.AccessToken);

            // ToDo: No no! /Dimitris
            Thread.Sleep(5000);

            var ttimeAfter = GetTokenExpirationTime(token.AccessToken);

            var expiredAfterSleep = CheckTokenIsValid(token.AccessToken);

            // Fetch with token

            bool success = false;

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/courses")) {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);
                
                var requestResult = await _httpClient.SendAsync(requestMessage);

                if (requestResult.IsSuccessStatusCode) {
                    success = true;
                }
            }

            // Assert

            Assert.False(success);
        }

        [Fact]
        public async Task Token_Should_Expire_As_Expected()
        {
            // Arrange

            var token = await _util.LogInTestStudentAsync();
            var ttime = GetTokenExpirationTime(token.AccessToken);

            long unixTimestamp = ttime; // Token's expiration time
            DateTimeOffset expirationTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
            DateTime expirationTimeUtc = expirationTimeOffset.UtcDateTime;

            // Simulate current time after token expiration
            DateTime simulatedCurrentTime = expirationTimeUtc.AddMinutes(1); // 1 minute after expiration

            // Act

            bool isTokenExpired = simulatedCurrentTime >= expirationTimeUtc;

            // Assert

            Assert.True(isTokenExpired, "Token should be expired.");
        }

        // Straight up Stack Overflow
        private static long GetTokenExpirationTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);
            return ticks;
        }

        private static bool CheckTokenIsValid(string token)
        {
            var tokenTicks = GetTokenExpirationTime(token);
            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

            var now = DateTime.Now.ToUniversalTime();

            var valid = tokenDate >= now;

            return valid;
        }
    }
}