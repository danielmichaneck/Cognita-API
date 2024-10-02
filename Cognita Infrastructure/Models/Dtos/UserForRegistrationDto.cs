using Cognita_Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Cognita_Infrastructure.Models.Dtos;

public record UserForRegistrationDto
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Password is required")]
    public required string? Password { get; init; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public required string? Email { get; init; }

    [Required(ErrorMessage = "CourseId is required")]
    public required int CourseId { get; init; }

    [Required(ErrorMessage = "Role is required")]
    public required UserRole Role { get; init; }
}
