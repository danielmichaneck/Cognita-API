using System.ComponentModel.DataAnnotations;

namespace Cognita.API.Models.Dtos;

public record UserForAuthenticationDto
{
    [Required]
    public string? UserName { get; init; }

    [Required]
    public string? Password { get; init; }
}
