using Cognita_Shared.Dtos.Activity;
using Cognita_Shared.Dtos.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Service.Contracts {
    public interface IActivityService {
        Task<IEnumerable<ActivityDto>> GetActivitiesAsync(int moduleId);

        Task<ActivityDto> GetSingleActivityAsync(int id);

        Task<bool> EditActivityAsync(int id, ActivityForUpdateDto dto);

        Task<ActivityDto> CreateActivityAsync(ActivityForCreationDto dto, int moduleId);
    }
}
