using System;

namespace Cognita_Shared.Dtos.User;

public class UserForUpdateDto
{
    public required string Name { get; set; }
    public int CourseId { get; set; }
    //TODO Email?
}
