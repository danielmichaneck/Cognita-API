using System;
using Cognita_API.Infrastructure.Data;
using Cognita_Domain.Contracts;

namespace Cognita_Domain.Repositories;

public class UoW : IUoW
{
    private readonly Lazy<IModuleRepository> _moduleRepository;
    private readonly Lazy<ICourseRepository> _courseRepository;
    private readonly CognitaDbContext _context;

    public ICourseRepository CourseRepository => _courseRepository.Value;
    public IModuleRepository ModuleRepository => _moduleRepository.Value;

    public UoW(
        Lazy<ICourseRepository> courseRepository,
        Lazy<IModuleRepository> moduleRepository,
        CognitaDbContext context
    )
    {
        _moduleRepository = moduleRepository;
        _courseRepository = courseRepository;
        _context = context;
    }

    public async Task CompleteAsync() => await _context.SaveChangesAsync();
}
