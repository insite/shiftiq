namespace Shift.Service.Workflow;

public partial class CaseDocumentRequestEntity
{
    public Guid CaseIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid RequestedUserIdentifier { get; set; }

    public string RequestedFileCategory { get; set; } = null!;
    public string? RequestedFileDescription { get; set; }
    public string? RequestedFileSubcategory { get; set; }
    public string RequestedFrom { get; set; } = null!;

    public DateTimeOffset RequestedTime { get; set; }
}