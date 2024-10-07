using System;
using AutoMapper;
using Cognita_Domain.Contracts;
using Cognita_Service.Contracts;
using Cognita_Shared.Dtos.Module;
using Cognita_Shared.Entities;

namespace Cognita_Service;

public class ModuleService : IModuleService
{
    private readonly IMapper _mapper;
    private readonly IUoW _uow;

    public ModuleService(IMapper mapper, IUoW uow)
    {
        _mapper = mapper;
        _uow = uow;
    }

    public async Task<ModuleDto> CreateModuleAsync(ModuleForCreationDto dto, int courseId)
    {
        var module = _mapper.Map<Module>(dto);
        await _uow.ModuleRepository.CreateModuleAsync(module, courseId);
        await _uow.CompleteAsync();
        return _mapper.Map<ModuleDto>(module);
    }

    public async Task<bool> EditModuleAsync(int id, ModuleForUpdateDto dto)
    {
        var module = await _uow.ModuleRepository.GetSingleModuleAsync(id, trackChanges: true);

        if (module is null)
        {
            return false;
        }

        _mapper.Map(dto, module);
        try
        {
            await _uow.CompleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<IEnumerable<ModuleDto>> GetModulesAsync(int courseId)
    {
        var modules = await _uow.ModuleRepository.GetAllModulesAsync(courseId);
        return _mapper.Map<IEnumerable<ModuleDto>>(modules);
    }

    public async Task<ModuleDto> GetSingleModuleAsync(int id)
    {
        var module = await _uow.ModuleRepository.GetSingleModuleAsync(id);
        return _mapper.Map<ModuleDto>(module);
    }

    public async void AddActivityToModule(Activity activity, int id) {
        var module = await _uow.ModuleRepository.GetSingleModuleAsync(id, trackChanges: true);
        module.Activities.Add(activity);
    }
}
