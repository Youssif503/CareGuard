using CareGuard.Application.Common.Interfaces;
using CareGuard.Domain.Models;
using CareGuard.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CareGuard.Infrastructure.Data;

public class AppDbContext:IdentityDbContext<AppUser> , IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
        
    }
    
    public DbSet<AppUser> User { get; set; }
    public DbSet<CareGiverProfile> CareGiver { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }
    public DbSet<ElderProfile> Elder { get; set; }
    public DbSet<Medication> Medication { get; set; }
    public DbSet<MedicationSchedule> MedicationSchedule { get; set; }
    public DbSet<MedicationScheduleDose> MedicationScheduleDose { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}