using CareGuard.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MedicationConfiguration
    : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .Property(x => x.Description)
            .HasMaxLength(500);

        builder
            .Property(x => x.Form)
            .HasMaxLength(100);

        builder
            .Property(x => x.Strength)
            .HasMaxLength(100);

        builder
            .Property(x => x.Stock)
            .IsRequired();

        builder
            .Property(x => x.LowStockThreshold)
            .IsRequired();

        builder
            .HasOne(x => x.ElderProfile)
            .WithMany(x => x.Medications)
            .HasForeignKey(x => x.ElderProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}