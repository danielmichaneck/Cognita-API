using Bogus;
using Cognita_API.Infrastructure.Data;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Entities;
using Cognita_Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cognita_Infrastructure.Data
{
    /*
     drop-database & update-database to clear out the database for fresh seeding.
     */
    public class SeedData
    {
        private static Faker faker = new Faker("sv");
        private static Random random = new Random();

        private static UserManager<ApplicationUser> userManager = null!;
        private static RoleManager<IdentityRole> roleManager = null!;
        private static IConfiguration configuration = null!;
        private const string adminRole = "Admin";
        private const string userRole = "User";

        public static async Task InitAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<CognitaDbContext>();
            
            if (await context.Course.AnyAsync())
                return;

            userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            configuration = serviceProvider.GetRequiredService<IConfiguration>();


            string[] activityTypeArray = ["LECTURE", "ASSIGMENT", "ELEARNING"];
            var activityTypes = GenerateActivityTypes(activityTypeArray);
            await context.AddRangeAsync(activityTypes);

            var activities = GenerateActivities(100, activityTypes);
            await context.AddRangeAsync(activities);

            var modules = GenerateModules(50, 2, activities);
            await context.AddRangeAsync(modules);

            var courses = GenerateCourses(25, 2, modules);
            await context.AddRangeAsync(courses);

            await context.SaveChangesAsync();

            await CreateRolesAsync(new[] { adminRole, userRole });
            await GenerateUsersAsync(100, courses);

            //Null check on services!
            //await db.Database.MigrateAsync();


            /*try {
                await CreateRolesAsync(new[] { adminRole, employeeRole });
                var companies = GenerateCourses(4);
                await db.AddRangeAsync(companies);
                await GenerateUsersAsync(30, companies.ToList());
                await db.SaveChangesAsync();
            } catch (Exception ex) {

                throw;
            }*/
        }

        private static IEnumerable<ActivityType> GenerateActivityTypes(string[] activityTypeArray)
        {
            List<ActivityType> activityTypes = new List<ActivityType>();
            foreach (string activityType in activityTypeArray)
            {
                activityTypes.Add(new ActivityType { Title = activityType });
                ;
            }
            return activityTypes;
        }

        private static IEnumerable<Activity> GenerateActivities(
            int nrOfActivities,
            IEnumerable<ActivityType> activityTypes
        )
        {
            var activities = new List<Activity>(nrOfActivities);

            for (int i = 0; i < nrOfActivities; i++)
            {
                var randomDate = faker.Date.Future(1, DateTime.Now);
                var randomDayIncrement = random.Next(1, 10);
                var randomHourIncrement = random.Next(1, 15);

                var name = faker.Lorem.Word();
                var description = faker.Lorem.Paragraph(2);
                var startDate = randomDate;
                var endDate = randomDate.Add(
                    new TimeSpan(randomDayIncrement, randomHourIncrement, 0, 0)
                );
                var type = faker.PickRandom(activityTypes);

                var activity = new Activity()
                {
                    ActivityName = name,
                    Description = description,
                    StartDate = startDate,
                    EndDate = endDate,
                    ActivityType = type
                };

                activities.Add(activity);
            }
            return activities;
        }

        private static IEnumerable<Module> GenerateModules(
            int nrOfModules,
            int activitiesPerModule,
            IEnumerable<Activity> activities
        )
        {
            var modules = new List<Module>(nrOfModules);
            var activityArray = activities.ToArray();
            int activityIndex = 0;

            for (int i = 0; i < nrOfModules; i++)
            {
                var slicedPosts = activityArray.Skip(activityIndex).Take(2);

                var randomDate = faker.Date.Future(1, DateTime.Now);
                var randomDayIncrement = random.Next(12, 20);

                var name = faker.Lorem.Word();
                var description = faker.Lorem.Paragraph(2);
                var startDate = DateOnly.FromDateTime(randomDate);
                var endDate = DateOnly.FromDateTime(
                    randomDate.Add(new TimeSpan(randomDayIncrement, 0, 0, 0))
                );

                var module = new Module()
                {
                    ModuleName = name,
                    Description = description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Activities = slicedPosts.ToList()
                };

                modules.Add(module);
                activityIndex += activitiesPerModule;
            }
            return modules;
        }

        private static IEnumerable<Course> GenerateCourses(
            int nrOfCourses,
            int modulesPerCourse,
            IEnumerable<Module> modules
        )
        {
            var courses = new List<Course>(nrOfCourses);
            var moduleArray = modules.ToArray();
            int moduleIndex = 0;

            for (int i = 0; i < nrOfCourses; i++)
            {
                var slicedPosts = moduleArray.Skip(moduleIndex).Take(2);

                var randomDate = faker.Date.Future(1, DateTime.Now);
                var randomDayIncrement = random.Next(25, 60);

                var name = faker.Lorem.Word();
                var description = faker.Lorem.Paragraph(2);
                var startDate = DateOnly.FromDateTime(randomDate);
                var endDate = DateOnly.FromDateTime(
                    randomDate.Add(new TimeSpan(randomDayIncrement, 0, 0, 0))
                );

                var course = new Course()
                {
                    CourseName = name,
                    Description = description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Modules = slicedPosts.ToList()
                };

                courses.Add(course);
                moduleIndex += modulesPerCourse;
            }
            return courses;
        }

        private static async Task CreateRolesAsync(string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;
                var Role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(Role);

                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static async Task GenerateUsersAsync(int numberOfUsers, IEnumerable<Course> courses) {
            var faker = new Faker<ApplicationUser>("sv").Rules((f, e) => {
                e.Email = f.Person.Email;
                e.UserName = f.Person.UserName;
                e.User = new User()
                {
                    Name = f.Person.Email,
                    Role = UserRole.Student,
                    Course = f.PickRandom(courses)
                };
            });

            var users = faker.Generate(numberOfUsers);

            var numberOfTeachers = Math.Ceiling((double)users.Count / 5);

            for (int i = 0; i < numberOfTeachers; i++) {
                users[i].User.Role = UserRole.Teacher;
            }

            var passWord = configuration["password"];
            if (string.IsNullOrEmpty(passWord))
                throw new Exception("password not exist in config");

            foreach (var user in users) {
                var result = await userManager.CreateAsync(user, passWord);
                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));

                if (user.User.Role == UserRole.Teacher) {
                    await userManager.AddToRoleAsync(user, adminRole);
                } else {
                    await userManager.AddToRoleAsync(user, userRole);
                }
            }
        }
    }
}
