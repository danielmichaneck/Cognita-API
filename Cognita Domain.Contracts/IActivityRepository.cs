using Cognita_Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Domain.Contracts {
    public interface IActivityRepository {
        Task<IEnumerable<Activity>> GetAllActivitiesInModuleAsync(int courseId, int moduleId);
        Task<Activity?> GetSingleActivityAsync(int id, bool trackChanges = false);
        Task CreateActivityAsync(Activity activity, int courseId, int moduleId);

        Task<bool> EditActivityAsync(int id, Activity activity);

        //TODO Delete?
        // Task<bool> EditModuleAsync(int id, Module module);
    }
}
