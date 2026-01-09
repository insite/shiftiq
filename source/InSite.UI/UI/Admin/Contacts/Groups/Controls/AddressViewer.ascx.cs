using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Web.Helpers;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.Groups.Controls
{
    public partial class AddressViewer : UserControl
    {
        private const string None = "<i>None</i>";

        public void SetInputValues(string type, QGroupAddress address)
        {
            if (address == null)
            {
                address = new QGroupAddress();
                address.AddressIdentifier = UniqueIdentifier.Create();
            }

            if (type != null)
                AddressHeading.InnerText = ((IAdminPage)Page).Translator.Translate($"{type} Address");

            Description.Text = address.Description.IfNullOrEmpty(None);
            Street1.Text = address.Street1.IfNullOrEmpty(None);
            Street2.Text = address.Street2.IfNullOrEmpty(None);

            City.Text = address.City.IfNullOrEmpty(None);
            Province.Text = address.Province.IfNullOrEmpty(None);
            PostalCode.Text = address.PostalCode.IfNullOrEmpty(None);
            Country.Text = address.Country.IfNullOrEmpty(None);

            var gMapUrl = address.GetGMapsAddressLink();
            MapURL.Visible = gMapUrl.IsNotEmpty();
            MapURL.NavigateUrl = gMapUrl;

            SetNamesPerCountry();
        }

        private void SetNamesPerCountry()
        {
            if (Country.Text == "United States")
            {
                ProvinceFieldLabel.InnerHtml = "State";
                PostalCodeFieldLabel.InnerHtml = "ZIP Code";
            }
            else if (Country.Text == "Canada")
            {
                ProvinceFieldLabel.InnerHtml = "Province";
                PostalCodeFieldLabel.InnerHtml = "Postal Code";
            }
            else
            {
                ProvinceFieldLabel.InnerHtml = "State/Province";
                PostalCodeFieldLabel.InnerHtml = "Zip/Postal Code";
            }
        }
    }
}