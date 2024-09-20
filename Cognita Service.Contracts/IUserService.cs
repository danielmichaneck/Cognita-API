using System;
using Cognita_Shared.Dtos.User;
using Cognita_Shared.Entities;

namespace Cognita_Service.Contracts;

public interface IUserService
{
    Task<UserDto> AddUserAsync(UserForCreationDto dto);
    Task<bool> UpdateUserAsync(int userId, UserForUpdateDto dto);
    Task<IEnumerable<UserDto>> GetParticipantsAsync(int courseId);
    Task<UserDto> GetSingleUserAsync(int userId);
}