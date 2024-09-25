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
        //base.ConfigureWebHost(builder);

        

        var userManager = base.Services.GetRequiredService<UserManager<ApplicationUser>>();
        //var userManager = new UserManager<ApplicationUser>();

        builder.ConfigureServices(async services => {
            var dbContextOptions = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CognitaDbContext>));

            if (dbContextOptions != null)
                services.Remove(dbContextOptions);

            services.AddDbContext<CognitaDbContext>(options => {
                options.UseInMemoryDatabase("T2");
            });

            var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CognitaDbContext>();

            context.Course.AddRange([
                                    new Course() {
                                        Description = "This is a test course",
                                        CourseName = "Test course 1"
                                    }
                               ]);

            //context.Users.AddRange(
            //                   [
            //                        new ApplicationUser() {
            //                            Email = "urbanek@email.com",
            //                            UserName = "Urban Ek",
            //                             User = new User() {
            //                                 Role = UserRole.Teacher,
            //                                 Name = "Urban Ek",
            //                                 CourseId = 1
            //                             }
            //                        }
            //                   ]);

            //await userManager.CreateAsync(
            //    new ApplicationUser() {
            //        Email = "urbanek@email.com",
            //        UserName = "Urban Ek",
            //        User = new User() {
            //            Role = UserRole.Teacher,
            //            Name = "Urban Ek",
            //            CourseId = 1
            //        }
            //    },
            //    "password123");

            context.SaveChanges();
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