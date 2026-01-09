using System;

namespace InSite.Application.Invoices.Read
{
    public class VEventRegistrationPayment
    {
        public Guid RegistrationIdentifier { get; set; }
        public Guid InvoiceIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid PaymentIdentifier { get; set; }

        public string EmployerName { get; set; }
        public string EventName { get; set; }
        public string LearnerAttendee { get; set; }
        public string LearnerCode { get; set; }
        public string OrderNumber { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }
        public string RegistrantCardholder { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionStatus { get; set; }
        public string AchievementTitle { get; set; }
        public string InvoiceStatus { get; set; }

        public int? InvoiceNumber { get; set; }

        public decimal TransactionAmount { get; set; }

        public decimal? RegistrationFee { get; set; }

        public DateTimeOffset EventDate { get; set; }
        public DateTimeOffset? InvoiceSubmitted { get; set; }
        public DateTimeOffset? TransactionDate { get; set; }
        public DateTimeOffset? AttendanceTaken { get; set; }

        public Guid? CreditIdentifier { get; set; }
        public Guid? CreditPaymentIdentifier { get; set; }
        public DateTimeOffset? CreditSubmitted { get; set; }
        public int? CreditNumber { get; set; }
        public decimal? CreditAmount { get; set; }

        public string SeatTitle { get; set; }
        public string CurrentAttendanceStatus { get; set; }
        public string CurrentRegistrationStatus { get; set; }

        public VEventRegistrationPayment Clone()
            => (VEventRegistrationPayment)MemberwiseClone();
    }
}