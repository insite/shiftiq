using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Cases.Write;
using InSite.Application.Files.Read;
using InSite.Common.Web.UI;
using InSite.Web.Helpers;

namespace InSite.UI.Portal.Issues.Controls
{
    public partial class CaseFileRequirementList : BaseUserControl
    {
        public event EventHandler FileUploaded;

        private Guid IssueIdentifier
        {
            get => (Guid)ViewState[nameof(IssueIdentifier)];
            set => ViewState[nameof(IssueIdentifier)] = value;
        }

        private string[] DocumentTypes
        {
            get => (string[])ViewState[nameof(DocumentTypes)];
            set => ViewState[nameof(DocumentTypes)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FileUpload.FileUploaded += FileUpload_FileUploaded;
        }

        private void FileUpload_FileUploaded(object sender, EventArgs e)
        {
            if (!FileUpload.HasFile)
                throw new ArgumentException("No file");

            var documentType = HttpUtility.UrlDecode(DocumentTypeId.Value);
            if (!DocumentTypes.Any(x => string.Equals(x, documentType, StringComparison.OrdinalIgnoreCase)))
                return;

            CompleteRequest(IssueIdentifier, documentType, FileUpload);

            FileUploaded?.Invoke(this, new EventArgs());
        }

        public bool BindIssueRequests(Guid issueIdentifier)
        {
            IssueIdentifier = issueIdentifier;

            var requests = ServiceLocator.IssueSearch.GetFileRequirements(issueIdentifier);

            var items = requests
                .Where(x => string.Equals(x.RequestedFrom, "Candidate", StringComparison.OrdinalIgnoreCase))
                .Select(x => new
                {
                    Id = HttpUtility.UrlEncode(x.RequestedFileCategory),
                    DocumentType = x.RequestedFileCategory,
                    DocumentSubtype = x.RequestedFileSubcategory,
                    Status = x.RequestedFileStatus,
                    RequestedTime = x.RequestedTime,
                    RequestedFromCandidate = string.Equals(x.RequestedFrom, "Candidate", StringComparison.OrdinalIgnoreCase)
                })
                .ToList();

            DocumentTypes = items.Select(x => x.DocumentType).ToArray();

            ListRepeater.DataSource = items;
            ListRepeater.DataBind();

            return items.Count > 0;
        }

        internal static void CompleteRequest(Guid issueIdentifier, string documentType, FileUploadV2 fileUpload)
        {
            var fileRequirement = ServiceLocator.IssueSearch.GetFileRequirement(issueIdentifier, documentType);
            if (fileRequirement == null)
                return;

            var fileName = GetFileName(issueIdentifier, fileUpload.FileName);
            var fileType = Path.GetExtension(fileName);
            var posted = DateTimeOffset.UtcNow;
            var poster = User.UserIdentifier;

            var model = fileUpload.SaveFile(issueIdentifier, FileObjectType.Issue);

            model.Properties.DocumentName = fileName;
            model.Properties.Category = fileRequirement.RequestedFileCategory;
            model.Properties.Subcategory = fileRequirement.RequestedFileSubcategory;
            model.Properties.Description = fileRequirement.RequestedFileDescription;
            model.Properties.AllowLearnerToView = CaseAttachmentHelper.AllowLearnerToViewByIssue(issueIdentifier);

            ServiceLocator.StorageService.ChangeProperties(model.FileIdentifier, User.Identifier, model.Properties, false);

            CaseDocumentList.ApplyPermissionRule(model, Organization.Toolkits.Issues?.PortalUploadClaimGroups);

            var command = new CompleteFileRequirement(issueIdentifier, documentType, fileName, fileType, model.FileIdentifier, posted, poster);
            ServiceLocator.SendCommand(command);
        }

        private static string GetFileName(Guid issueIdentifier, string fileName)
        {
            const int MaxFileLength = 200;

            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            var index = 1;

            while (ServiceLocator.IssueSearch.GetAttachment(issueIdentifier, fileName) != null)
            {
                var prefix = "_" + index;
                var delta = fileNameWithoutExt.Length + prefix.Length + ext.Length - MaxFileLength;

                var fileNameWithoutExtCurrent = delta > 0
                    ? fileNameWithoutExt.Substring(0, fileNameWithoutExt.Length - delta)
                    : fileNameWithoutExt;

                fileName = $"{fileNameWithoutExtCurrent}{prefix}{ext}";

                index++;
            }

            return fileName;
        }
    }
}