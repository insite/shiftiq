using InSite.Common.Web.UI;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Accounts.Organizations.Controls
{
    public partial class DetailConfigurationLocationDescription : BaseUserControl
    {
        public void SetInputValues(OrganizationState organization)
        {
            var location = organization.PlatformCustomization.TenantLocation;
            LocationType.Value = location.LocationType.GetName();
            LocationDescription.Text = location.Description;
            LocationPhone.Text = Phone.Format(location.Phone);
            LocationTollFree.Text = location.TollFree;
            LocationEmail.Text = location.Email;
        }

        public void GetInputValues(OrganizationState organization)
        {
            var location = organization.PlatformCustomization.TenantLocation;
            location.LocationType = LocationType.Value.ToEnum<LocationType>();
            location.Description = LocationDescription.Text;
            location.Phone = LocationPhone.Text;
            location.TollFree = LocationTollFree.Text;
            location.Email = LocationEmail.Text;
        }
    }
}