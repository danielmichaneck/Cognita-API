using System;
using Cognita_Shared.Entities;

namespace Cognita_Domain.Contracts;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Task<Course?> GetSingleCourseAsync(int id);
    Task CreateCourseAsync(Course course);
    bool EditCourseAsync(int id, Course course);
}
