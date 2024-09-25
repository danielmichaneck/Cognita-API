using Bogus.Extensions.UnitedKingdom;
using Cognita_API;
using Cognita_API.Infrastructure.Data;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Entities;
using Cognita_Shared.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public CognitaDbContext Context { get; private set; }
  

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async services =>
        {
            var dbContextOptions = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CognitaDbContext>));

            if (dbContextOptions != null)
            {
                services.Remove(dbContextOptions);
            }

            services.AddDbContext<CognitaDbContext>(options =>
            {
                options.UseInMemoryDatabase("T2");
            });

            var serviceprovider = services.BuildServiceProvider();

            var scope = serviceprovider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<CognitaDbContext>();
            var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed the database with test data
            context.Course.AddRange(new Course
            {
                Description = "This is a test course",
                CourseName = "Test course 1"
            });

            await userManager.CreateAsync(new ApplicationUser
            {
                Email = "urbanek@email.com",
                UserName = "Urban Ek",
                User = new User
                {
                    Role = UserRole.Teacher,
                    Name = "Urban Ek",
                    CourseId = 1
                }
            }, "Password123!");

            context.SaveChanges();

            // Set properties for test access
            Context = context;
            
        });
    }

    public override ValueTask DisposeAsync()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
        return base.DisposeAsync();
    }
}
