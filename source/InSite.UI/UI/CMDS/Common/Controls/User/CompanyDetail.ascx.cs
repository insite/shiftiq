using System;
using System.IO;
using System.Web.UI;

using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.Cmds.Controls.Contacts.Companies
{
    public partial class CompanyDetails : UserControl
    {
        #region Constants

        private const string LogoFolderUrl = "~/library/platforms/cmds/logos/";

        #endregion

        #region Properties

        private Guid OrganizationIdentifier
        {
            get => (Guid)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LogoUpdatePanel.Request += LogoUpdatePanel_Request;

            RemoveLogoButton.Click += RemoveLogoButton_Click;
        }

        #endregion

        #region Public methods

        public void SetInputValues(OrganizationState organization)
        {
            LogoField.Visible = true;

            OrganizationIdentifier = organization.OrganizationIdentifier;

            CompanyName.Text = organization.CompanyDescription.LegalName;
            Acronym.Text = organization.CompanyName;
            OrganizationCode.Text = organization.OrganizationCode;

            Description.Text = organization.CompanyDescription.CompanySummary;

            CompanyAttachmentEditorLink.NavigateUrl = $"/ui/cmds/design/uploads/search?id={organization.OrganizationIdentifier}";
            RowAttachments.Visible = organization.OrganizationIdentifier != Guid.Empty;

            WebSiteUrl.Text = organization.CompanyDomain;

            EnableDivisions.Checked = OrganizationHelper.EnableDivisions(organization.CompanyDescription.CompanySize);

            SetupLogo(organization.PlatformCustomization.PlatformUrl.Logo);
        }

        public void GetInputValues(OrganizationState organization)
        {
            organization.CompanyDescription.LegalName = CompanyName.Text;
            organization.CompanyName = Acronym.Text;
            organization.OrganizationCode = OrganizationCode.Text;
            organization.CompanyDescription.CompanySummary = Description.Text;
            organization.CompanyDomain = WebSiteUrl.Text;
            organization.CompanyDescription.CompanySize = EnableDivisions.Checked ? CompanySize.Large : CompanySize.Medium;
        }

        #endregion

        #region Event handlers

        private void LogoUpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value != "upload" || !LogoUpload.HasFile)
                return;

            var folderPath = MapPath(LogoFolderUrl);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var metadata = LogoUpload.Metadata;
            var fileUrl = LogoFolderUrl + metadata.FileName;
            var filePath = MapPath(fileUrl);

            if (File.Exists(filePath))
                File.Delete(filePath);

            var organizationPacket = OrganizationSearch.Select(OrganizationIdentifier);

            if (!string.IsNullOrEmpty(organizationPacket.PlatformCustomization.PlatformUrl.Logo))
            {
                var oldFilePath = MapPath(organizationPacket.PlatformCustomization.PlatformUrl.Logo);

                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

            File.Copy(metadata.FilePath, filePath);

            organizationPacket.PlatformCustomization.PlatformUrl.Logo = fileUrl;

            OrganizationStore.Update(organizationPacket);

            SetupLogo(fileUrl);
        }

        private void RemoveLogoButton_Click(object sender, EventArgs e)
        {
            var organizationPacket = OrganizationSearch.Select(OrganizationIdentifier);

            if (string.IsNullOrEmpty(organizationPacket.PlatformCustomization.PlatformUrl.Logo))
                return;

            var filePath = MapPath(organizationPacket.PlatformCustomization.PlatformUrl.Logo);

            if (File.Exists(filePath))
                File.Delete(filePath);

            organizationPacket.PlatformCustomization.PlatformUrl.Logo = null;

            OrganizationStore.Update(organizationPacket);

            SetupLogo(null);
        }

        #endregion

        #region Helper methods

        private void SetupLogo(string imageUrl)
        {
            var hasLogo = !string.IsNullOrEmpty(imageUrl);

            LogoImage.Visible = hasLogo;
            UploadLogoButton.Visible = !hasLogo;
            ReplaceLogoButton.Visible = hasLogo;
            RemoveLogoButton.Visible = hasLogo;

            if (hasLogo)
                LogoImage.ImageUrl = imageUrl + "?" + UniqueIdentifier.Create();
        }

        #endregion
    }
}