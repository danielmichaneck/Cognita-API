using Cognita.API.Service.Contracts;
using Cognita_Shared.Dtos.Activity;
using Cognita_Shared.Dtos.Module;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cognita_API.Controllers {
    [Route("api/courses/{id}/modules/{id2}/activities")]
    [ApiController]
    public class ActivityController : ControllerBase {
        private readonly IServiceManager _serviceManager;

        public ActivityController(IServiceManager serviceManager) {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Get all activities for a module",
            Description = "Get all available activities for a module",
            OperationId = "GetActivitiesForModule"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Activities found", Type = typeof(ActivityDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No activities found")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivitiesForModule(int id) {
            var activities = await _serviceManager.ActivityService.GetActivitiesAsync(id);

            if (activities == null) {
                return NotFound();
            }

            return Ok(activities);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Create a new activity",
            Description = "Create a new activity for a module",
            OperationId = "PostActivity"
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Activity created successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request")]
        public async Task<ActionResult<ActivityDto>> PostActivity(int id, ActivityForCreationDto dto) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            var activityDTO = await _serviceManager.ActivityService.CreateActivityAsync(
                dto,
                moduleId: id
            );
            return CreatedAtAction(
                nameof(PostActivity),
                new { moduleId = id, activityId = activityDTO.ActivityId },
                activityDTO
            );
        }
    }
}
