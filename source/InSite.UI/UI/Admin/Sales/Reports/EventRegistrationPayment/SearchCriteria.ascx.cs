using System;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Sales.Reports.EventRegistrationPayment
{
    public partial class SearchCriteria : SearchCriteriaController<VEventRegistrationPaymentFilter>
    {
        public override VEventRegistrationPaymentFilter Filter
        {
            get
            {
                var filter = new VEventRegistrationPaymentFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,

                    EventDateSince = EventDateSince.Value,
                    EventDateBefore = EventDateBefore.Value,
                    EventName = EventName.Text,
                    EmployerName = EmployerName.Text,
                    RegistrantName = RegistrantName.Text,
                    LearnerName = LearnerName.Text,
                    LearnerCode = LearnerCode.Text,
                    InvoiceNumber = InvoiceNumber.ValueAsInt,
                    PaymentStatus = PaymentStatus.Text,
                    InvoiceSubmittedSince = InvoiceSubmittedSince.Value,
                    InvoiceSubmittedBefore = InvoiceSubmittedBefore.Value,
                    PaymentApprovedSince = PaymentApprovedSince.Value,
                    PaymentApprovedBefore = PaymentApprovedBefore.Value,
                    PaymentTransactionId = PaymentTransactionId.Text,
                    AchievementTitle = AchievementName.Text,
                    InvoiceStatus = InvoiceStatus.Value

                };

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                EventDateSince.Value = value.EventDateSince;
                EventDateBefore.Value = value.EventDateBefore;
                EventName.Text = value.EventName;
                EmployerName.Text = value.EmployerName;
                RegistrantName.Text = value.RegistrantName;
                LearnerName.Text = value.LearnerName;
                LearnerCode.Text = value.LearnerCode;
                InvoiceNumber.ValueAsInt = value.InvoiceNumber;
                PaymentStatus.Text = value.PaymentStatus;
                InvoiceSubmittedSince.Value = value.InvoiceSubmittedSince;
                InvoiceSubmittedBefore.Value = value.InvoiceSubmittedBefore;
                PaymentApprovedSince.Value = value.PaymentApprovedSince;
                PaymentApprovedBefore.Value = value.PaymentApprovedBefore;
                PaymentTransactionId.Text = value.PaymentTransactionId;
                AchievementName.Text = value.AchievementTitle;
                InvoiceStatus.Value = value.InvoiceStatus;

                SortColumns.Value = value.OrderBy;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LearnerCode.EmptyMessage = GetEmptyMessage("Person Code");
        }

        public override void Clear()
        {
            EventDateSince.Value = null;
            EventDateBefore.Value = null;
            EventName.Text = null;
            EmployerName.Text = null;
            RegistrantName.Text = null;
            LearnerName.Text = null;
            LearnerCode.Text = null;
            InvoiceNumber.ValueAsInt = null;
            PaymentStatus.Text = null;
            InvoiceSubmittedSince.Value = null;
            InvoiceSubmittedBefore.Value = null;
            PaymentApprovedSince.Value = null;
            PaymentApprovedBefore.Value = null;
            PaymentTransactionId.Text = null;
            AchievementName.Text = null;
            InvoiceStatus.Value = null;
        }

        protected static string GetEmptyMessage(string text) => LabelHelper.GetLabelContentText(text);
    }
}