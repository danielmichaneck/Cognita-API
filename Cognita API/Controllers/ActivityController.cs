using Cognita.API.Service.Contracts;
using Cognita_Shared.Dtos.Activity;
using Cognita_Shared.Dtos.Course;
using Cognita_Shared.Dtos.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cognita_API.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ActivityController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("courses/{id}/modules/{id2}/activities")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Get all activities for a module",
            Description = "Get all available activities for a module",
            OperationId = "GetActivitiesForModule"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Activities found", Type = typeof(ActivityDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No activities found")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivitiesForModule(int id)
        {
            var activities = await _serviceManager.ActivityService.GetActivitiesAsync(id);

            if (activities == null)
            {
                return NotFound();
            }

            return Ok(activities);
        }

        [HttpPost("courses/{id}/modules/{id2}/activities")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Create a new activity",
            Description = "Create a new activity for a module",
            OperationId = "PostActivity"
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Activity created successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request")]
        public async Task<ActionResult<ActivityDto>> PostActivity(
            int id,
            ActivityForCreationDto dto
        )
        {
            if (!ModelState.IsValid)
            {
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

        // PUT: api/activities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("activities/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Edit an activity by id",
            Description = "Edit an activity by id",
            OperationId = "EditActivityById"
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Activity edited successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The activity was not found")]
        public async Task<IActionResult> PutCourse(int id, ActivityForUpdateDto dto) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            if (await _serviceManager.ActivityService.EditActivityAsync(id, dto)) {
                return NoContent();
            } else {
                return NotFound();
            }
        }
    }
}
