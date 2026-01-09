using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using InSite.Common;
using InSite.Domain.Payments;
using InSite.Web.Helpers;

namespace InSite.UI.Portal.Billing.Controls
{
    public partial class CardDetail : UserControl
    {
        private static readonly PortalFieldHandler<HtmlGenericControl> _fieldHandler = new PortalFieldHandler<HtmlGenericControl>();

        public void GetInputValues(UnmaskedCreditCard card, BillingAddress billing)
        {
            CreditCardCreator.Create(card, CardholderName.Text, CardNumber.Text, Expiry.Text, SecurityCode.Text, LabelHelper.GetTranslation);

            GetAddressValues(billing);
        }

        private void GetAddressValues(BillingAddress billing)
        {
            if (ContactNameField.Visible)
                billing.Name = ContactName.Text;

            if (ContactEmailField.Visible)
                billing.Email = ContactEmail.Text;

            if (ContactPhoneField.Visible)
                billing.Phone = ContactPhone.Text;

            if (ContactAddressField.Visible)
                billing.Address = ContactAddress.Text;

            if (ContactCityField.Visible)
                billing.City = ContactCity.Text;

            if (ContactProvinceField.Visible)
                billing.Province = ContactProvince.Value;

            if (ContactPostalCodeField.Visible)
                billing.PostalCode = ContactPostalCode.Text;

            if (ContactCountryField.Visible)
                billing.Country = ContactCountry.Value;
        }

        public void Clear()
        {
            InitAddressFields();

            CardholderName.Text = null;
            CardNumber.Text = null;
            Expiry.Text = null;
            SecurityCode.Text = null;

            ContactName.Text = null;
            ContactEmail.Text = null;
            ContactPhone.Text = null;

            ContactAddress.Text = null;
            ContactCity.Text = null;
            ContactProvince.Value = null;
            ContactPostalCode.Text = null;
            ContactCountry.ClearSelection();
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
    }
}