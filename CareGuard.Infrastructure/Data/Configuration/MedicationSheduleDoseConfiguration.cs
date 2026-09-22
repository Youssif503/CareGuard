using CareGuard.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuard.Infrastructure.Data.Configuration;

public class MedicationScheduleDoseConfiguration
    : IEntityTypeConfiguration<MedicationScheduleDose>
{
    public void Configure(
        EntityTypeBuilder<MedicationScheduleDose> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.MedicationSchedule)
            .WithMany(x => x.Doses)
            .HasForeignKey(x => x.MedicationScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.ScheduledAt)
            .IsRequired();

        builder
            .Property(x => x.Quantity)
            .IsRequired();

        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder
            .Property(x => x.TakenAt)
            .IsRequired(false);
    }
}