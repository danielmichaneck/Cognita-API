using Cognita_Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cognita_API.Infrastructure.Data {
    public class CognitaDbContext : DbContext {
        public CognitaDbContext(DbContextOptions<CognitaDbContext> options)
            : base(options) { }

        public DbSet<Course> Course => Set<Course>();

    }
}
