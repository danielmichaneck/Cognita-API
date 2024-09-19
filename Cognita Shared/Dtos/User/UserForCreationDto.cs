using System;

namespace Cognita_Shared.Dtos.User;

public class UserForCreationDto
{
    public required string Name { get; set; }
    public int CourseId { get; set; }

    //TODO Email?
}
