using Cognita_Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Cognita.API.Service.Contracts;

public interface IAuthService
{
    Task<TokenDto> CreateTokenAsync(bool expires = true);
    Task<TokenDto> RefreshTokenAsync(TokenDto token);
    Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistration);
    Task<bool> ValidateUserAsync(UserForAuthenticationDto user);
}
