using Shift.Service.Security;

namespace Shift.Service.Booking;

public partial class EventUserEntity
{
    public QEventEntity? Event { get; set; }
    public QUserEntity? User { get; set; }

    public Guid EventIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid UserIdentifier { get; set; }

    public string AttendeeRole { get; set; } = null!;

    public DateTimeOffset? Assigned { get; set; }
}