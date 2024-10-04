using Cognita.API.Service.Contracts;
using Cognita_Shared.Dtos.Activity;
using Cognita_Shared.Dtos.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cognita_API.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ModuleController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost("courses/{id}/modules")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Create a new module",
            Description = "Create a new module for a course",
            OperationId = "PostModule"
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Module created successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request")]
        public async Task<ActionResult<ModuleDto>> PostModule(int id, ModuleForCreationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var moduleDTO = await _serviceManager.ModuleService.CreateModuleAsync(
                dto,
                courseId: id
            );
            return CreatedAtAction(
                nameof(PostModule),
                new { courseId = id, moduleId = moduleDTO.ModuleId },
                moduleDTO
            );
        }

        // PUT: api/modules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("modules/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Edit a module by id",
            Description = "Edit a module by id",
            OperationId = "EditModuleById"
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Module edited successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The module was not found")]
        public async Task<IActionResult> PutModule(int id, ModuleForUpdateDto dto) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            if (await _serviceManager.ModuleService.EditModuleAsync(id, dto)) {
                return NoContent();
            } else {
                return NotFound();
            }
        }
    }
}
