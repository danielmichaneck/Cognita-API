using System;
using Cognita_Shared.Dtos.Module;

namespace Cognita_Service.Contracts;

public interface IModuleService
{
    Task<IEnumerable<ModuleDto>> GetModulesAsync(int courseId);

    Task<ModuleDto> GetSingleModuleAsync(int id);

    Task<bool> EditModuleAsync(int id, ModuleForUpdateDto dto);

    Task<ModuleDto> CreateModuleAsync(ModuleForCreationDto dto, int courseId);
}
