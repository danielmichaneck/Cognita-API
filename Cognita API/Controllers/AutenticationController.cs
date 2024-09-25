using Cognita.API.Service.Contracts;
using Cognita_Infrastructure.Models.Dtos;
using Cognita_Shared.Dtos.Course;
using Cognita_Shared.Dtos.User;
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
            Summary = "Registers a user",
            Description = "Register user with name, password, email, and courseId",
            OperationId = "RegisterUser"
        )]
    [SwaggerResponse(StatusCodes.Status201Created, "The user was registered successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The user was not registered")]
    public async Task<IActionResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var result = await _serviceManager.AuthService.RegisterUserAsync(userForRegistration);
        return result.Succeeded
            ? StatusCode(StatusCodes.Status201Created)
            : BadRequest(result.Errors);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
            Summary = "User login",
            Description = "Allows a user to log in to the app",
            OperationId = "Login"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was logged in successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The user could not be logged in")]
    public async Task<IActionResult> Authenticate(UserForAuthenticationDto user)
    {
        if (!await _serviceManager.AuthService.ValidateUserAsync(user))
            return Unauthorized();

        TokenDto tokenDto = await _serviceManager.AuthService.CreateTokenAsync(expireTime: true);
        return Ok(tokenDto);
    }
}
