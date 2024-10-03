using System;
using Cognita_API.Infrastructure.Data;
using Cognita_Domain.Contracts;

namespace Cognita_Domain.Repositories;

public class UoW : IUoW
{
    private readonly Lazy<IModuleRepository> _moduleRepository;
    private readonly Lazy<ICourseRepository> _courseRepository;
    private readonly Lazy<IActivityRepository> _activityRepository;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<IActivityTypeRepository> _activityTypeRepository;
    private readonly CognitaDbContext _context;

    public ICourseRepository CourseRepository => _courseRepository.Value;
    public IModuleRepository ModuleRepository => _moduleRepository.Value;
    public IActivityRepository ActivityRepository => _activityRepository.Value;
    public IUserRepository UserRepository => _userRepository.Value;
    public IActivityTypeRepository ActivityTypeRepository => _activityTypeRepository.Value;

    public UoW(
        Lazy<ICourseRepository> courseRepository,
        Lazy<IModuleRepository> moduleRepository,
        Lazy<IActivityRepository> activityRepository,
        Lazy<IActivityTypeRepository> activityTypeRepository,
        Lazy<IUserRepository> userRepository,
        CognitaDbContext context
    )
    {
        _moduleRepository = moduleRepository;
        _courseRepository = courseRepository;
        _activityRepository = activityRepository;
        _activityTypeRepository = activityTypeRepository;
        _userRepository = userRepository;
        _context = context;
    }

    //TODO impl try catch return bool here instead of in each service
    public async Task CompleteAsync() => await _context.SaveChangesAsync();
}
