using Cognita.API.Service.Contracts;
using Cognita_Infrastructure.Models.Dtos;
using Cognita_Shared.Dtos.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cognita_API.Controllers;

[Route("api/authentication")]
[ApiController]
public class AutenticationController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AutenticationController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
            Summary = "Creates a user",
            Description = "Create user with name, password, email, and courseId",
            OperationId = "CreateUser"
        )]
    [SwaggerResponse(StatusCodes.Status201Created, "The user was created succesfully", Type = typeof(CourseDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The course was not found")]
    public async Task<IActionResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var result = await _serviceManager.AuthService.RegisterUserAsync(userForRegistration);
        return result.Succeeded
            ? StatusCode(StatusCodes.Status201Created)
            : BadRequest(result.Errors);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
            Summary = "Get a course by id",
            Description = "Get a course by id",
            OperationId = "GetCourseById"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "The course was found", Type = typeof(CourseDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The course was not found")]
    public async Task<IActionResult> Authenticate(UserForAuthenticationDto user)
    {
        if (!await _serviceManager.AuthService.ValidateUserAsync(user))
            return Unauthorized();

        TokenDto tokenDto = await _serviceManager.AuthService.CreateTokenAsync(expireTime: true);
        return Ok(tokenDto);
    }
}
