using Cognita_Infrastructure.Models.Entities;
using Cognita_Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cognita_Infrastructure.Data
{
    public class CognitaDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public CognitaDbContext(DbContextOptions<CognitaDbContext> options)
            : base(options) { }

        public DbSet<Course> Course => Set<Course>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property("Email").IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Courses)
                .WithMany();

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Files);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Files);

            modelBuilder.Entity<Module>()
                .HasOne(c => c.Files);

            modelBuilder.Entity<Activity>()
                .HasOne(c => c.Files);

            modelBuilder.Entity<DocumentHolder>()
                .HasMany(dh => dh.Docs)
                .WithOne();
        }
    }
}
