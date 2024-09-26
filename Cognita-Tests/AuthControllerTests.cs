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

namespace Cognita_Tests
{
    public class AuthControllerTests
    : IClassFixture<CustomWebApplicationFactory>
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
            //applicationFactory.ClientOptions.BaseAddress = new Uri("https://localhost:5000/api/");
            _httpClient = applicationFactory.CreateClient();
            _context = applicationFactory.Context;
            _applicationFactory = applicationFactory;
            _userManager = applicationFactory.UserManager;
            _testUserSeeded = false;
            _util = new TestUtil(_userManager);
        }

        [Fact]
        public async Task LoginTest()
        {
            // Arrage
            //await SeedTestUser();
            await _util.SeedTestUserAsync();
            var testUser = _util.GetTestUserAuthenticationDto();

            // Act

            var response = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication/login", testUser);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
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
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task AccessTokenAuthorizationSuccessTest()
        {
            // Arrange
            await SeedTestUser();

            var obj = new UserForAuthenticationDto {
                UserName = "Kalle",
                Password = "password123",
            };

            // Act

            // Create and convert token
            var baseResponse = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication/login", obj);
            var jsonResponse = await baseResponse.Content.ReadAsStringAsync();
            TokenDto convertedToken = JsonConvert.DeserializeObject<TokenDto>(jsonResponse);

            // Fetch with token

            bool success = false;

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/courses")) {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", convertedToken.AccessToken);

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
            await SeedTestUser();
            var testUser = _util.GetTestUserAuthenticationDto();

            // Act

            // Create and convert token
            var baseResponse = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication/login", testUser);
            var jsonResponse = await baseResponse.Content.ReadAsStringAsync();
            TokenDto convertedToken = JsonConvert.DeserializeObject<TokenDto>(jsonResponse);

            // Fetch with token

            bool success = false;

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/courses")) {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", convertedToken.AccessToken);

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
            await SeedTestUser();

            var obj = new UserForAuthenticationDto {
                UserName = "Kalle",
                Password = "password123",
            };

            // Act

            // Create and convert token
            var baseResponse = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication/login", obj);
            var jsonResponse = await baseResponse.Content.ReadAsStringAsync();
            TokenDto convertedToken = JsonConvert.DeserializeObject<TokenDto>(jsonResponse);


            //Expire the token

            //var stuff = await _userManager.FindByNameAsync("Kalle");

            var ttime = GetTokenExpirationTime(convertedToken.AccessToken);

            var expired = CheckTokenIsValid(convertedToken.AccessToken);

            Thread.Sleep(2000);

            var ttimeAfter = GetTokenExpirationTime(convertedToken.AccessToken);

            var expiredAfterSleep = CheckTokenIsValid(convertedToken.AccessToken);




            // Fetch with token

            bool success = false;

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/courses")) {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", convertedToken!.AccessToken);
                
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
            await SeedTestUser();

            var obj = new UserForAuthenticationDto
            {
                UserName = "Kalle",
                Password = "password123",
            };

            // Act

            // Create and convert token
            var baseResponse = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication/login", obj);
            var jsonResponse = await baseResponse.Content.ReadAsStringAsync();
            TokenDto convertedToken = JsonConvert.DeserializeObject<TokenDto>(jsonResponse);


            //Expire the token

            //var stuff = await _userManager.FindByNameAsync("Kalle");

            var ttime = GetTokenExpirationTime(convertedToken.AccessToken);

            // Arrange
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

        private async Task SeedTestUser() {
            if (_testUserSeeded) return;
            var user = new ApplicationUser
            {
                UserName = "Kalle",
                Email = "kalle@example.com",
                User = new User
                {
                    Name = "Kalle",
                    Role = UserRole.Student,
                    CourseId = 1
                }
            };

            _testUserSeeded = true;

            await _userManager.CreateAsync(user, "password123");
        }

        public static long GetTokenExpirationTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);
            return ticks;
        }

        public static bool CheckTokenIsValid(string token)
        {
            var tokenTicks = GetTokenExpirationTime(token);
            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

            var now = DateTime.Now.ToUniversalTime();

            var valid = tokenDate >= now;

            return valid;
        }
    }
}