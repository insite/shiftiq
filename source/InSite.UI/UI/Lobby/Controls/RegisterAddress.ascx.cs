using System.Web.UI;

using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Lobby.Controls
{
    public partial class RegisterAddress : UserControl
    {
        public void SetDefaultInputValues()
        {
            Country.Value = "Canada";

            Province.Country = Country.Value;
            Province.RefreshData();
        }

        internal void SetupFieldsVisibility(LocationFieldMask mask)
        {
            DescriptionField.Visible = mask.Description;
            PhoneField.Visible = mask.Phone;
            MobileField.Visible = mask.Mobile;
            Address1Field.Visible = mask.Address1;
            Address2Field.Visible = mask.Address2;
            CityField.Visible = mask.City;
            CountryField.Visible = mask.Country;
            ProvinceField.Visible = mask.Province;
            PostalCodeField.Visible = mask.PostalCode;
        }

        public void GetInputValues(Address address, out string phone, out string mobile)
        {
            address.Description = Description.Text;
            address.Street1 = Street1.Text;
            address.Street2 = Street2.Text;
            address.City = City.Text;
            address.PostalCode = PostalCode.Text;
            address.Province = Province.Value.IfNullOrEmpty(Province.GetSelectedOption().Text);
            address.Country = Country.Value;

            phone = Shift.Common.Phone.Format(Phone.Text);
            mobile = Shift.Common.Phone.Format(Mobile.Text);
        }
    }
}