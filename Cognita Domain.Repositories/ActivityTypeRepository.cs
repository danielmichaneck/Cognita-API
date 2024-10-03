using Cognita_Domain.Contracts;
using Cognita_Infrastructure.Data;
using Cognita_Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cognita_Domain.Repositories
{
    public class ActivityTypeRepository : RepositoryBase<ActivityType>, IActivityTypeRepository
    {
        public ActivityTypeRepository(CognitaDbContext context)
            : base(context) { }

        public async Task<ActivityType?> GetSingleActivityTypeAsync(int id, bool trackChanges = false) =>
            await GetByCondition(c => c.ActivityTypeId == id).FirstOrDefaultAsync();
    }
}
