using System.Text;
using Cognita.API.Service.Contracts;
using Cognita.API.Services;
using Cognita_Domain.Contracts;
using Cognita_Domain.Repositories;
using Cognita_Infrastructure.Data;
using Cognita_Service;
using Cognita_Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Cognita.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(builder =>
        {
            builder.AddPolicy(
                "AllowAll",
                p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );
        });
    }

    public static void ConfigureOpenApi(this IServiceCollection services) =>
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(setup =>
            {
                setup.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Place to add JWT with Bearer",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    }
                );

                setup.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>()
                        }
                    }
                );
            });

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IServiceManager, ServiceManager>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped(provider => new Lazy<IAuthService>(
            () => provider.GetRequiredService<IAuthService>()
        ));
        services.AddScoped(provider => new Lazy<ICourseService>(
            () => provider.GetRequiredService<ICourseService>()
        ));
        services.AddScoped(provider => new Lazy<IModuleService>(
            () => provider.GetRequiredService<IModuleService>()
        ));
        services.AddScoped(provider => new Lazy<IUserService>(
            () => provider.GetRequiredService<IUserService>()
        ));
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUoW, UoW>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped(provider => new Lazy<ICourseRepository>(
            () => provider.GetRequiredService<ICourseRepository>()
        ));
        services.AddScoped(provider => new Lazy<IModuleRepository>(
            () => provider.GetRequiredService<IModuleRepository>()
        ));
        services.AddScoped(provider => new Lazy<IUserRepository>(
            () => provider.GetRequiredService<IUserRepository>()
        ));
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        //ToDo: Set in "Manage User Secrets" (right click project and you find it!). Example "secretkey" : "ReallyLongKeyItDosentWorkIfItsNotAtleat32Caracters!!!!!!!!!!!!!!!!!!!!!!!!"
        var secretkey = configuration["secretkey"];
        ArgumentNullException.ThrowIfNull(secretkey, nameof(secretkey));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                ArgumentNullException.ThrowIfNull(jwtSettings, nameof(jwtSettings));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
    }

    public static async Task SeedDataAsync(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            //var context = serviceProvider.GetRequiredService<CognitaDbContext>();

            try
            {
                await SeedData.InitAsync(serviceProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }

    public static async Task CreateRolesAsync(this IApplicationBuilder app)
    {
        var roleManager = app
            .ApplicationServices
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<int>>>();

        string[] roleNames = new string[] { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            if (await roleManager.RoleExistsAsync(roleName)) continue;
            var Role = new IdentityRole<int> { Name = roleName };
            var result = await roleManager.CreateAsync(Role);

            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        }
    }
}
