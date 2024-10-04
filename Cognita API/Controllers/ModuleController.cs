using Cognita.API.Service.Contracts;
using Cognita_Shared.Dtos.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cognita_API.Controllers
{
    [Authorize]
    [Route("api/courses/{id}/modules")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ModuleController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Get all modules for a course",
            Description = "Get all available modules for a course",
            OperationId = "GetModulesForCourse"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Modules found", Type = typeof(ModuleDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No modules found")]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesForCourse(int id)
        {
            var modules = await _serviceManager.ModuleService.GetModulesAsync(id);

            if (modules == null)
            {
                return NotFound();
            }

            return Ok(modules);
        }

        [HttpPost]
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
    }
}
