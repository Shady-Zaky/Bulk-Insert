using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BulkInsertAPI.Infrastructure.Data.Configurations
{
    public class WorkerZoneAssignmentConfiguration : IEntityTypeConfiguration<WorkerZoneAssignment>
    {
        public void Configure(EntityTypeBuilder<WorkerZoneAssignment> builder)
        {
            builder.ToTable("worker_zone_assignment");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("id");

            builder.Property(t => t.AssignmentDate)
                .HasColumnName("effective_date");

            builder.Property(t => t.ZoneId)
                .HasColumnName("zone_id");

            builder.Property(t => t.WorkerId)
                .HasColumnName("worker_id");

            builder.HasOne(x => x.Worker).WithMany(x => x.WorkerZoneAssignments).HasForeignKey(x => x.WorkerId);

            builder.HasOne(x => x.Zone).WithMany(x => x.WorkerZoneAssignments).HasForeignKey(x => x.ZoneId);

        }
    }
}
