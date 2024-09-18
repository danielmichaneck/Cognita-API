using System;

namespace Cognita_Domain.Contracts;

public interface IUoW
{
    ICourseRepository courseRepository { get; }
}
