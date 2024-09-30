using Cognita_Shared.Dtos.Module;
using System;

namespace Cognita_Shared.Dtos.Course;

public class CourseWithDetailsDto
{
    public int CourseId { get; set; }
    public required string CourseName { get; set; }
    public required string Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public ICollection<ModuleDto>? Modules { get; set; }
}
