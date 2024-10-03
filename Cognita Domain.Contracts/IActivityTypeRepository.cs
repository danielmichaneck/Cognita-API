using Cognita_Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Domain.Contracts
{
    public interface IActivityTypeRepository {
        Task<ActivityType?> GetSingleActivityTypeAsync(int id, bool trackChanges = false);
    }
}
