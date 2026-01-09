using System.Linq;

using InSite.Application.Invoices.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public static class VEventRegistrationPaymentHelper
    {
        public static IQueryable<VEventRegistrationPayment> Filter(this IQueryable<VEventRegistrationPayment> query, VEventRegistrationPaymentFilter filter)
        {
            query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.EventDateSince.HasValue)
                query = query.Where(x => x.EventDate >= filter.EventDateSince.Value);

            if (filter.EventDateBefore.HasValue)
                query = query.Where(x => x.EventDate < filter.EventDateBefore.Value);

            if (filter.EventName.HasValue())
                query = query.Where(x => x.EventName.Contains(filter.EventName));

            if (filter.EmployerName.HasValue())
                query = query.Where(x => x.EmployerName.Contains(filter.EmployerName));

            if (filter.RegistrantName.HasValue())
                query = query.Where(x => x.RegistrantCardholder.Contains(filter.RegistrantName));

            if (filter.LearnerName.HasValue())
                query = query.Where(x => x.LearnerAttendee.Contains(filter.LearnerName));

            if (filter.LearnerCode.HasValue())
                query = query.Where(x => x.LearnerCode.Contains(filter.LearnerCode));

            if (filter.AchievementTitle.HasValue())
                query = query.Where(x => x.AchievementTitle.Contains(filter.AchievementTitle));

            if (filter.InvoiceStatus.HasValue())
                query = query.Where(x => x.InvoiceStatus == filter.InvoiceStatus);

            if (filter.InvoiceNumber.HasValue)
                query = query.Where(x => x.InvoiceNumber == filter.InvoiceNumber);

            if (filter.PaymentStatus.HasValue())
                query = query.Where(x => x.TransactionStatus.Contains(filter.PaymentStatus));

            if (filter.InvoiceSubmittedSince.HasValue)
                query = query.Where(x => x.InvoiceSubmitted >= filter.InvoiceSubmittedSince.Value);

            if (filter.InvoiceSubmittedBefore.HasValue)
                query = query.Where(x => x.InvoiceSubmitted < filter.InvoiceSubmittedBefore.Value);

            if (filter.PaymentApprovedSince.HasValue)
                query = query.Where(x => x.TransactionDate >= filter.PaymentApprovedSince.Value);

            if (filter.PaymentApprovedBefore.HasValue)
                query = query.Where(x => x.TransactionDate <= filter.PaymentApprovedBefore.Value);

            if (filter.PaymentTransactionId.HasValue())
                query = query.Where(x => x.TransactionCode.Contains(filter.PaymentTransactionId));

            return query;
        }
    }
}
