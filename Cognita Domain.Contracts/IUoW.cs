using System;

namespace Cognita_Domain.Contracts;

public interface IUoW
{
    ICourseRepository CourseRepository { get; }
    IModuleRepository ModuleRepository { get; }
    IActivityRepository ActivityRepository { get; }
    IUserRepository UserRepository { get; }

    Task CompleteAsync();
}
