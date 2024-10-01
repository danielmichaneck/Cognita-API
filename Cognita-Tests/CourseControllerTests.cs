using Cognita_API.Infrastructure.Data;
using Cognita_Infrastructure.Models.Dtos;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Dtos.Course;
using IntegrationTests;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Cognita_Tests
{
    //[DoNotParallelize]
    [Collection("DbCollection")]
    public class CourseControllerTests
    {
        private readonly HttpClient _httpClient;
        private readonly CognitaDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TestUtil _util;

        const string baseHttpAddress = "https://localhost:7147/api/";

        public CourseControllerTests(CustomWebApplicationFactory applicationFactory) {
            _httpClient = applicationFactory.CreateClient();
            _context = applicationFactory.Context;
            _userManager = applicationFactory.UserManager;
            _util = new TestUtil(_userManager, _httpClient);
        }

        [Fact]
        public async Task Get_All_Courses_Test_Success() {
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

            Assert.True(success);
        }

        [Fact]
        public async Task Get_All_Courses_Fail_Unautherized_Test()
        {
            // Act
            var response = await _httpClient.GetAsync("api/courses");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Course_Success_Test()
        {
            // Act
            var response = await _httpClient.GetAsync("api/courses/1");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_Course_With_Details_Success_Test()
        {
            // Act
            var response = await _httpClient.GetAsync("api/courses/1");

            var courseAsJsonString = await response.Content.ReadAsStringAsync();

            var course = JsonConvert.DeserializeObject<CourseWithDetailsDto>(courseAsJsonString);

            // Assert
            Assert.True(course is CourseWithDetailsDto);

            var activityType = course.Modules
                .FirstOrDefault().Activities
                .FirstOrDefault().ActivityType;

            Assert.True(activityType is not null);
        }

        [Fact]
        public async Task Create_Course_Success_Test()
        {
            // Arrange

            var newCourse = new CourseForCreationDto()
            {
                CourseName = "Test-course-1",
                Description = "This is a test course generated in the Create_Course_Success_Test",
                StartDate = DateOnly.MinValue,
                EndDate = DateOnly.MaxValue
            };

            // Act

            var response = await _httpClient.PostAsJsonAsync("api/courses", newCourse);

            // Assert

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Edit_Course_Success_Test()
        {
            // Arrange

            var newCourse = new CourseForUpdateDto()
            {
                CourseName = "Test-course-1",
                Description = "This is a test course generated in the Create_Course_Success_Test",
                StartDate = DateOnly.MinValue,
                EndDate = DateOnly.MaxValue
            };

            // Act

            var response = await _httpClient.PutAsJsonAsync("api/courses/1", newCourse);

            // Assert

            Assert.True(response.IsSuccessStatusCode);
        }
    }
}