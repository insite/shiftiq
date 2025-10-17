using System;

using Shift.Common;

namespace InSite.Domain.Messages
{
    [Serializable]
    public class DeliveryFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public Guid? MessageIdentifier { get; set; }
        public Guid? MailoutIdentifier { get; set; }
        public string RecipientAddress { get; set; }
        public string Status { get; set; }
        public string Keyword { get; set; }
    }

    [Serializable]
    public class MailoutFilter : Filter
    {
        public string PostOffice { get; set; }

        public Guid? OrganizationIdentifier { get; set; }
        public Guid? MailoutIdentifier { get; set; }
        public Guid? MessageIdentifier { get; set; }
        public Guid? EventIdentifier { get; set; }

        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string MessageType { get; set; }
        public string MessageName { get; set; }

        public DateTimeOffsetRange Scheduled { get; set; }
        public DateTimeOffsetRange Completed { get; set; }

        public bool? IsCancelled { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsStarted { get; set; }
        public bool? IsEmpty { get; set; }

        public int? MinDeliveryCount { get; set; }
        public int? MaxDeliveryCount { get; set; }
    }
}
