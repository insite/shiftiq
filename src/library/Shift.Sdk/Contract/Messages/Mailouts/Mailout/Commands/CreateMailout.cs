using System;

namespace Shift.Contract
{
    public class CreateMailout
    {
        public Guid MailoutId { get; set; }
        public Guid? EventId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? SurveyId { get; set; }
        public Guid? UserId { get; set; }
        public Guid SenderId { get; set; }
        public Guid? MessageId { get; set; }

        public string SenderStatus { get; set; }
        public string SenderType { get; set; }
        public string MessageType { get; set; }
        public string MessageName { get; set; }
        public string ContentBodyHtml { get; set; }
        public string ContentBodyText { get; set; }
        public string ContentPriority { get; set; }
        public string ContentSubject { get; set; }
        public string ContentVariables { get; set; }
        public string ContentAttachments { get; set; }
        public string MailoutStatus { get; set; }
        public string MailoutStatusCode { get; set; }
        public string MailoutStatusDescription { get; set; }
        public string MailoutError { get; set; }
        public string RecipientEmailsTo { get; set; }
        public string RecipientEmailsCc { get; set; }
        public string RecipientEmailsBcc { get; set; }
        public string RecipientIdentifiersTo { get; set; }
        public string RecipientIdentifiersCc { get; set; }
        public string RecipientIdentifiersBcc { get; set; }
        public string RecipientListTo { get; set; }
        public string RecipientListCc { get; set; }
        public string RecipientListBcc { get; set; }

        public DateTimeOffset MailoutScheduled { get; set; }
        public DateTimeOffset? MailoutStarted { get; set; }
        public DateTimeOffset? MailoutCancelled { get; set; }
        public DateTimeOffset? MailoutCompleted { get; set; }
    }
}