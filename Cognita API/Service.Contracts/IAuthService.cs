using Cognita.API.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Cognita.API.Service.Contracts;

public interface IAuthService
{
    Task<TokenDto> CreateTokenAsync(bool expireTime);
    Task<TokenDto> RefreshTokenAsync(TokenDto token);
    Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistration);
    Task<bool> ValidateUserAsync(UserForAuthenticationDto user);
}
