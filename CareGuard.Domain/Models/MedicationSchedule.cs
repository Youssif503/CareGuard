namespace CareGuard.Domain.Models;
public class MedicationSchedule
{
    public string Id { get; set; }

    public string MedicationId { get; set; }

    public Medication Medication { get; set; } = null!;

    public TimeOnly Time { get; set; }

    public int Quantity { get; set; }

    public bool IsActive { get; set; } = true;
    
    public ICollection<MedicationScheduleDose> Doses { get; set; }
        = new List<MedicationScheduleDose>();
}