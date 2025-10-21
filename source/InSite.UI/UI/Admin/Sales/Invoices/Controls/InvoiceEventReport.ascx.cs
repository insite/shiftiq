using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

using InSite.Application.Events.Read;
using InSite.Application.Invoices.Read;
using InSite.Application.Payments.Read;
using InSite.Application.Registrations.Read;
using InSite.Common.Web;
using InSite.Domain.Invoices;
using InSite.UI.Admin.Events.Classes.Controls;
using InSite.UI.Layout.Portal.Controls;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Invoices.Controls
{
    public partial class InvoiceEventReport : UserControl
    {
        private class RegistrationInfo
        {
            public string EventTitle { get; set; }
            public string EventDate { get; set; }
            public string VenueName { get; set; }
            public string VenueAddress { get; set; }
            public string CandidateInfo { get; set; }
            public string BillingInfo { get; set; }
        }

        private static TimeZoneInfo TimeZone
        {
            get
            {
                return CurrentSessionState.Identity.User?.TimeZone
                    ?? CurrentSessionState.Identity.Organization.TimeZone;
            }
        }

        public static byte[] PrintByRegistration(Guid registrationIdentifier, InvoiceEventReportType reportType)
        {
            var registration = ServiceLocator.RegistrationSearch.GetRegistration(registrationIdentifier);
            var payment = ServiceLocator.PaymentSearch.GetPayment(registration.PaymentIdentifier.Value);

            if (payment == null)
                return null;

            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(payment.InvoiceIdentifier);
            var (report, _) = Print(invoice, payment, new List<QRegistration> { registration }, reportType);

            return report;
        }

        public static (byte[] report, string fileName) PrintByInvoice(Guid invoiceIdentifier, InvoiceEventReportType reportType)
        {
            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(invoiceIdentifier);

            var paymentFilter = new QPaymentFilter { InvoiceIdentifier = invoiceIdentifier, PaymentStatus = "Completed" };
            var payment = ServiceLocator.PaymentSearch.GetPayments(paymentFilter).FirstOrDefault();

            List<QRegistration> registrations = new List<QRegistration>();

            if (invoice.InvoiceAmount < 0 && invoice.ReferencedInvoiceIdentifier.HasValue)
            {
                var referencedRegistration = GetReferencedRegistration(invoice.ReferencedInvoiceIdentifier.Value);
                if (referencedRegistration != null)
                    registrations.Add(referencedRegistration);
            }
            else if (payment != null)
            {
                var registrationFilter = new QRegistrationFilter { PaymentIdentifier = payment.PaymentIdentifier };
                registrations = ServiceLocator.RegistrationSearch.GetRegistrations(registrationFilter);
            }

            return Print(invoice, payment, registrations, reportType);
        }

        private static (byte[] report, string fileName) Print(VInvoice invoice, QPayment payment, List<QRegistration> registrations, InvoiceEventReportType reportType)
        {
            var report = (InvoiceEventReport)new Page().LoadControl("~/UI/Admin/Sales/Invoices/Controls/InvoiceEventReport.ascx");
            var invoiceNumber = report.LoadReport(invoice, payment, registrations);

            var siteContent = new StringBuilder();
            using (var stringWriter = new StringWriter(siteContent))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    report.RenderControl(htmlWriter);
            }

            var date = DateTimeOffset.Now.FormatDateOnly(TimeZone);

            string headerTitle, footerTitle;
            switch (reportType)
            {
                case InvoiceEventReportType.Invoice:
                    headerTitle = $"Receipt: {invoiceNumber}";
                    footerTitle = "Receipt";
                    break;
                case InvoiceEventReportType.Receipt:
                    headerTitle = $"Receipt: {invoiceNumber}";
                    footerTitle = "Receipt";
                    break;
                default:
                    throw new ArgumentException("Unexpected report type: " + reportType);
            }

            var logoUrl = PortalHeader.GetLogoImageUrl(CurrentSessionState.Identity, System.Web.HttpContext.Current.Server);

            string absoluteLogoUrl = null;
            if (!string.IsNullOrEmpty(logoUrl))
                absoluteLogoUrl = HttpRequestHelper.GetAbsoluteUrl(logoUrl);

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                PageOrientation = PageOrientationType.Portrait,
                Viewport = new HtmlConverterSettings.ViewportSize(1400, 980),
                MarginTop = 32,
                MarginBottom = 22,
                HeaderSpacing = 7,

                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Sales/Invoices/Controls/ReportHeader.html"),
                FooterUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Sales/Invoices/Controls/ReportFooter.html"),
                Variables = new HtmlConverterSettings.Variable[]
                {
                        new HtmlConverterSettings.Variable("header_title", headerTitle),
                        new HtmlConverterSettings.Variable("footer_title", footerTitle),
                        new HtmlConverterSettings.Variable("footer_date", date),
                        new HtmlConverterSettings.Variable("logo_url", absoluteLogoUrl ?? string.Empty),
                }
            };

            var fileName = GetFileName(reportType, invoice);

            return (HtmlConverter.HtmlToPdf(siteContent.ToString(), settings), fileName);
        }

        private static string GetFileName(InvoiceEventReportType reportType, VInvoice invoice)
        {
            var invoiceNumber = invoice.InvoiceNumber.HasValue
                ? invoice.InvoiceNumber.ToString()
                : invoice.InvoiceIdentifier.ToString();

            var filePrefix = reportType == InvoiceEventReportType.Receipt ? "receipt" : "invoice";
            var date = TimeZones.ConvertFromUtc(DateTimeOffset.UtcNow, TimeZone);
            return $"{filePrefix}-{invoiceNumber}-{date:yyyy}-{date:MM}-{date:dd}-{date:HH}-{date:mm}";
        }

        private string LoadReport(VInvoice invoice, QPayment payment, List<QRegistration> registrations)
        {
            PurchaseDate.Text = invoice.InvoiceSubmitted.FormatDateOnly(TimeZone, nullValue: string.Empty);
            OrderNumber.Text = Invoice.FormatInvoiceNumber(invoice.InvoiceNumber ?? 0);

            var @event = registrations.Count > 0 ? ServiceLocator.EventSearch.GetEvent(registrations.First().EventIdentifier, x => x.VenueLocation) : null;

            BindIssuedBy();

            if (payment != null)
                BindIssuedTo(payment.CreatedBy);

            BindPurchases(invoice.InvoiceIdentifier);
            BindEventDetail(@event);
            BindPayments(invoice, payment);

            return invoice.InvoiceNumber.HasValue
                ? Invoice.FormatInvoiceNumber(invoice.InvoiceNumber.Value)
                : invoice.InvoiceIdentifier.ToString();
        }

        private void BindIssuedBy()
        {
            var organization = CurrentSessionState.Identity.Organization;

            var issuedBy = new StringBuilder();
            issuedBy.AppendLine(organization.CompanyName);

            var custom = organization.PlatformCustomization;
            var location = custom.TenantLocation;
            issuedBy.Append($"<br>{location.Street}");
            issuedBy.Append($"<br>{location.City}, {location.Province} {location.PostalCode}");

            IssuedBy.Text = issuedBy.ToString();
        }

        private void BindIssuedTo(Guid paymentCreatedBy)
        {
            var createdByUser = ServiceLocator.UserSearch.GetUser(paymentCreatedBy);
            IssuedToPerson.Text = createdByUser?.FullName ?? paymentCreatedBy.ToString();

            var createdByPerson = ServiceLocator.PersonSearch.GetPerson(paymentCreatedBy, CurrentSessionState.Identity.Organization.Identifier, x => x.EmployerGroup.Addresses);
            var company = createdByPerson?.EmployerGroup;
            if (company == null)
                return;

            var addressText = ClassVenueAddressInfo.GetAddress(company.Addresses.FirstOrDefault(x => string.Equals(x.AddressType, AddressType.Billing.ToString(), StringComparison.OrdinalIgnoreCase)));
            if (string.IsNullOrEmpty(addressText))
            {
                addressText = ClassVenueAddressInfo.GetAddress(company.Addresses.FirstOrDefault(x => string.Equals(x.AddressType, AddressType.Shipping.ToString(), StringComparison.OrdinalIgnoreCase)));
                if (string.IsNullOrEmpty(addressText))
                    addressText = ClassVenueAddressInfo.GetAddress(company.Addresses.FirstOrDefault(x => string.Equals(x.AddressType, AddressType.Physical.ToString(), StringComparison.OrdinalIgnoreCase)));
            }

            IssuedToPersonEmployer.Text = string.IsNullOrEmpty(addressText)
                ? company.GroupName
                : $"{company.GroupName}<br/>{addressText}";
        }

        private void BindPurchases(Guid invoiceIdentifier)
        {
            var items = ServiceLocator.InvoiceSearch.GetInvoiceItems(invoiceIdentifier, x => x.Product);

            var dataSource = items
                .OrderBy(x => x.ItemSequence)
                .Select(x => new
                {
                    Description = !string.IsNullOrEmpty(x.ItemDescription) ? x.ItemDescription : x.Product.ProductName,
                    Quantity = x.ItemQuantity,
                    Price = x.ItemPrice < 0 ? $"({-x.ItemPrice:c2})" : $"{x.ItemPrice:c2}",
                    Amount = x.ItemPrice < 0 ? $"({-x.ItemPrice * x.ItemQuantity:c2})" : $"{x.ItemPrice * x.ItemQuantity:c2}"
                })
                .ToList();

            PurchaseRepeater.DataSource = dataSource;
            PurchaseRepeater.DataBind();

            decimal subtotal = 0m;
            decimal taxTotal = 0m;
            bool anyTaxed = false;

            foreach (var li in items)
            {
                var lineBase = li.ItemPrice * li.ItemQuantity;
                subtotal += lineBase;

                var rate = li.TaxRate ?? 0m;
                if (rate > 0m) anyTaxed = true;

                var lineTax = Math.Round(lineBase * rate, 2, MidpointRounding.AwayFromZero);
                taxTotal += lineTax;
            }

            TaxBlock.Visible = anyTaxed;
            if (anyTaxed)
                TaxAmount.Text = ToCad(taxTotal);

            var grandTotal = subtotal + taxTotal;
            TotalPayment.Text = ToCad(grandTotal);
        }

        private static string ToCad(decimal amount) =>
            amount < 0 ? $"(CAD$ {-amount:n2})" : $"CAD$ {amount:n2}";

        private void BindEventDetail(QEvent @event)
        {
            EventDetails.Visible = @event != null;

            if (@event == null)
                return;

            var content = ContentEventClass.Deserialize(@event.Content);
            var text = content?.Get(EventInstructionType.Contact.GetName())?.Default;

            ContactAndSupportRow.Visible = text.IsNotEmpty();
            ContactAndSupport.Text = Markdown.ToHtml(text);

            EventTitle.Text = @event.EventTitle;

            EventDate.Text = @event.EventScheduledEnd.HasValue
                ? (
                    @event.EventScheduledEnd.Value.Date != @event.EventScheduledStart.Date
                        ? $"{@event.EventScheduledStart.Format(TimeZone)} - {@event.EventScheduledEnd.Value.Format(TimeZone)}"
                        : $"{@event.EventScheduledStart.FormatDateOnly(TimeZone)} {@event.EventScheduledStart.FormatTimeOnly(TimeZone)} - {@event.EventScheduledEnd.Value.FormatTimeOnly(TimeZone)}"
                )
                : $"{@event.EventScheduledStart.Format(TimeZone)}";

            VenuePanel.Visible = @event.VenueLocation != null;

            if (@event.VenueLocation != null)
            {
                VenueName.Text = @event.VenueLocation.GroupName;

                var address = ServiceLocator.GroupSearch.GetAddress(@event.VenueLocationIdentifier.Value, AddressType.Physical);

                if (address != null)
                    VenueAddress.Text = ClassVenueAddressInfo.GetAddress(address);
            }
        }

        private void BindPayments(VInvoice invoice, QPayment payment)
        {
            if (payment == null)
                return;

            var payments = new[] { new
            {
                PaymentDate = payment.PaymentStarted.HasValue
                                ? payment.PaymentStarted.Value.FormatDateOnly(TimeZone)
                                : "Unknown",
                PaidBy = GetPaidBy(payment),
                Notes = GetPaymentNotes(invoice, payment),
                PaymentAmount = GetPaymentAmount(payment)
            }};

            PaymentRepeater.DataSource = payments;
            PaymentRepeater.DataBind();
        }

        private string GetPaidBy(QPayment payment)
        {
            var userName = payment.CardholderName.IfNullOrEmpty("Unknown");

            return !string.IsNullOrEmpty(payment.CardNumber)
                ? $"{userName}<br/>{payment.CardNumber}"
                : userName;
        }

        private string GetPaymentNotes(VInvoice invoice, QPayment payment)
        {
            if (invoice.ReferencedInvoiceIdentifier == null)
                return $"Online Payment<br />Transaction: {payment.TransactionId}";

            var referencedEvent = GetReferencedEvent(invoice.ReferencedInvoiceIdentifier.Value);

            return referencedEvent != null
                ? $"Moved from {referencedEvent.EventTitle}"
                : "Moved from another class";
        }

        private QEvent GetReferencedEvent(Guid referencedInvoiceIdentifier)
        {
            var referencedRegistration = GetReferencedRegistration(referencedInvoiceIdentifier);

            return referencedRegistration != null
                ? ServiceLocator.EventSearch.GetEvent(referencedRegistration.EventIdentifier)
                : null; ;
        }

        private static QRegistration GetReferencedRegistration(Guid referencedInvoiceIdentifier)
        {
            var referencedPayments = ServiceLocator.PaymentSearch.GetPayments(new QPaymentFilter
            {
                InvoiceIdentifier = referencedInvoiceIdentifier,
                PaymentStatus = "Completed"
            });

            if (referencedPayments.Count == 0)
                return null;

            return ServiceLocator.RegistrationSearch.GetRegistration(new QRegistrationFilter
            {
                PaymentIdentifier = referencedPayments[0].PaymentIdentifier
            });
        }

        private string GetPaymentAmount(QPayment payment)
        {
            return payment.PaymentAmount < 0
                ? $"(CAD$ {-payment.PaymentAmount:n2})"
                : $"CAD$ {payment.PaymentAmount:n2}";
        }
    }
}