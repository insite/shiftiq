using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VEventRegistrationPaymentFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }

        public DateTimeOffset? EventDateSince { get; set; }
        public DateTimeOffset? EventDateBefore { get; set; }
        public string EventName { get; set; }
        public string EmployerName { get; set; }
        public string RegistrantName { get; set; }
        public string LearnerName { get; set; }
        public string LearnerCode { get; set; }
        public int? InvoiceNumber { get; set; }
        public string PaymentStatus { get; set; }
        public DateTimeOffset? InvoiceSubmittedSince { get; set; }
        public DateTimeOffset? InvoiceSubmittedBefore { get; set; }
        public DateTimeOffset? PaymentApprovedSince { get; set; }
        public DateTimeOffset? PaymentApprovedBefore { get; set; }
        public string PaymentTransactionId { get; set; }
        public string AchievementTitle { get; set; }
        public string InvoiceStatus { get; set; }

        public VEventRegistrationPaymentFilter()
        {

        }

        public VEventRegistrationPaymentFilter Clone()
        {
            return (VEventRegistrationPaymentFilter)MemberwiseClone();
        }
    }
}
