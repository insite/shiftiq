using System;
using System.IO;
using System.Linq;

using Humanizer;

using InSite.Application.Cases.Write;
using InSite.Application.Files.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Issues.Outlines.Controls
{
    public partial class AttachmentsSection : BaseUserControl
    {
        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        private Guid IssueId
        {
            get => (Guid)ViewState[nameof(IssueId)];
            set => ViewState[nameof(IssueId)] = value;
        }

        private Guid? RespondentUserId
        {
            get => (Guid?)ViewState[nameof(RespondentUserId)];
            set => ViewState[nameof(RespondentUserId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CaseFileRequirementList.Changed += IssueFileRequirementList_Changed;

            CopyDocumentsCaseSelector.AutoPostBack = true;
            CopyDocumentsCaseSelector.ValueChanged += CopyDocumentsCaseSelector_ValueChanged;
        }

        private void CopyDocumentsCaseSelector_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            var issueId = e.NewValue;
            var attachmentIds = CaseDocumentList.GetSelectedAttachmentIds();

            CopyDocumentsCaseSelector.Value = null;
            CaseDocumentList.ClearSelectedAttachments();

            if (!issueId.HasValue || attachmentIds.IsEmpty())
            {
                OnAlert(AlertType.Warning, "Please select a target case and attachments to copy.");
                return;
            }

            var selectedAttachments = ServiceLocator.IssueSearch.GetAttachments(new QIssueAttachmentFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                AttachmentIdentifiers = attachmentIds
            });
            if (selectedAttachments.IsEmpty())
            {
                OnAlert(AlertType.Error, "Failed to retrieve the selected attachments. Please reload the page and try again.");
                return;
            }

            var existAttachments = ServiceLocator.IssueSearch.GetAttachments(new QIssueAttachmentFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                IssueIdentifier = issueId
            });
            var newAttachments = selectedAttachments
                .Where(x => !existAttachments
                    .Any(y => y.FileIdentifier == x.FileIdentifier
                           || y.FileName.Equals(x.FileName, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
            var copiedCount = 0;

            foreach (var a in newAttachments)
            {
                var (sourceModel, sourceStream) = ServiceLocator.StorageService.GetFileStream(a.FileIdentifier);
                if (sourceModel == null || sourceStream == Stream.Null)
                    continue;

                FileStorageModel copyModel;

                using (sourceStream)
                {
                    copyModel = ServiceLocator.StorageService.Create(
                        sourceStream,
                        sourceModel.FileName,
                        Organization.OrganizationIdentifier,
                        User.Identifier,
                        issueId.Value,
                        FileObjectType.Issue,
                        sourceModel.Properties,
                        sourceModel.Claims
                    );
                }

                ServiceLocator.SendCommand(new AddAttachment(
                    issueId.Value,
                    copyModel.Properties.DocumentName,
                    Path.GetExtension(copyModel.FileName),
                    copyModel.FileIdentifier,
                    a.FileUploaded,
                    User.UserIdentifier
                ));

                copiedCount++;
            }

            if (copiedCount > 0)
                OnAlert(AlertType.Success, "document".ToQuantity(copiedCount) + " has been successfully copied.");
            else
                OnAlert(AlertType.Information, "All selected documents are already attached to the target case.");
        }

        private void IssueFileRequirementList_Changed(object sender, EventArgs e)
        {
            BindModelToControlsInternal();
        }

        public void BindModelToControls(Guid issueIdentifier, Guid? respondentUserId, Guid? topictUserId)
        {
            IssueId = issueIdentifier;
            RespondentUserId = respondentUserId;

            BindModelToControlsInternal();

            CopyDocumentsCaseSelector.Filter.TopicUserIdentifier = topictUserId;
            CopyDocumentsCaseSelector.Filter.ExcludeIssueIdentifier = issueIdentifier;

            AddAttachmentButton.NavigateUrl = $"/ui/admin/assets/files/upload?case={issueIdentifier}";
            AddRequestButton.NavigateUrl = $"/ui/admin/workflow/attachments/request?case={issueIdentifier}";
        }

        private void BindModelToControlsInternal()
        {
            CaseDocumentList.BindIssueFiles(IssueId, RespondentUserId);

            CopyDocumentsButton.Visible = Organization.Toolkits.Issues.CaseDocumentCopy
                && CaseDocumentList.ItemsCount > 0;

            RequestsSection.Visible = CaseFileRequirementList.BindIssueRequests(IssueId);
        }
    }
}
