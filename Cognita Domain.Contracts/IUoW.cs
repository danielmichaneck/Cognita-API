using System;

namespace Cognita_Domain.Contracts;

public interface IUoW
{
    ICourseRepository CourseRepository { get; }
    IModuleRepository ModuleRepository { get; }

    Task CompleteAsync();
}
