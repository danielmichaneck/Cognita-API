using Cognita_Infrastructure.Models.Dtos;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Dtos.User;
using Cognita_Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cognita.API.Service.Contracts;

public interface IAuthService
{
    Task<TokenDto> CreateTokenAsync(bool expires = true);
    Task<TokenDto> RefreshTokenAsync(TokenDto token);
    Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistration);
    Task<bool> ValidateUserAsync(UserForAuthenticationDto user);
    public Task<ApplicationUser?> GetUserAsync(int id);
    public Task<IEnumerable<ApplicationUser>> GetUsersAsync(int? courseId = null);
    public Task<bool> UpdateUser(int id, UserForUpdateDto dto);
}
