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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace IntegrationTests;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public CognitaDbContext Context { get; private set; }
    public UserManager<ApplicationUser> UserManager { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async services => {
            var dbContextOptions = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CognitaDbContext>));

            if (dbContextOptions != null)
                services.Remove(dbContextOptions);

            services.AddDbContext<CognitaDbContext>(options => {
                options.UseInMemoryDatabase("T2");
            });

            var serviceprovider = services.BuildServiceProvider();
            var scope = serviceprovider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<CognitaDbContext>();
            var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();

            context.Course.AddRange([
                new Course() {
                    Description = "This is a test course",
                    CourseName = "Test course 1",
                    StartDate = DateOnly.MinValue,
                    EndDate = DateOnly.MaxValue,
                    Modules = new Collection<Module>() {
                        new Module() {
                            Description = "This is a test module",
                            ModuleName = "Test module 1",
                            StartDate = DateOnly.MinValue,
                            EndDate = DateOnly.MaxValue,
                            Activities = new Collection<Activity>() {
                                new Activity() {
                                    Description = "This is a test activity",
                                    ActivityName = "Test activity 1",
                                    StartDate = DateTime.MinValue,
                                    EndDate = DateTime.MaxValue,
                                    ActivityType = new ActivityType() {
                                        Title = "LESSON"
                                    }
                                }
                            }
                        }
                    }
                }
            ]);

            await userManager.CreateAsync(new ApplicationUser
            {
                Email = "urbanek@email.com",
                UserName = "urbanek@email.com",
                User = new User
                {
                    Role = UserRole.Teacher,
                    Name = "Urban Ek",
                    CourseId = 1
                }
            }, "Password123!");

            context.SaveChanges();
            Context = context;
            UserManager = userManager;
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.tests.json");

            config.AddJsonFile(configPath, optional: false, reloadOnChange: true);
        });
    }

    public override ValueTask DisposeAsync()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
        return base.DisposeAsync();
    }
}