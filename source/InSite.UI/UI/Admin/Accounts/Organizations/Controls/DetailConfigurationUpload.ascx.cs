using InSite.Common.Web.UI;
using InSite.Domain.Organizations;

namespace InSite.UI.Admin.Accounts.Organizations.Controls
{
    public partial class DetailConfigurationUpload : BaseUserControl
    {
        public void SetInputValues(OrganizationState organization)
        {
            var settings = organization.PlatformCustomization.UploadSettings;

            UploadImageMaxFileSize.ValueAsInt = settings.Images.MaximumFileSize;
            UploadImageMaxHeight.ValueAsInt = settings.Images.MaximumHeight;
            UploadImageMaxWidth.ValueAsInt = settings.Images.MaximumWidth;
            UploadDocumentMaxFileSize.ValueAsInt = settings.Documents.MaximumFileSize;
        }

        public void GetInputValues(OrganizationState organization)
        {
            var settings = organization.PlatformCustomization.UploadSettings;

            if (UploadImageMaxFileSize.ValueAsInt.HasValue)
                settings.Images.MaximumFileSize = UploadImageMaxFileSize.ValueAsInt.Value;

            if (UploadImageMaxHeight.ValueAsInt.HasValue)
                settings.Images.MaximumHeight = UploadImageMaxHeight.ValueAsInt.Value;

            if (UploadImageMaxWidth.ValueAsInt.HasValue)
                settings.Images.MaximumWidth = UploadImageMaxWidth.ValueAsInt.Value;

            if (UploadDocumentMaxFileSize.ValueAsInt.HasValue)
                settings.Documents.MaximumFileSize = UploadDocumentMaxFileSize.ValueAsInt.Value;
        }
    }
}