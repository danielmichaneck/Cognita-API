using Cognita_Infrastructure.Models.Dtos;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Entities;
using Cognita_Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace Cognita_Tests
{
    internal class TestUtil
    {
        private const string STUDENT_SEED_NAME = "Kalle Anka";
        private const string TEACHER_SEED_NAME = "Pelle Svanslos";
        private const string STUDENT_SEED_EMAIL = "kalle@ankeborg.anka";
        private const string TEACHER_SEED_EMAIL = "pelle@svanslos.uppsala";
        private const string STUDENT_SEED_PASSWORD = "KallesPassword123!";
        private const string TEACHER_SEED_PASSWORD = "PellesPassword123!";
        private const string STUDENT_SEED_ROLE = "User";
        private const string TEACHER_SEED_ROLE = "Admin";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;

        private bool _userSeeded = false;
        private Course? _seededCourse;

        public TestUtil(UserManager<ApplicationUser> userManager, HttpClient httpClient) {
            _userManager = userManager;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Seeds test student and teacher based on constants if one does not exist.
        /// </summary>
        /// <returns></returns>
        private async Task SeedTestUsersAsync() {
            if (_userSeeded) return;
            _userSeeded = true;

            var teacherExists = await _userManager.FindByNameAsync(TEACHER_SEED_EMAIL);

            if (teacherExists is null) {
                var course = SeedCourse();

                var teacher = new ApplicationUser {
                    UserName = TEACHER_SEED_EMAIL,
                    Email = TEACHER_SEED_EMAIL,
                    Name = TEACHER_SEED_NAME
                };

                teacher.Courses = [course];

                var createTeacherResult = await _userManager.CreateAsync(teacher, TEACHER_SEED_PASSWORD);
                var roleTeacherResult = await _userManager.AddToRoleAsync(teacher, TEACHER_SEED_ROLE);
            }

            var studentExists = await _userManager.FindByNameAsync(STUDENT_SEED_EMAIL);

            if (studentExists is null) {
                var course = SeedCourse();

                var student = new ApplicationUser {
                    UserName = STUDENT_SEED_EMAIL,
                    Email = STUDENT_SEED_EMAIL,
                    Name = STUDENT_SEED_NAME
                };

                student.Courses = [course];

                var createStudentResult = await _userManager.CreateAsync(student, STUDENT_SEED_PASSWORD);
                var roleStudentResult = await _userManager.AddToRoleAsync(student, STUDENT_SEED_ROLE);
            }
        }

        /// <summary>
        /// Returns a dto for logging in a test student.
        /// </summary>
        /// <returns></returns>
        internal async Task<UserForAuthenticationDto> GetTestStudentAuthenticationDtoAsync() {
            await SeedTestUsersAsync();
            return new UserForAuthenticationDto() {
                UserName = STUDENT_SEED_EMAIL,
                Password = STUDENT_SEED_PASSWORD
            };
        }

        /// <summary>
        /// Returns a dto for logging in a test teacher.
        /// </summary>
        /// <returns></returns>
        internal async Task<UserForAuthenticationDto> GetTestTeacherAuthenticationDtoAsync() {
            await SeedTestUsersAsync();
            return new UserForAuthenticationDto() {
                UserName = TEACHER_SEED_EMAIL,
                Password = TEACHER_SEED_PASSWORD
            };
        }

        /// <summary>
        /// Gets a Token for a logged in test student.
        /// </summary>
        /// <returns>TokenDto</returns>
        internal async Task<TokenDto> LogInTestStudentAsync() {
            return await LogInTestUserAsync(await GetTestStudentAuthenticationDtoAsync());
        }

        /// <summary>
        /// Gets a Token for a logged in test teacher.
        /// </summary>
        /// <returns>TokenDto</returns>
        internal async Task<TokenDto> LogInTestTeacherAsync() {
            return await LogInTestUserAsync(await GetTestTeacherAuthenticationDtoAsync());
        }

        /// <summary>
        /// Gets a Token for a user.
        /// </summary>
        /// <returns>TokenDto</returns>
        private async Task<TokenDto> LogInTestUserAsync(UserForAuthenticationDto authDto) {
            var baseResponse = await _httpClient.PostAsJsonAsync("api/authentication/login", authDto);
            var jsonResponse = await baseResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenDto>(jsonResponse);
        }

        private Course SeedCourse() {
            _seededCourse ??= new Course() {
                Description = "This is a second test course",
                CourseName = "Test course 2",
                StartDate = DateOnly.MinValue,
                EndDate = DateOnly.MaxValue,
                Modules = new Collection<Module>() {
                    new Module() {
                        Description = "This is a second test module",
                        ModuleName = "Test module 2",
                        StartDate = DateOnly.MinValue,
                        EndDate = DateOnly.MaxValue,
                        Activities = new Collection<Activity>() {
                            new Activity() {
                                Description = "This is a second test activity",
                                ActivityName = "Test activity 2",
                                StartDate = DateTime.MinValue,
                                EndDate = DateTime.MaxValue,
                                ActivityType = new ActivityType() {
                                    Title = "ELEARNING"
                                }
                            }
                        }
                    }
                }
            };

            return _seededCourse;
        }
    }
}
