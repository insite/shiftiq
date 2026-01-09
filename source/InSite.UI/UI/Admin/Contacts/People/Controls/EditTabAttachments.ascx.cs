using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Files.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Contacts.People.Controls
{
    public partial class EditTabAttachments : BaseUserControl
    {
        public void SetInputValues(Guid userIdentifier)
        {
            var enableWorkflowManagment = Organization.Toolkits?.Issues?.EnableWorkflowManagement ?? false;

            var (images, documents) = GetAttachments(userIdentifier);

            DocumentsCard.Visible = DocumentList.BindFiles(documents);
            ImagesCard.Visible = ImageList.BindFiles(images);
            ResponsesCard.Visible = ResponseList.BindResponseFiles(userIdentifier);
            IssuesCard.Visible = enableWorkflowManagment && IssueList.BindIssueFiles(userIdentifier);

            NoAttachments.Visible = !DocumentsCard.Visible
                && !ImagesCard.Visible
                && !ResponsesCard.Visible
                && !IssuesCard.Visible;

            UploadFileButton.NavigateUrl = $"/ui/admin/assets/files/upload?user={userIdentifier}";
        }

        private (List<FileStorageModel>, List<FileStorageModel>) GetAttachments(Guid userIdentifier)
        {
            var imageExtensions = new[] { ".gif", ".jpg", ".jpeg", ".png" };
            var documentExtensions = new[] { ".doc", ".docx", ".pdf", ".zip", ".rtf" };

            var files = ServiceLocator.StorageService.GetGrantedFiles(Identity, userIdentifier);

            var images = new List<FileStorageModel>();
            var documents = new List<FileStorageModel>();

            foreach (var file in files)
            {
                if (imageExtensions.Any(x => file.FileName.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                    images.Add(file);

                if (documentExtensions.Any(x => file.FileName.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                    documents.Add(file);
            }

            return (images, documents);
        }
    }
}