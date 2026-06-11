namespace Shift.Service.Messaging;

public partial class RecipientEntity
{
    public Guid RecipientIdentifier { get; set; }
    public Guid MailoutIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid UserIdentifier { get; set; }

    public string UserEmail { get; set; } = null!;
    public string? PersonCode { get; set; }
    public string? PersonName { get; set; }
    public string? PersonLanguage { get; set; }
    public string? RecipientVariables { get; set; }
    public string? DeliveryStatus { get; set; }
    public string? DeliveryError { get; set; }

    public DateTimeOffset? DeliveryStarted { get; set; }
    public DateTimeOffset? DeliveryCompleted { get; set; }
    public DateTime? DeliveryCallbackTimestamp { get; set; }
}