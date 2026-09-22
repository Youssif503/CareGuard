namespace CareGuard.Domain.Models;

public class Medication
{
    public string Id { get; set; }

    public string ElderProfileId { get; set; }

    public ElderProfile ElderProfile { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Form { get; set; }

    public string? Strength { get; set; }

    public int Stock { get; set; }

    public int LowStockThreshold { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<MedicationSchedule> Schedules { get; set; }
        = new List<MedicationSchedule>();
}