using System;
using Cognita_API.Infrastructure.Data;
using Cognita_Domain.Contracts;
using Cognita_Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cognita_Domain.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(CognitaDbContext context)
        : base(context) { }

    public async Task CreateUserAsync(User user) => await CreateAsync(user);

    public async Task<IEnumerable<User>> GetAllUsersAsync() =>
        await GetAll()
            .Include(c => c.Course)
            .ToListAsync();

    public async Task<IEnumerable<User>> GetUsersInCourseAsync(int courseId) =>
        await GetByCondition(u => u.CourseId == courseId).ToListAsync();

    public async Task<User?> GetSingleUserAsync(int id, bool trackChanges = false) =>
        trackChanges
            ? await GetByCondition(u => u.UserId == id).FirstOrDefaultAsync()
            : await GetByCondition(u => u.UserId == id).AsNoTracking().FirstOrDefaultAsync();
}
