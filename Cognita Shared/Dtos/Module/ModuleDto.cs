using Cognita_Shared.Dtos.Activity;
using Cognita_Shared.Entities;
using System;

namespace Cognita_Shared.Dtos.Module;

public class ModuleDto
{
    public int ModuleId { get; set; }
    public required string ModuleName { get; set; }
    public required string Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public ICollection<ActivityDto>? Activities { get; set; }
}
