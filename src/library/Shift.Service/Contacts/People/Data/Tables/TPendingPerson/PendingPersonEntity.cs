namespace Shift.Service.Directory;

public partial class PendingPersonEntity
{
    public Guid SubmittedBy { get; set; }
    public Guid PendingId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? PersonId { get; set; }
    public Guid? UserId { get; set; }

    public string PendingStatus { get; set; } = null!;
    public string PersonCode { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string UserFirstName { get; set; } = null!;
    public string UserLastName { get; set; } = null!;

    public DateTimeOffset SubmittedAt { get; set; }
}
