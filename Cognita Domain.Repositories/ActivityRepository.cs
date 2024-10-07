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
        throw new NotImplementedException();
    }

    public Task<bool> EditActivityAsync(int id, Activity activity) {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Activity>> GetAllActivitiesInModuleAsync(int courseId, int moduleId) {
        throw new NotImplementedException();
    }

    public async Task<Activity?> GetSingleActivityAsync(int id, bool trackChanges = false) =>
        await GetByCondition(a => a.ActivityId == id, trackChanges).FirstOrDefaultAsync();
}
