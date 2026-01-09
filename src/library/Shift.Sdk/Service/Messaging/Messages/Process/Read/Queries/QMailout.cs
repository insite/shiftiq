using System;
using System.Collections.Generic;

namespace InSite.Application.Messages.Read
{
    public class QMailout
    {
        public Guid? EventIdentifier { get; set; }
        public Guid MailoutIdentifier { get; set; }
        public Guid? MessageIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid SenderIdentifier { get; set; }
        public Guid? SurveyIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string ContentAttachments { get; set; }
        public string ContentBodyHtml { get; set; }
        public string ContentBodyText { get; set; }
        public string ContentSubject { get; set; }
        public string ContentVariables { get; set; }
        public string MailoutError { get; set; }
        public string MailoutStatus { get; set; }
        public string MailoutStatusCode { get; set; }
        public string MessageName { get; set; }
        public string MessageType { get; set; }

        public string RecipientListBcc { get; set; }
        public string RecipientListCc { get; set; }
        public string RecipientListTo { get; set; }

        public string SenderStatus { get; set; }
        public string SenderType { get; set; }

        public DateTimeOffset? MailoutCancelled { get; set; }
        public DateTimeOffset? MailoutCompleted { get; set; }
        public DateTimeOffset MailoutScheduled { get; set; }
        public DateTimeOffset? MailoutStarted { get; set; }

        public bool IsStarted => MailoutStarted.HasValue;

        public virtual ICollection<QRecipient> Recipients { get; set; } = new HashSet<QRecipient>();
    }

    public class VMailout
    {
        public Guid? EventIdentifier { get; set; }
        public Guid MailoutIdentifier { get; set; }
        public Guid? MessageIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid SenderIdentifier { get; set; }
        public Guid? SurveyIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string ContentAttachments { get; set; }
        public string ContentBodyHtml { get; set; }
        public string ContentBodyText { get; set; }
        public string ContentSubject { get; set; }
        public string ContentVariables { get; set; }
        public string MailoutError { get; set; }
        public string MailoutStatus { get; set; }
        public string MailoutStatusCode { get; set; }
        public string MessageName { get; set; }
        public string MessageType { get; set; }
        public string OrganizationName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string SenderType { get; set; }
        public string UserName { get; set; }

        public string RecipientListBcc { get; set; }
        public string RecipientListCc { get; set; }
        public string RecipientListTo { get; set; }

        public int? DeliveryCount { get; set; }
        public int? DeliveryCountFailure { get; set; }
        public int? DeliveryCountSuccess { get; set; }
        public int? SubscriberCount { get; set; }
        public int? SurveyFormAsset { get; set; }

        public DateTimeOffset? MailoutCancelled { get; set; }
        public DateTimeOffset? MailoutCompleted { get; set; }
        public DateTimeOffset MailoutScheduled { get; set; }
        public DateTimeOffset? MailoutStarted { get; set; }

        public bool IsStarted => MailoutStarted.HasValue;
    }
}