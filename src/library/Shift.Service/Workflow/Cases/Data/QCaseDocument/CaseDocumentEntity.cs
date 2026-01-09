namespace Shift.Service.Workflow;

public partial class CaseDocumentEntity
{
    public Guid AttachmentIdentifier { get; set; }
    public Guid FileIdentifier { get; set; }
    public Guid IssueIdentifier { get; set; }
    public Guid? OrganizationIdentifier { get; set; }
    public Guid PosterIdentifier { get; set; }

    public string FileName { get; set; } = null!;
    public string? FileType { get; set; }

    public DateTimeOffset AttachmentPosted { get; set; }
}