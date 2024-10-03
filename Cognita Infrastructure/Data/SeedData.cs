using Bogus;
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
        private static Faker _faker = new Faker("sv");
        private static Random _random = new Random();

        private static UserManager<ApplicationUser> _userManager = null!;
        private static RoleManager<IdentityRole<int>> _roleManager = null!;
        private static IConfiguration _configuration = null!;
        private const string ADMIN_ROLE = "Admin";
        private const string USER_ROLE = "User";

        public static async Task InitAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<CognitaDbContext>();
            
            if (await context.Course.AnyAsync())
                return;

            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();


            string[] activityTypeArray = ["LECTURE", "ASSIGMENT", "ELEARNING"];
            var activityTypes = GenerateActivityTypes(activityTypeArray);
            await context.AddRangeAsync(activityTypes);

            var activities = GenerateActivities(100, activityTypes);
            await context.AddRangeAsync(activities);

            var modules = GenerateModules(50, 2, activities);
            await context.AddRangeAsync(modules);

            var courses = GenerateCourses(25, 2, modules);
            await context.AddRangeAsync(courses);

            //await CreateRolesAsync(new[] { ADMIN_ROLE, USER_ROLE });
            await GenerateUsersAsync(100, courses);

            await context.SaveChangesAsync();

            //Null check on services!
            //await db.Database.MigrateAsync();


            /*try {
                await CreateRolesAsync(new[] { ADMIN_ROLE, employeeRole });
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
                var randomDate = _faker.Date.Future(1, DateTime.Now);
                var randomDayIncrement = _random.Next(1, 10);
                var randomHourIncrement = _random.Next(1, 15);

                var name = _faker.Lorem.Word();
                var description = _faker.Lorem.Paragraph(2);
                var startDate = randomDate;
                var endDate = randomDate.Add(
                    new TimeSpan(randomDayIncrement, randomHourIncrement, 0, 0)
                );
                var type = _faker.PickRandom(activityTypes);

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

                var randomDate = _faker.Date.Future(1, DateTime.Now);
                var randomDayIncrement = _random.Next(12, 20);

                var name = _faker.Lorem.Word();
                var description = _faker.Lorem.Paragraph(2);
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

                var randomDate = _faker.Date.Future(1, DateTime.Now);
                var randomDayIncrement = _random.Next(25, 60);

                var name = _faker.Lorem.Word();
                var description = _faker.Lorem.Paragraph(2);
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
                if (await _roleManager.RoleExistsAsync(roleName)) continue;
                var Role = new IdentityRole<int> { Name = roleName };
                var result = await _roleManager.CreateAsync(Role);

                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static async Task GenerateUsersAsync(int numberOfUsers, IEnumerable<Course> courses) {
            var faker = new Faker<ApplicationUser>("sv").Rules((f, e) => {
                e.Email = f.Person.Email;
                e.UserName = e.Email;
                e.Name = f.Person.FullName;
            });

            var users = faker.Generate(numberOfUsers);

            foreach(ApplicationUser user in users) {
                int randomCourse = _random.Next(1, courses.Count());
                user.Courses = [courses.ElementAt(randomCourse)];
            }

            double numberOfTeachers = Math.Ceiling((double)users.Count / 5);

            var passWord = _configuration["password"];

            if (string.IsNullOrEmpty(passWord))
                throw new Exception("password not exist in config");

            int i = 0;

            foreach (var user in users) {
                var result = await _userManager.CreateAsync(user, passWord);
                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));

                if (i < numberOfTeachers) {
                    await _userManager.AddToRoleAsync(user, ADMIN_ROLE);
                } else {
                    await _userManager.AddToRoleAsync(user, USER_ROLE);
                }
                i++;
            }
        }
    }
}
