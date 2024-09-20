using Cognita_Service.Contracts;

namespace Cognita.API.Service.Contracts;

public interface IServiceManager
{
    IAuthService AuthService { get; }

    ICourseService CourseService { get; }

    IModuleService ModuleService { get; }

    IUserService UserService { get; }
}
