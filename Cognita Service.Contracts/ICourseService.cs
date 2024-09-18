using System;
using Cognita_Shared.Dtos;

namespace Cognita_Service.Contracts;

public interface ICourseService
{
    IEnumerable<CourseDto> GetCourses();

    CourseDto GetSingleCourse();

    bool EditCourse(int id, CourseForUpdateDto dto);

    CourseDto CreateCourse(CourseForCreationDto dto);
}
