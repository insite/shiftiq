using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using InSite.Domain.Payments;
using InSite.Web.Helpers;

using Shift.Common;

namespace InSite.UI.Portal.Billing.Controls
{
    public partial class CardDetailConfirm : UserControl
    {
        private static readonly PortalFieldHandler<HtmlGenericControl> _fieldHandler = new PortalFieldHandler<HtmlGenericControl>(InitFields);

        public void GetInputValues(UnmaskedCreditCard card, BillingAddress billing)
        {
            card.CardholderName = CardholderName.Text;
            card.CardNumber = CardNumber.Text;

            var expiryDate = CreditCardExpiry.Parse(Expiry.Text);
            card.ExpiryMonth = expiryDate.Month;
            card.ExpiryYear = expiryDate.Year;

            card.SecurityCode = SecurityCode.Text;

            if (BillingName.Visible)
                billing.Name = BillingName.Text.NullIfEmpty();

            if (BillingEmail.Visible)
                billing.Email = BillingEmail.Text.NullIfEmpty();

            if (BillingPhone.Visible)
                billing.Phone = BillingPhone.Text.NullIfEmpty();

            if (BillingAddress.Visible)
                billing.Address = BillingAddress.Text.NullIfEmpty();

            if (BillingCity.Visible)
                billing.City = BillingCity.Text.NullIfEmpty();

            if (BillingProvince.Visible)
                billing.Province = BillingProvince.Text.NullIfEmpty();

            if (BillingPostalCode.Visible)
                billing.PostalCode = BillingPostalCode.Text.NullIfEmpty();

            if (BillingCountry.Visible)
                billing.Country = BillingCountry.Text.NullIfEmpty();
        }

        public void SetInputValues(UnmaskedCreditCard card, BillingAddress billing)
        {
            InitAddressFields();

            CardholderName.Text = card.CardholderName;
            CardNumber.Text = card.CardNumber;
            Expiry.Text = $"{card.ExpiryMonth:00}/{card.ExpiryYear:00}";
            SecurityCode.Text = card.SecurityCode;

            BillingName.Text = billing.Name;
            BillingEmail.Text = billing.Email;
            BillingPhone.Text = billing.Phone;

            BillingAddress.Text = billing.Address;
            BillingCity.Text = billing.City;
            BillingProvince.Text = billing.Province;
            BillingPostalCode.Text = billing.PostalCode;
            BillingCountry.Text = billing.Country;
        }

        private void InitAddressFields()
        {
            var isAddressVisible = false;
            var organizationFields = CurrentSessionState.Identity.Organization.Fields.InvoiceBillingAddress;

            foreach (var defaultField in PortalFieldInfo.InvoiceBillingAddress)
            {
                var orgField = organizationFields?.FirstOrDefault(x => x.FieldName == defaultField.FieldName);
                var field = _fieldHandler.Init(AddressPanel, defaultField, orgField, null);
                if (field.IsVisible && !isAddressVisible)
                    isAddressVisible = true;
            }

            AddressPanel.Visible = isAddressVisible;
        }

        private static void InitFields(PortalFieldHandler<HtmlGenericControl>.IFieldData f)
        {
            var fieldId = $"{f.Name}Field";
            var fieldControl = f.Container.FindControl(fieldId);

            if (fieldControl == null)
                throw ApplicationError.Create($"Field control not found: " + fieldId);

            fieldControl.Visible = f.IsVisible;
        }
    }
}