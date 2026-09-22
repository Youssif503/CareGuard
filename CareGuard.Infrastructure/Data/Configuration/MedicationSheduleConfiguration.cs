using CareGuard.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuard.Infrastructure.Data.Configuration;

public class MedicationScheduleConfiguration
    : IEntityTypeConfiguration<MedicationSchedule>
{
    public void Configure(EntityTypeBuilder<MedicationSchedule> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.Medication)
            .WithMany(x => x.Schedules)
            .HasForeignKey(x => x.MedicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.Time)
            .IsRequired();

        builder
            .Property(x => x.Quantity)
            .IsRequired();

        builder
            .Property(x => x.IsActive)
            .IsRequired();
    }
}