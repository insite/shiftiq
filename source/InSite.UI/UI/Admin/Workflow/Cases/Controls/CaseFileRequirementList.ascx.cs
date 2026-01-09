using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Cases.Write;
using InSite.Common.Web.UI;

using Shift.Common;

using PortalHelper = InSite.UI.Portal.Issues.Controls.CaseFileRequirementList;

namespace InSite.UI.Admin.Issues.Outlines.Controls
{
    public partial class CaseFileRequirementList : BaseUserControl
    {
        public event EventHandler Changed;

        private Guid IssueIdentifier
        {
            get => (Guid)ViewState[nameof(IssueIdentifier)];
            set => ViewState[nameof(IssueIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ListRepeater.ItemCommand += ListRepeater_ItemCommand;

            FileUpload.FileUploaded += FileUpload_FileUploaded;
        }

        private void ListRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete")
                return;

            var requestedFileCategory = (string)e.CommandArgument;
            var command = new DeleteFileRequirement(IssueIdentifier, requestedFileCategory);

            ServiceLocator.SendCommand(command);

            Changed?.Invoke(this, new EventArgs());
        }

        private void FileUpload_FileUploaded(object sender, EventArgs e)
        {
            if (!FileUpload.HasFile)
                throw new ArgumentException("No file");

            if (!int.TryParse(DocumentTypeIndex.Value, out var documentTypeIndex))
                throw new ArgumentException($"DocumentTypeIndex.Value is not valid integer: {DocumentTypeIndex.Value}");

            if (documentTypeIndex < 0 || documentTypeIndex >= ListRepeater.Items.Count)
                throw new ArgumentException($"DocumentTypeIndex {documentTypeIndex} is not valid index. Number of documentTypes is {ListRepeater.Items.Count}");

            var uploadButton = (IconButton)ListRepeater.Items[documentTypeIndex].FindControl("UploadButton");
            var documentType = uploadButton.CommandArgument;

            PortalHelper.CompleteRequest(IssueIdentifier, documentType, FileUpload);

            Changed?.Invoke(this, new EventArgs());
        }

        public bool BindIssueRequests(Guid issueIdentifier)
        {
            IssueIdentifier = issueIdentifier;

            var requests = ServiceLocator.IssueSearch.GetFileRequirements(issueIdentifier);

            var items = requests
                .Select(x => new
                {
                    DocumentType = x.RequestedFileCategory,
                    DocumentSubType = x.RequestedFileSubcategory,
                    RequestedTime = x.RequestedTime.Format(User.TimeZone, true),
                    RequestedUserName = x.RequestedUserName,
                    RequestedFrom = x.RequestedFrom,
                    Description = x.RequestedFileDescription,
                    ModifyUrl = $"/ui/admin/workflow/attachments/request?case={issueIdentifier}&category={HttpUtility.UrlEncode(x.RequestedFileCategory)}"
                })
                .ToList();

            ListRepeater.DataSource = items;
            ListRepeater.DataBind();

            return items.Count > 0;
        }
    }
}