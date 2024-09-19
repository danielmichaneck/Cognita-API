using System;
using Cognita_Shared.Dtos.Course;

namespace Cognita_Service.Contracts;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetCoursesAsync();

    Task<CourseDto> GetSingleCourseAsync(int id);

    Task<bool> EditCourseAsync(int id, CourseForUpdateDto dto);

    Task<CourseDto> CreateCourseAsync(CourseForCreationDto dto);
}
