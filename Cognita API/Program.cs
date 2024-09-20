using Cognita.API.Extensions;
using Cognita_API.Infrastructure.Data;
using Cognita_Infrastructure.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cognita_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.ConfigureJwt(builder.Configuration);
            builder.Services.ConfigureCors();
            builder.Services.AddAutoMapper(typeof(Cognita_Service.AssemblyReference).Assembly);
            builder.Services.ConfigureRepositories();
            builder.Services.ConfigureServices();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<CognitaDbContext>(Options =>
                Options.UseSqlite(builder.Configuration.GetConnectionString("Cognita_APIContext"))
            );

            builder
                .Services.AddIdentityCore<ApplicationUser>(opt =>
                {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequiredLength = 3;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<CognitaDbContext>()
                .AddDefaultTokenProviders();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                await app.SeedDataAsync();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
