using InSite.Common.Web.UI;
using InSite.Domain.Organizations;

namespace InSite.UI.Admin.Accounts.Organizations.Controls
{
    public partial class DetailConfigurationCustomizationUrl : BaseUserControl
    {
        public void SetInputValues(OrganizationState organization)
        {
            var custom = organization.PlatformCustomization;
            var platformUrl = custom.PlatformUrl;

            SupportUrl.Text = platformUrl.Support;
            ContactUrl.Text = platformUrl.Contact;

            LogoUrl.Text = platformUrl.Logo;
            UploadLogo.NavigateUrl = $"/ui/admin/accounts/organizations/upload?organization={organization.OrganizationIdentifier}";

            WallpaperUrl.Text = platformUrl.Wallpaper;
            UploadWallpaper.NavigateUrl = $"/ui/admin/accounts/organizations/upload?organization={organization.OrganizationIdentifier}";

            var organizationUrl = custom.TenantUrl;
            FacebookUrl.Text = organizationUrl.Facebook;
            TwitterUrl.Text = organizationUrl.Twitter;
            LinkedInUrl.Text = organizationUrl.LinkedIn;
            InstagramUrl.Text = organizationUrl.Instagram;
            YouTubeUrl.Text = organizationUrl.YouTube;
            OtherUrl.Text = organizationUrl.Other;
        }

        public void GetInputValues(OrganizationState organization)
        {
            var custom = organization.PlatformCustomization;

            var platformUrl = custom.PlatformUrl;
            platformUrl.Logo = LogoUrl.Text;
            platformUrl.Wallpaper = WallpaperUrl.Text;
            platformUrl.Support = SupportUrl.Text;
            platformUrl.Contact = ContactUrl.Text;

            var organizationUrl = custom.TenantUrl;
            organizationUrl.Facebook = FacebookUrl.Text;
            organizationUrl.Twitter = TwitterUrl.Text;
            organizationUrl.LinkedIn = LinkedInUrl.Text;
            organizationUrl.Instagram = InstagramUrl.Text;
            organizationUrl.YouTube = YouTubeUrl.Text;
            organizationUrl.Other = OtherUrl.Text;
        }
    }
}