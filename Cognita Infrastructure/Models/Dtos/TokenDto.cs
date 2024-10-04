namespace Cognita_Infrastructure.Models.Dtos;

public record TokenDto(string AccessToken, string RefreshToken, string Role);
