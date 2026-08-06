namespace Shift.Service.Directory;

public partial class PendingPersonEntity
{
    public Guid PendingPersonIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid? GroupIdentifier { get; set; }
    public Guid SubmittedBy { get; set; }

    public string PersonCode { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string UserFirstName { get; set; } = null!;
    public string UserLastName { get; set; } = null!;
    public string? UserMiddleName { get; set; }
    public string? JobDivision { get; set; }
    public string? JobTitle { get; set; }
    public string? WorkAddressStreet1 { get; set; }
    public string? WorkAddressStreet2 { get; set; }
    public string? WorkAddressCity { get; set; }
    public string? WorkAddressProvince { get; set; }
    public string? WorkAddressPostalCode { get; set; }
    public string EmployeeStatus { get; set; } = null!;

    public DateTimeOffset SubmittedAt { get; set; }
}
