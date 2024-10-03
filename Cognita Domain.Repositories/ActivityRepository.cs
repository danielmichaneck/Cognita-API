using System;
using Cognita_Domain.Contracts;
using Cognita_Infrastructure.Data;
using Cognita_Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cognita_Domain.Repositories;

public class ActivityRepository : RepositoryBase<Activity>, IActivityRepository {
    public ActivityRepository(CognitaDbContext context)
        : base(context) { }

    public async Task CreateActivityAsync(Activity activity, int courseId, int moduleId) {
        /*var module = await Context
            .Course.Where(c => c.CourseId == courseId).Select<>


            .Include(c => c.Modules)
            .FirstOrDefaultAsync(c => c.CourseId == courseId);*/
       
            

        //TODO
        //Throw and catch custom exception
        //ArgumentNullException.ThrowIfNull(module);

        //module.Activities.Add(activity);
        throw new NotImplementedException();
    }

    public Task<bool> EditActivityAsync(int id, Activity activity) {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Activity>> GetAllActivitiesInModuleAsync(int courseId, int moduleId) {
        throw new NotImplementedException();
    }

    public Task<Activity?> GetSingleActivityAsync(int id, bool trackChanges = false) {
        throw new NotImplementedException();
    }

    // public Task<bool> EditModuleAsync(int id, Module module) { }

    /*public async Task<IEnumerable<Module>> GetAllModulesAsync(int courseId) =>
        await GetByCondition(m => m.CourseId == courseId).ToListAsync();

    public async Task<Module?> GetSingleModuleAsync(int id, bool trackChanges) =>
        await GetByCondition(m => m.ModuleId == id, trackChanges).FirstOrDefaultAsync();*/
}
