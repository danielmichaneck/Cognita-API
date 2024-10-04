﻿using Cognita_Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cognita_Infrastructure.Models.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
    public required string Name { get; set; }

    // Foreign key
    public int FilesId { get; set; }

    // Navigation property
    public ICollection<Course> Courses { get; set; }
    public DocumentHolder Files { get; set; }
}
