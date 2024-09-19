using System;

namespace Cognita_Shared.Dtos.Course;

public class CourseForCreationDto
{
    public required string CourseName { get; set; }
    public required string Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
