using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Sales.Reports.EventRegistrationPayment
{
    public partial class SearchResults : SearchResultsGridViewController<VEventRegistrationPaymentFilter>
    {
        #region Classes 

        public class ExportDataItem
        {
            public DateTimeOffset EventDate { get; set; }
            public string EventName { get; set; }
            public string AchievementTitle { get; set; }
            public Guid RegistrationIdentifier { get; set; }
            public string RegisteredBy { get; set; }
            public string EmployerAtTimeOfRegistration { get; set; }
            public string Registrant { get; set; }
            public string LearnerCode { get; set; }
            public string CurrentRegistrationStatus { get; set; }
            public decimal? RegistrationFee { get; set; }
            public string CurrentAttendanceStatus { get; set; }
            public DateTimeOffset? AttendanceTaken { get; set; }
            public Guid InvoiceIdentifier { get; set; }
            public int? InvoiceNumber { get; set; }
            public string InvoiceStatus { get; set; }
            public DateTimeOffset? InvoiceSubmitted { get; set; }
            public Guid PaymentIdentifier { get; set; }
            public string SeatTitle { get; set; }
            public string OrderNumber { get; set; }
            public string TransactionCode { get; set; }
            public string TransactionStatus { get; set; }
            public decimal TransactionAmount { get; set; }
            public DateTimeOffset? TransactionDate { get; set; }
            public Guid OrganizationIdentifier { get; set; }
            public string OrganizationCode { get; set; }
            public string OrganizationName { get; set; }
            public Guid? CreditIdentifier { get; set; }
            public Guid? CreditPaymentIdentifier { get; set; }
            public DateTimeOffset? CreditSubmitted { get; set; }
            public int? CreditNumber { get; set; }
            public decimal? CreditAmount { get; set; }
        }

        #endregion  

        protected override int SelectCount(VEventRegistrationPaymentFilter filter)
        {
            return VEventRegistrationPaymentSearch.Count(filter);
        }

        protected override IListSource SelectData(VEventRegistrationPaymentFilter filter)
        {
            return VEventRegistrationPaymentSearch
                .Bind(x => x, filter)
                .ToList()
                .ToSearchResult();
        }

        #region Export Data
        public override IListSource GetExportData(VEventRegistrationPaymentFilter filter, bool empty)
        {
            var query = SelectData(filter).GetList().Cast<VEventRegistrationPayment>().AsQueryable();

            if (empty)
                query = query.Take(0);

            var data = query.Select(x => new ExportDataItem
            {
                EventDate = x.EventDate,
                EventName = x.EventName,
                AchievementTitle = x.AchievementTitle,
                RegistrationIdentifier = x.RegistrationIdentifier,
                RegisteredBy = x.RegistrantCardholder,
                EmployerAtTimeOfRegistration = x.EmployerName,
                Registrant = x.LearnerAttendee,
                LearnerCode = x.LearnerCode,
                CurrentRegistrationStatus = x.CurrentRegistrationStatus,
                RegistrationFee = x.RegistrationFee,
                CurrentAttendanceStatus = x.CurrentAttendanceStatus,
                AttendanceTaken = x.AttendanceTaken,
                InvoiceIdentifier = x.InvoiceIdentifier,
                InvoiceNumber = x.InvoiceNumber,
                InvoiceStatus = x.InvoiceStatus,
                InvoiceSubmitted = x.InvoiceSubmitted,
                PaymentIdentifier = x.PaymentIdentifier,
                SeatTitle = x.SeatTitle,
                OrderNumber = x.OrderNumber,
                TransactionCode = x.TransactionCode,
                TransactionStatus = x.TransactionStatus,
                TransactionAmount = x.TransactionAmount,
                TransactionDate = x.TransactionDate,
                OrganizationIdentifier = x.OrganizationIdentifier,
                OrganizationCode = x.OrganizationCode,
                OrganizationName = x.OrganizationName,
                CreditIdentifier = x.CreditIdentifier,
                CreditPaymentIdentifier = x.CreditPaymentIdentifier,
                CreditSubmitted = x.CreditSubmitted,
                CreditNumber = x.CreditNumber,
                CreditAmount = x.CreditAmount
            }).ToList();

            CallUpdateDateTimeOffsets(data, User.TimeZoneId);

            return data.ToSearchResult();
        }

        #endregion
    }
}