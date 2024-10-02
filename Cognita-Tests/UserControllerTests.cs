using System.Net.Http.Json;
using IntegrationTests;
using Cognita_API.Infrastructure.Data;
using System.Net;
using Cognita_Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Cognita_Infrastructure.Models.Entities;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Cognita_Shared.Dtos.User;
using Cognita_Shared.Dtos.Course;
using Newtonsoft.Json;
using System.Text.Json;

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
        public async Task Get_All_Users_Course_Name_Success_Test()
        {
            // Arrange

            TokenDto token = await _util.LogInTestUserAsync();
            bool success = false;
            IEnumerable<UserDto>? dtos;

            // Act

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/users"))
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);

                var requestResult = await _httpClient.SendAsync(requestMessage);

                if (requestResult.IsSuccessStatusCode)
                {
                    var usersAsJsonString = await requestResult.Content.ReadAsStringAsync();
                    dtos = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(usersAsJsonString);
                    if (dtos is not null && dtos.Any()) {
                        if (dtos.FirstOrDefault().CourseName is not null &&
                             !String.IsNullOrWhiteSpace(dtos.FirstOrDefault().CourseName)) {
                            success = true;
                        }
                    }
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

        [Fact]
        public async Task Update_User_Test_Success()
        {
            // Arrange

            TokenDto token = await _util.LogInTestUserAsync();
            bool success = false;

            var dto = new UserForUpdateDto() {
                Name = "Updated name",
                Email = "Updated email"
            };

            var dtoAsJson = JsonConvert.SerializeObject(dto);

            // Act

            var response = await _httpClient.PutAsJsonAsync("api/users/1", dtoAsJson);

            if (response.IsSuccessStatusCode)
                success = true;

            //using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "api/users/1"))
            //{
            //    requestMessage.Headers.Authorization =
            //        new AuthenticationHeaderValue("Bearer", token.AccessToken);

            //    requestMessage.Content = new StringContent(dtoAsJson);

            //    var requestResult = await _httpClient.SendAsync(requestMessage);

            //    if (requestResult.IsSuccessStatusCode)
            //    {
            //        success = true;
            //    }
            //}

            // Assert

            Assert.True(success);
        }
    }
}