namespace Shift.Service.Booking;

public partial class RegistrationTimerEntity
{
    public Guid? OrganizationIdentifier { get; set; }
    public Guid RegistrationIdentifier { get; set; }
    public Guid TriggerCommand { get; set; }

    public string? TimerDescription { get; set; }
    public string TimerStatus { get; set; } = null!;

    public DateTimeOffset TriggerTime { get; set; }
}