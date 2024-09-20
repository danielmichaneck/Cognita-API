using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cognita_API.Infrastructure.Data
{
    public class CognitaDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public CognitaDbContext(DbContextOptions<CognitaDbContext> options)
            : base(options) { }

        public DbSet<Course> Course => Set<Course>();
    }
}
