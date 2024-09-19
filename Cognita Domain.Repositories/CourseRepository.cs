using System;
using Cognita_API.Infrastructure.Data;
using Cognita_Domain.Contracts;
using Cognita_Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cognita_Domain.Repositories;

public class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    public CourseRepository(CognitaDbContext context)
        : base(context) { }

    public async Task CreateCourseAsync(Course course)
    {
        await CreateAsync(course);
    }

    //TODO Delete?
    // public Task<bool> EditCourseAsync(int id, Course course) { }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync() => await GetAll().ToListAsync();

    public async Task<Course?> GetSingleCourseAsync(int id, bool trackChanges) =>
        await GetByCondition(c => c.CourseId == id, trackChanges).FirstOrDefaultAsync();
}
