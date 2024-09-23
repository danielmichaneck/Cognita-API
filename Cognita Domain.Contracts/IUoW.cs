using System;

namespace Cognita_Domain.Contracts;

public interface IUoW
{
    ICourseRepository CourseRepository { get; }
    IModuleRepository ModuleRepository { get; }
    IUserRepository UserRepository { get; }

    Task CompleteAsync();
}
