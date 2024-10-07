using Cognita_Shared.Dtos.Course;
using Cognita_Shared.Entities;

namespace Cognita_Service.Contracts;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetCoursesAsync();
    Task<CourseWithDetailsDto> GetSingleCourseAsync(int id);
    Task<bool> EditCourseAsync(int id, CourseForUpdateDto dto);
    Task<CourseDto> CreateCourseAsync(CourseForCreationDto dto);
    Task<Course?> GetCourse(int courseId);
}
