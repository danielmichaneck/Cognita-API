using Microsoft.AspNetCore.Mvc.Testing;
using Cognita.API.Services;
using Cognita_API.Controllers;
using Cognita_Service;
using Cognita_API;
using System.Security.Policy;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using IntegrationTests;
using Cognita_API.Infrastructure.Data;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Net;
using Cognita_Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Cognita_Infrastructure.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Cognita_Shared.Entities;
using Cognita_Shared.Enums;

namespace Cognita_Tests
{
    // ...

    public class AuthControllerTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private HttpClient _httpClient;
        private CognitaDbContext _context;

        const string baseHttpAddress = "https://localhost:7147/api/";
        private readonly CustomWebApplicationFactory applicationFactory;

        public AuthControllerTests(CustomWebApplicationFactory applicationFactory)
        {
            //applicationFactory.ClientOptions.BaseAddress = new Uri("https://localhost:5000/api/");
            _httpClient = applicationFactory.CreateClient();
            _context = applicationFactory.Context;
            this.applicationFactory = applicationFactory;
        }

        [Fact]
        public async Task LoginTest()
        {
            // Arrange
            using var scope = applicationFactory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

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

            // Act
            var result = await userManager.CreateAsync(user, "password123");

            // Act
            var obj = new UserForAuthenticationDto
            {
                UserName = "Kalle",
                Password = "password123",
            };
            var response = await _httpClient.PostAsJsonAsync(baseHttpAddress + "authentication/login", obj);

            // Assert
            //response.EnsureSuccessStatusCode(); // Status Code 200-299
            //Assert.Equal("text/html; charset=utf-8",
            //    response.Content.Headers.ContentType.ToString());

            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateUserTest()
        {

            // Arrange
            using var scope = applicationFactory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser
            {
                UserName = "Kalle2",
                Email = "kalle2@example.com",
                User = new User
                {
                    Name = "Kalle2",
                    Role = UserRole.Student,
                    CourseId = 1
                }
            };

            // Act
            var result = await userManager.CreateAsync(user, "password123");

            // Assert
            Assert.True(result.Succeeded);
            var createdUser = await userManager.FindByNameAsync("Kalle2");
            Assert.NotNull(createdUser);
        }
    }
}