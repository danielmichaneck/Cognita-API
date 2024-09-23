using System;
using Cognita_Shared.Entities;

namespace Cognita_Domain.Contracts;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Task<Course?> GetSingleCourseAsync(int id, bool trackChanges = false);
    Task CreateCourseAsync(Course course);

    //TODO Delete?
    // Task<bool> EditCourseAsync(int id, Course course);
}
