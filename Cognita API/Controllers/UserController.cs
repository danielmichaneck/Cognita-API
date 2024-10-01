﻿using Cognita.API.Service.Contracts;
using Cognita_Shared.Dtos.Module;
using Cognita_Shared.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cognita_API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public UserController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("courses/{id}/users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Get all users in a course",
            Description = "Get all users in a course",
            OperationId = "GetUsersInCourse"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Users found", Type = typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No users found")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersInCourse(int id)
        {
            var users = await _serviceManager.UserService.GetUsersInCourseAsync(id);

            if (users is null) {
                return NotFound();
            }

            return Ok(users);
        }

        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Get all users",
            OperationId = "GetUsersInCourse"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Users found", Type = typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No users found")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _serviceManager.UserService.GetAllUsersAsync();

            if (users is null) {
                return NotFound();
            }

            return Ok(users);
        }
    }
}