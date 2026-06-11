using System;

namespace Shift.Contract
{
    public partial class RecipientModel
    {
        public Guid RecipientId { get; set; }
        public Guid MailoutId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }

        public string UserEmail { get; set; }
        public string PersonCode { get; set; }
        public string PersonName { get; set; }
        public string PersonLanguage { get; set; }
        public string RecipientVariables { get; set; }
        public string DeliveryStatus { get; set; }
        public string DeliveryError { get; set; }

        public DateTimeOffset? DeliveryStarted { get; set; }
        public DateTimeOffset? DeliveryCompleted { get; set; }
        public DateTime? DeliveryCallbackTimestamp { get; set; }
    }
}