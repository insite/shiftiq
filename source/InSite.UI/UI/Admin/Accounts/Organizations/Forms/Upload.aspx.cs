using System;
using System.IO;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.File;

namespace InSite.Admin.Accounts.Organizations.Forms
{
    public partial class Upload : AdminBasePage, IHasParentLinkParameters
    {
        private Guid OrganizationIdentifier => Guid.TryParse(Request["organization"], out var id) ? id : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;

            LogoFileRefreshButton.Click += LogoFileRefreshButton_Click;
            WallpaperFileRefreshButton.Click += WallpaperFileRefreshButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }

        private void LogoFileRefreshButton_Click(object sender, EventArgs e)
        {
            SaveFile("logos", LogoFileInput, LogoThumbnail);
        }

        private void WallpaperFileRefreshButton_Click(object sender, EventArgs e)
        {
            SaveFile("wallpapers", WallpaperFileInput, WallpaperThumbnail);
        }

        private void Open()
        {
            var organization = OrganizationSearch.Select(OrganizationIdentifier);
            if (organization == null)
                RedirectToSearch();

            SetInputValues(organization);
        }

        private void Save()
        {
            var organization = OrganizationSearch.Select(OrganizationIdentifier);
            if (organization == null)
                RedirectToSearch();

            var custom = organization.PlatformCustomization;
            var platformUrl = custom.PlatformUrl;

            platformUrl.Logo = LogoFileInput.InputText;
            platformUrl.Wallpaper = WallpaperFileInput.InputText;

            OrganizationStore.Update(organization);

            RedirectToParent();
        }

        private void SetInputValues(OrganizationState organization)
        {
            PageHelper.AutoBindHeader(Page, qualifier: organization.CompanyName);

            var domain = ServiceLocator.AppSettings.Security.Domain;
            var environment = ServiceLocator.AppSettings.Environment;

            var custom = organization.PlatformCustomization;
            var platformUrl = custom.PlatformUrl;
            var code = ServiceLocator.OrganizationSearch.Get(OrganizationIdentifier).OrganizationCode;

            var logoUrl = UrlHelper.GetAbsoluteUrl(domain, environment, platformUrl.Logo, code);
            LogoThumbnail.ImageUrl = logoUrl;
            LogoFileInput.MaxFileSize = Organization.PlatformCustomization.UploadSettings.Images.MaximumFileSize;
            LogoFileInput.InputText = platformUrl.Logo;

            var wallpaperUrl = UrlHelper.GetAbsoluteUrl(domain, environment, platformUrl.Wallpaper, code);
            WallpaperThumbnail.ImageUrl = wallpaperUrl;
            WallpaperFileInput.MaxFileSize = Organization.PlatformCustomization.UploadSettings.Images.MaximumFileSize;
            WallpaperFileInput.InputText = platformUrl.Wallpaper;
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/accounts/organizations/search");

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/accounts/organizations/edit?organization={OrganizationIdentifier}&panel=configuration&tab=url");

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"organization={OrganizationIdentifier}&panel=configuration&tab=url"
                : null;
        }

        private void SaveFile(string type, FileUploadV1 fileUpload, Image image)
        {
            var sourceFilePath = GetFilePath(fileUpload.Metadata);

            var fileName = Path.GetFileName(sourceFilePath);
            var fileNameWithoutExt = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(fileName), '-');
            var fileExt = Path.GetExtension(fileNameWithoutExt);
            var path = $"/images/{type}/{fileName}{fileExt}";
            var fileUrl = $"/files{path}";

            using (var stream = new FileStream(sourceFilePath, FileMode.Open))
                FileHelper.Provider.Save(OrganizationIdentifier, path, stream);

            fileUpload.InputText = fileUrl;

            var code = ServiceLocator.OrganizationSearch.Get(OrganizationIdentifier).OrganizationCode;
            var domain = ServiceLocator.AppSettings.Security.Domain;
            var environment = ServiceLocator.AppSettings.Environment;
            var absoluteUrl = UrlHelper.GetAbsoluteUrl(domain, environment, fileUrl, code);

            image.ImageUrl = absoluteUrl;
        }

        private static string GetFilePath(UploadMetadata metadata)
        {
            if (metadata == null || string.IsNullOrEmpty(metadata.FilePath))
                throw new ArgumentNullException(nameof(metadata));

            return metadata.FilePath;
        }
    }
}