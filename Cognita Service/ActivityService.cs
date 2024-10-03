using AutoMapper;
using Cognita_Domain.Contracts;
using Cognita_Service.Contracts;
using Cognita_Shared.Dtos.Activity;
using Cognita_Shared.Dtos.Module;
using Cognita_Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognita_Service {

    public class ActivityService : IActivityService {

        private readonly IMapper _mapper;
        private readonly IUoW _uow;

        public ActivityService(IMapper mapper, IUoW uow) {
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ActivityDto> CreateActivityAsync(ActivityForCreationDto dto, int moduleId) {
            var activity = _mapper.Map<Activity>(dto);

            var activityType = await _uow.ActivityTypeRepository.GetSingleActivityTypeAsync(dto.ActivityTypeId);

            if (activityType is null) {
                throw new NullReferenceException("The activity type does not exist.");
            }

            activity.ActivityType = activityType;

            var module = await _uow.ModuleRepository.GetSingleModuleAsync(moduleId, true);

            if (module is null) {
                throw new NullReferenceException("The module does not exist.");
            }

            module.Activities.Add(activity);

            

            //await _uow.ActivityRepository.CreateActivityAsync(activity, moduleId);
            await _uow.CompleteAsync();
            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task<bool> EditActivityAsync(int id, ActivityForUpdateDto dto) {
            var activity = await _uow.ActivityRepository.GetSingleActivityAsync(id, trackChanges: true);

            if (activity is null) {
                return false;
            }

            _mapper.Map(dto, activity);

            var activityType = await _uow.ActivityTypeRepository.GetSingleActivityTypeAsync(dto.ActivityTypeId);

            if (activityType is null) {
                return false;
            }

            activity.ActivityType = activityType;

            try {
                await _uow.CompleteAsync();
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<IEnumerable<ActivityDto>> GetActivitiesAsync(int moduleId) {
            /*var activities = await _uow.ActivityRepository.GetAllActivitiesAsync(moduleId);
            return _mapper.Map<IEnumerable<ActivityDto>>(activities);*/
            throw new NotImplementedException();
        }

        public async Task<ActivityDto> GetSingleActivityAsync(int id) {
            throw new NotImplementedException();
        }
    }
}
