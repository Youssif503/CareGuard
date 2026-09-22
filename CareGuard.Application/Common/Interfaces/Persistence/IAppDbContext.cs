using CareGuard.Domain.Models;
using CareGuard.Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareGuard.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<AppUser> User { get;}
    DbSet<CareGiverProfile> CareGiver { get; }
    DbSet<RefreshToken> RefreshToken { get; }
    DbSet<ElderProfile> Elder { get;  }
    DbSet<Medication> Medication { get;}
    DbSet<MedicationSchedule> MedicationSchedule { get; }
    DbSet<MedicationScheduleDose> MedicationScheduleDose { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}