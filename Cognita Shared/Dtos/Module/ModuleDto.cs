using System;

namespace Cognita_Shared.Dtos.Module;

public class ModuleDto
{
    public int Id { get; set; }
    public required string ModuleName { get; set; }
    public required string Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
