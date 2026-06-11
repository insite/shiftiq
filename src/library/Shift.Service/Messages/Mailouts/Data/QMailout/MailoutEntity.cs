namespace Shift.Service.Messaging;

public partial class MailoutEntity
{
    public Guid MailoutIdentifier { get; set; }
    public Guid? EventIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid? SurveyIdentifier { get; set; }
    public Guid? UserIdentifier { get; set; }
    public Guid SenderIdentifier { get; set; }
    public Guid? MessageIdentifier { get; set; }

    public string? SenderStatus { get; set; }
    public string? SenderType { get; set; }
    public string MessageType { get; set; } = null!;
    public string MessageName { get; set; } = null!;
    public string? ContentBodyHtml { get; set; }
    public string? ContentBodyText { get; set; }
    public string? ContentPriority { get; set; }
    public string ContentSubject { get; set; } = null!;
    public string? ContentVariables { get; set; }
    public string? ContentAttachments { get; set; }
    public string MailoutStatus { get; set; } = null!;
    public string? MailoutStatusCode { get; set; }
    public string? MailoutStatusDescription { get; set; }
    public string? MailoutError { get; set; }
    public string? RecipientEmailsTo { get; set; }
    public string? RecipientEmailsCc { get; set; }
    public string? RecipientEmailsBcc { get; set; }
    public string? RecipientIdentifiersTo { get; set; }
    public string? RecipientIdentifiersCc { get; set; }
    public string? RecipientIdentifiersBcc { get; set; }
    public string? RecipientListTo { get; set; }
    public string? RecipientListCc { get; set; }
    public string? RecipientListBcc { get; set; }

    public DateTimeOffset MailoutScheduled { get; set; }
    public DateTimeOffset? MailoutStarted { get; set; }
    public DateTimeOffset? MailoutCancelled { get; set; }
    public DateTimeOffset? MailoutCompleted { get; set; }
}
