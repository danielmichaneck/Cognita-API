using System;
using Cognita_Shared.Dtos.Module;
using Cognita_Shared.Entities;

namespace Cognita_Service.Contracts;

public interface IModuleService
{
    Task<IEnumerable<ModuleDto>> GetModulesAsync(int courseId);

    Task<ModuleDto> GetSingleModuleAsync(int id);

    Task<bool> EditModuleAsync(int id, ModuleForUpdateDto dto);

    Task<ModuleDto> CreateModuleAsync(ModuleForCreationDto dto, int courseId);

    void AddActivityToModule(Activity activity, int id);
}
