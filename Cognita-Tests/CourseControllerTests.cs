using Cognita.API.Services;
using Cognita_API;
using Cognita_API.Controllers;
using Cognita_API.Infrastructure.Data;
using IntegrationTests;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Cognita_Tests
{
    public class CourseControllerTests
    : IClassFixture<CustomWebApplicationFactory>
    {
        private HttpClient _httpClient;
        private CognitaDbContext _context;

        const string baseHttpAddress = "https://localhost:7147/api/";

        public CourseControllerTests(CustomWebApplicationFactory applicationFactory) {
            //applicationFactory.ClientOptions.BaseAddress = new Uri("https://localhost:5000/api/");
            _httpClient = applicationFactory.CreateClient();
            _context = applicationFactory.Context;
        }

        [Fact]
        public async Task GetAllTest() {
            // Arrange

            // Act
            var response = await _httpClient.GetAsync("api/courses");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }
    }
}