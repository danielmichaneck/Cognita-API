using Cognita_Shared.Enums;
using System;

namespace Cognita_Shared.Dtos.User;

public class UserDto
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required UserRole Role { get; set; }
    public string CourseName { get; set; }
    public int CourseId { get; set; }
}
