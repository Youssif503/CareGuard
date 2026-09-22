using CareGuard.Domain.Models;
using CareGuard.Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuard.Infrastructure.Data.Configuration;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder
            .HasOne(u => u.Elder)
            .WithOne(e => e.User)
            .HasForeignKey<ElderProfile>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(u => u.CareGiver)
            .WithOne(c => c.User)
            .HasForeignKey<CareGiverProfile>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}