using Cognita.API.Service.Contracts;
using Cognita_Infrastructure.Models.Dtos;
using Cognita_Shared.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cognita_API.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AuthenticationController(IServiceManager serviceManager)
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
    public async Task<IActionResult> RegisterUser(UserForRegistrationDto userForRegistrationDto)
    {
        var result = await _serviceManager.AuthService.RegisterUserAsync(userForRegistrationDto);
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

        TokenDto tokenDto = await _serviceManager.AuthService.CreateTokenAsync();
        return Ok(tokenDto);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
            Summary = "User refresh access token",
            Description = "Allows a user to refresh their access token",
            OperationId = "Refresh"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Access token refreshed")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The access token could not be refreshed")]
    public async Task<IActionResult> Refresh(TokenDto currentToken) {

        try {
            TokenDto tokenDto = await _serviceManager.AuthService.RefreshTokenAsync(currentToken);
            return Ok(tokenDto);
            //Return
        } catch (Exception) {
            return BadRequest();
        }
    }
}
