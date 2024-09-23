using System;
using Cognita_Shared.Entities;

namespace Cognita_Domain.Contracts;

public interface IModuleRepository
{
    Task<IEnumerable<Module>> GetAllModulesAsync(int courseId);
    Task<Module?> GetSingleModuleAsync(int id, bool trackChanges = false);
    Task CreateModuleAsync(Module module, int courseId);

    //TODO Delete?
    // Task<bool> EditModuleAsync(int id, Module module);
}
