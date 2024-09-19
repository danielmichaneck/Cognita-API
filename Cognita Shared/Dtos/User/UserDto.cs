using System;

namespace Cognita_Shared.Dtos.User;

public class UserDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CourseId { get; set; }

    //TODO Email?
}
