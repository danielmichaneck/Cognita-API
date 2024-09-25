using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cognita.API.Service.Contracts;
using Cognita_API.Controllers;
using Cognita_Infrastructure.Models.Dtos;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Cognita.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private ApplicationUser? _user;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        //RoleManager<IdentityRole> roleManager,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Creates a new TokenDto, a tuple of an access token and a refresh token.
    /// </summary>
    /// <param name="refreshTokenExpireTime">Time in milliseconds that a refresh token should be valid for. Should be larger than accessTokenExpireTime.</param>
    /// <param name="accessTokenExpireTime">Time in milliseconds that an access token should be valid for. Should be smaller than refreshTokenExpireTime.</param>
    /// <param name="expires">Dictates if the TokenDto should expire or not.</param>
    /// <returns></returns>

    public async Task<TokenDto> CreateTokenAsync(long refreshTokenExpireTime, long accessTokenExpireTime, bool expires = true)
    {
        SigningCredentials signing = GetSigningCredentials();
        IEnumerable<Claim> claims = GetClaims();
        JwtSecurityToken tokenOptions = GenerateTokenOptions(signing, claims, accessTokenExpireTime);

        ArgumentNullException.ThrowIfNull(_user, nameof(_user));

        _user.RefreshToken = GenerateRefreshToken();

        if (expires)
            _user.RefreshTokenExpireTime = DateTime.UtcNow.AddMilliseconds(refreshTokenExpireTime);


        var res = await _userManager.UpdateAsync(_user); //TODO: Validate res!
        string accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, _user.RefreshToken);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private JwtSecurityToken GenerateTokenOptions(
        SigningCredentials signing,
        IEnumerable<Claim> claims,
        long  accessTokenExpireTime
    )
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var tokenOptions = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMilliseconds(accessTokenExpireTime),
            signingCredentials: signing
        );

        return tokenOptions;
    }

    private IEnumerable<Claim> GetClaims()
    {
        ArgumentNullException.ThrowIfNull(_user);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, _user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, _user.Id!)
            //Add more if needed
        };

        return claims;
    }

    private SigningCredentials GetSigningCredentials()
    {
        string? secretKey = _configuration["secretkey"];
        ArgumentNullException.ThrowIfNull(secretKey, nameof(secretKey));

        byte[] key = Encoding.UTF8.GetBytes(secretKey);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    public async Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistration)
    {
        ArgumentNullException.ThrowIfNull(userForRegistration, nameof(userForRegistration));

        var user = new ApplicationUser {
            Email = userForRegistration.Email,
            UserName = userForRegistration.Email,

            User = new Cognita_Shared.Entities.User {
                CourseId = userForRegistration.CourseId,
                Name = userForRegistration.Name,
                Role = UserRole.Student
            }
        };

        IdentityResult result = await _userManager.CreateAsync(user, userForRegistration.Password!);

        return result;
    }

    public async Task<bool> ValidateUserAsync(UserForAuthenticationDto userDto)
    {
        ArgumentNullException.ThrowIfNull(userDto, nameof(userDto));

        _user = await _userManager.FindByNameAsync(userDto.UserName!);

        return _user != null && await _userManager.CheckPasswordAsync(_user, userDto.Password!);
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto token)
    {
        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token.AccessToken);

        ApplicationUser? user = await _userManager.FindByNameAsync(principal.Identity?.Name!);
        if (
            user == null
            || user.RefreshToken != token.RefreshToken
            || user.RefreshTokenExpireTime <= DateTime.Now
        )
            //ToDo: Handle with middleware and custom exception class
            throw new ArgumentException("The TokenDto has som invalid values");

        this._user = user;

        return await CreateTokenAsync(AuthenticationController.STANDARD_REFRESH_TOKEN_EXPIRE_TIME_MS, 
            AuthenticationController.STANDARD_ACCESS_TOKEN_EXPIRE_TIME_MS);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");

        string? secretKey = _configuration["secretkey"];
        ArgumentNullException.ThrowIfNull(nameof(secretKey));

        TokenValidationParameters tokenValidationParameters =
            new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
            };

        JwtSecurityTokenHandler tokenHandler = new();

        ClaimsPrincipal principal = tokenHandler.ValidateToken(
            accessToken,
            tokenValidationParameters,
            out SecurityToken securityToken
        );

        if (
            securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
            )
        )
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
