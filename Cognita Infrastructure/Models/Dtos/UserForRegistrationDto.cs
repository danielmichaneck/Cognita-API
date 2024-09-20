using System.ComponentModel.DataAnnotations;

namespace Cognita_Infrastructure.Models.Dtos;

public record UserForRegistrationDto
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; init; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string? Email { get; init; }

    [Required(ErrorMessage = "CourseId is required")]
    public int CourseId { get; init; }
}
