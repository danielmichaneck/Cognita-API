using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Cognita.API.Service.Contracts;
using Cognita_Infrastructure.Models.Dtos;
using Cognita_Infrastructure.Models.Entities;
using Cognita_Service.Contracts;
using Cognita_Shared.Dtos.User;
using Cognita_Shared.Entities;
using Cognita_Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;

namespace Cognita.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ICourseService _courseService;
    private ApplicationUser? _user;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IConfiguration configuration,
        IMapper mapper,
        ICourseService courseService
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _mapper = mapper;
        _courseService = courseService;
    }

    /// <summary>
    /// Creates a new TokenDto, a tuple of an access token and a refresh token.
    /// </summary>
    /// <param name="refreshTokenExpireTime">Time in milliseconds that a refresh token should be valid for. Should be larger than accessTokenExpireTime.</param>
    /// <param name="accessTokenExpireTime">Time in milliseconds that an access token should be valid for. Should be smaller than refreshTokenExpireTime.</param>
    /// <param name="expires">Dictates if the TokenDto should expire or not.</param>
    /// <returns></returns>

    public async Task<TokenDto> CreateTokenAsync(bool expires = true)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        SigningCredentials signing = GetSigningCredentials();
        IEnumerable<Claim> claims = GetClaims();
        JwtSecurityToken tokenOptions = GenerateTokenOptions(signing, claims, long.Parse(jwtSettings["accessTokenExpirationTime"]));

        ArgumentNullException.ThrowIfNull(_user, nameof(_user));

        _user.RefreshToken = GenerateRefreshToken();

        if (expires)
            _user.RefreshTokenExpireTime = DateTime.UtcNow.AddMilliseconds(long.Parse(jwtSettings["refreshTokenExpirationTime"]));


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
            new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString()!)
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
            Name = userForRegistration.Name
        };

        var course = await _courseService.GetCourse(userForRegistration.CourseId);

        if (course is null) throw new ArgumentException("The course does not exist.");

        user.Courses = [course];

        IdentityResult result = await _userManager.CreateAsync(user, userForRegistration.Password!);

        // ToDo: Set up roles in configuration
        if (userForRegistration.Role == UserRole.Teacher) {
            await _userManager.AddToRoleAsync(user, "Admin");
        } else {
            await _userManager.AddToRoleAsync(user, "User");
        }

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

        return await CreateTokenAsync();
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

    public async Task<UserDto?> GetUserAsync(int id) {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return null;
        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync(int? courseId = null) {
        var users = await _userManager.Users.Include(u => u.Courses).ToListAsync();
        var userDtos = new List<UserDto>();
        if (courseId is null) {
            return await CreateUserDtos(users);
        }

        var courseUsers = users.Where(u => u.Courses
                                .Where(c => c.CourseId == courseId)
                                .Any())
                               .ToList();

        return await CreateUserDtos(courseUsers);
    }

    public async Task<bool> UpdateUser(int id, UserForUpdateDto dto) {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return false;
        _mapper.Map(dto, user);
        user.UserName = dto.Email;
        user.NormalizedUserName = dto.Email.Normalize();
        user.NormalizedEmail = dto.Email.Normalize();
        await _userManager.UpdateAsync(user);
        return true;
    }

    private async Task<List<UserDto>> CreateUserDtos(List<ApplicationUser> users)
    {
        var userDtos = new List<UserDto>();
        foreach (ApplicationUser user in users)
        {
            if (user.Courses is null) throw new NullReferenceException("User courses is null");
            var userDto = _mapper.Map<UserDto>(user);
            var course = user.Courses.FirstOrDefault();
            if (course is null) throw new ArgumentException("A user has null or default courses.");
            userDto.CourseName = course.CourseName;
            userDto.CourseId = course.CourseId;
            userDto.Role = await GetRoleAsync(user);
            userDtos.Add(userDto);
        }
        return userDtos;
    }

    private async Task<UserRole> GetRoleAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        switch (roles.FirstOrDefault())
        {
            case "Admin":
                return UserRole.Teacher;

            case "User":
                return UserRole.Student;

            default:
                throw new ArgumentException("The user does not have a valid role.");
        }
    }
}
