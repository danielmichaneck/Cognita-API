using Cognita.API.Service.Contracts;

namespace Cognita.API.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAuthService> _authService;
    public IAuthService AuthService => _authService.Value;

    public ServiceManager(Lazy<IAuthService> authService)
    {
        _authService = authService;
    }
}
