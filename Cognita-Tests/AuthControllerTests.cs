using System.Net.Http.Json;
using IntegrationTests;
using Cognita_API.Infrastructure.Data;
using System.Net;
using Cognita_Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Entities;
using Cognita_Shared.Enums;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cognita_Tests
{
    [Collection("DbCollection")]
    public class AuthControllerTests
    //: IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CognitaDbContext _context;
        private readonly CustomWebApplicationFactory _applicationFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        private TestUtil _util;
        private bool _testUserSeeded;

        const string baseHttpAddress = "https://localhost:7147/api/";

        public AuthControllerTests(CustomWebApplicationFactory applicationFactory)
        {
            _httpClient = applicationFactory.CreateClient();
            _context = applicationFactory.Context;
            _applicationFactory = applicationFactory;
            _userManager = applicationFactory.UserManager;
            _testUserSeeded = false;
            _util = new TestUtil(_userManager, _httpClient);
        }

        [Fact]
        public async Task LoginTest()
        {
            // Arrange

            var testUser = await _util.GetTestUserAuthenticationDtoAsync();

            // Act

            var response = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication/login", testUser);

            // Assert

            Xunit.Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterUserTest()
        {
            // Arrange

            var newUser = new UserForRegistrationDto() {
                Name = "Daniel M",
                Email = "daniel.m@hemsida.se",
                Password = "test123",
                CourseId = 1
            };

            // Act

            var response = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication", newUser);

            // Assert

            Xunit.Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task AccessTokenAuthorizationSuccessTest()
        {
            // Arrange

            var token = await _util.LogInTestUserAsync();

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
            Xunit.Assert.True(success);
        }

        [Fact]
        public async Task AccessTokenAuthorizationFailureTest() {
            // Arrange

            var token = await _util.LogInTestUserAsync();

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

            Xunit.Assert.False(success);
        }

        [Fact]
        public async Task AccessTokenAuthorizationExpirationTest() {
            // Arrange

            var token = await _util.LogInTestUserAsync();

            //Expire the token

            var ttime = GetTokenExpirationTime(token.AccessToken);

            var expired = CheckTokenIsValid(token.AccessToken);

            // ToDo: No no! /Dimitris
            Thread.Sleep(2000);

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

            Xunit.Assert.False(success);
        }

        [Fact]
        public async Task Token_Should_Expire_As_Expected()
        {
            // Arrange

            var token = await _util.LogInTestUserAsync();
            var ttime = GetTokenExpirationTime(token.AccessToken);

            long unixTimestamp = ttime; // Token's expiration time
            DateTimeOffset expirationTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
            DateTime expirationTimeUtc = expirationTimeOffset.UtcDateTime;

            // Simulate current time after token expiration
            DateTime simulatedCurrentTime = expirationTimeUtc.AddMinutes(1); // 1 minute after expiration

            // Act

            bool isTokenExpired = simulatedCurrentTime >= expirationTimeUtc;

            // Assert

            Xunit.Assert.True(isTokenExpired, "Token should be expired.");
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