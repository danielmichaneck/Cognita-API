using System;
using Cognita_Shared.Entities;

namespace Cognita_Domain.Contracts;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersInCourseAsync(int courseId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetSingleUserAsync(int id, bool trackChanges = false);
    Task CreateUserAsync(User user);
}
