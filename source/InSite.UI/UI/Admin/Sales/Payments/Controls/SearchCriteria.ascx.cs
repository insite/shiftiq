using InSite.Application.Payments.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Payments.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QPaymentFilter>
    {
        public override QPaymentFilter Filter
        {
            get
            {
                var filter = new QPaymentFilter
                {
                    OrganizationIdentifier = Organization.Identifier,

                    MinAmount = MinAmount.ValueAsInt,
                    MaxAmount = MaxAmount.ValueAsInt,
                    PaymentStatus = PaymentStatus.Value,
                    PaymentApprovedSince = PaymentApprovedSince.Value,
                    PaymentApprovedBefore = PaymentApprovedBefore.Value,
                    PaymentStartedSince = PaymentStartedSince.Value,
                    PaymentStartedBefore = PaymentStartedBefore.Value,
                    PaymentDeclinedSince = PaymentDeclinedSince.Value,
                    PaymentDeclinedBefore = PaymentDeclinedBefore.Value,
                    PaymentAbortedSince = PaymentAbortedSince.Value,
                    PaymentAbortedBefore = PaymentAbortedBefore.Value,
                    CustomerName = CustomerName.Text,
                    CustomerEmail = CustomerEmail.Text,
                    InvoiceNumber = InvoiceNumber.ValueAsInt,
                    CustomerEmployer = CustomerEmployer.Text,
                    TransactionIdentifier = TransactionID.Text,

                    ProductIdentifier = Product.Value,

                    ExcludeBrokenReferences = true
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                MinAmount.ValueAsInt = value.MinAmount;
                MaxAmount.ValueAsInt = value.MaxAmount;
                PaymentStatus.Value = value.PaymentStatus;

                CustomerName.Text = value.CustomerName;
                CustomerEmail.Text = value.CustomerEmail;
                CustomerEmployer.Text = value.CustomerEmployer;

                PaymentApprovedSince.Value = value.PaymentApprovedSince;
                PaymentApprovedBefore.Value = value.PaymentApprovedBefore;
                PaymentStartedSince.Value = value.PaymentStartedSince;
                PaymentStartedBefore.Value = value.PaymentStartedBefore;
                PaymentDeclinedSince.Value = value.PaymentDeclinedSince;
                PaymentDeclinedBefore.Value = value.PaymentDeclinedBefore;
                PaymentAbortedSince.Value = value.PaymentAbortedSince;
                PaymentAbortedBefore.Value = value.PaymentAbortedBefore;

                InvoiceNumber.ValueAsInt = value.InvoiceNumber;
                TransactionID.Text = value.TransactionIdentifier;

                Product.Value = value.ProductIdentifier;
            }
        }

        public override void Clear()
        {
            MinAmount.ValueAsInt = null;
            MaxAmount.ValueAsInt = null;
            PaymentStatus.Value = null;

            CustomerName.Text = null;
            CustomerEmail.Text = null;
            CustomerEmployer.Text= null;

            PaymentApprovedSince.Value = null;
            PaymentApprovedBefore.Value = null;
            PaymentStartedSince.Value = null;
            PaymentStartedBefore.Value = null;
            PaymentDeclinedSince.Value = null;
            PaymentDeclinedBefore.Value = null;
            PaymentAbortedSince.Value = null;
            PaymentAbortedBefore.Value = null;

            InvoiceNumber.ValueAsInt = null;
            TransactionID.Text = null;

            Product.Value = null;
        }
    }
}