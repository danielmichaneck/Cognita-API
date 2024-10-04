﻿using Cognita_Infrastructure.Models.Dtos;
using Cognita_Shared.Dtos.Course;
using Cognita_Shared.Dtos.Document;
using Cognita_Shared.Dtos.User;
using Microsoft.AspNetCore.Identity;

namespace Cognita.API.Service.Contracts;

public interface IAuthService
{
    Task<TokenDto> CreateTokenAsync(bool expires = true);
    Task<TokenDto> RefreshTokenAsync(TokenDto token);
    Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistration);
    Task<bool> ValidateUserAsync(UserForAuthenticationDto user);
    public Task<UserDto?> GetUserAsync(int id);
    public Task<IEnumerable<UserDto>> GetUsersAsync(int? courseId = null);
    public Task<bool> UpdateUser(int id, UserForUpdateDto dto);
    public Task<IEnumerable<CourseDto>> GetCoursesForUserAsync(int userId);
    public Task<bool> AddDocumentToUser(int id, DocumentDto documentDto);
}
