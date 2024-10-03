using System;
using Cognita_API.Infrastructure.Data;
using Cognita_Domain.Contracts;
using Cognita_Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cognita_Domain.Repositories;

public class ModuleRepository : RepositoryBase<Module>, IModuleRepository
{
    public ModuleRepository(CognitaDbContext context)
        : base(context) { }

    public async Task CreateModuleAsync(Module module, int courseId)
    {
        var course = await Context
            .Course.Where(c => c.CourseId == courseId)
            .Include(c => c.Modules)
            .FirstOrDefaultAsync(c => c.CourseId == courseId);

        //TODO
        //Throw and catch custom exception
        ArgumentNullException.ThrowIfNull(course);

        course.Modules.Add(module);
    }

    // public Task<bool> EditModuleAsync(int id, Module module) { }

    public async Task<IEnumerable<Module>> GetAllModulesAsync(int courseId) =>
        await GetByCondition(m => m.CourseId == courseId).ToListAsync();

    public async Task<Module?> GetSingleModuleAsync(int id, bool trackChanges = false) =>
        await GetByCondition(m => m.ModuleId == id, trackChanges).Include(m => m.Activities).FirstOrDefaultAsync();
}
