using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Invoices.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VInvoiceFilter>
    {
        public override VInvoiceFilter Filter
        {
            get
            {
                var filter = new VInvoiceFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    CustomerName = EmployerContactName.Text,
                    CustomerEmail = CustomerEmail.Text,
                    InvoiceStatus = InvoiceStatus.Value,

                    InvoiceDraftedSince = InvoiceDraftedSince.Value,
                    InvoiceDraftedBefore = InvoiceDraftedBefore.Value,
                    InvoiceSubmittedSince = InvoiceSubmittedSince.Value,
                    InvoiceSubmittedBefore = InvoiceSubmittedBefore.Value,
                    InvoicePaidSince = InvoicePaidSince.Value,
                    InvoicePaidBefore = InvoicePaidBefore.Value,
                    InvoiceNumber = InvoiceNumber.ValueAsInt,
                    TransactionIdentifier = TransactionID.Text,

                    CustomerPersonCode = PersonCode.Text,
                    CustomerEmployer = Employer.Text,

                    ProductIdentifier = Product.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                EmployerContactName.Text = value.CustomerName;
                CustomerEmail.Text = value.CustomerEmail;
                InvoiceStatus.Value = value.InvoiceStatus;

                InvoiceDraftedSince.Value = value.InvoiceDraftedSince;
                InvoiceDraftedBefore.Value = value.InvoiceDraftedBefore;
                InvoiceSubmittedSince.Value = value.InvoiceSubmittedSince;
                InvoiceSubmittedBefore.Value = value.InvoiceSubmittedBefore;
                InvoicePaidSince.Value = value.InvoicePaidSince;
                InvoicePaidBefore.Value = value.InvoicePaidBefore;
                InvoiceNumber.ValueAsInt = value.InvoiceNumber;
                TransactionID.Text = value.TransactionIdentifier;

                PersonCode.Text = value.CustomerPersonCode;
                Employer.Text = value.CustomerEmployer;

                Product.Value = value.ProductIdentifier;
            }
        }

        public override void Clear()
        {
            EmployerContactName.Text = null;
            CustomerEmail.Text = null;
            InvoiceStatus.Value = null;

            InvoiceDraftedSince.Value = null;
            InvoiceDraftedBefore.Value = null;
            InvoiceSubmittedSince.Value = null;
            InvoiceSubmittedBefore.Value = null;
            InvoicePaidSince.Value = null;
            InvoicePaidBefore.Value = null;
            InvoiceNumber.ValueAsInt = null;
            TransactionID.Text = null;

            PersonCode.Text = null;
            Employer.Text = null;

            Product.Value = null;
        }
    }
}