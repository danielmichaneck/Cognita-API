using System;
using Cognita_Service.Contracts;
using Cognita_Shared.Dtos;

namespace Cognita_Service;

public class CourseService : ICourseService
{
    public CourseDto CreateCourse(CourseForCreationDto dto)
    {
        throw new NotImplementedException();
    }

    public bool EditCourse(int id, CourseForUpdateDto dto)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CourseDto> GetCourses()
    {
        throw new NotImplementedException();
    }

    public CourseDto GetSingleCourse()
    {
        throw new NotImplementedException();
    }
}
