using InSite.Common.Web.UI;
using InSite.Domain.Organizations;

namespace InSite.UI.Admin.Accounts.Organizations.Controls
{
    public partial class DetailConfigurationLocationAddress : BaseUserControl
    {
        public void SetInputValues(OrganizationState organization)
        {
            var location = organization.PlatformCustomization.TenantLocation;
            LocationStreet.Text = location.Street;
            LocationCity.Text = location.City;
            LocationProvince.Text = location.Province;
            LocationPostalCode.Text = location.PostalCode;
            LocationCountry.Text = location.Country;
        }

        public void GetInputValues(OrganizationState organization)
        {
            var location = organization.PlatformCustomization.TenantLocation;
            location.Street = LocationStreet.Text;
            location.City = LocationCity.Text;
            location.Province = LocationProvince.Text;
            location.PostalCode = LocationPostalCode.Text;
            location.Country = LocationCountry.Text;
        }

        public void EnableValidators(bool enable)
        {
            LocationStreetRequiredValidator.Visible = enable;
            LocationCityRequiredValidator.Visible = enable;
            LocationProvinceRequiredValidator.Visible = enable;
            LocationPostalCodeRequiredValidator.Visible = enable;
            LocationCountryRequiredValidator.Visible = enable;
        }
    }
}