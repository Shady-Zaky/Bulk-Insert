using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BulkInsertAPI.Infrastructure.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<WorkerZoneAssignment> WorkerZoneAssignments { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Worker> Workers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
