using CareGuard.Domain.Enums;
namespace CareGuard.Domain.Models;
public class MedicationScheduleDose
{
    public string Id { get; set; }

    public string MedicationScheduleId { get; set; }

    public MedicationSchedule MedicationSchedule { get; set; } = null!;

    public DateTime ScheduledAt { get; set; }

    public int Quantity { get; set; }

    public DoseStatus Status { get; set; }

    public DateTime? TakenAt { get; set; }
}