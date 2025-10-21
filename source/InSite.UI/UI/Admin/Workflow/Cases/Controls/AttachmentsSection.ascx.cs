using System;

using InSite.Common.Web.UI;

namespace InSite.Admin.Issues.Outlines.Controls
{
    public partial class AttachmentsSection : BaseUserControl
    {
        private Guid IssueIdentifier
        {
            get => (Guid)ViewState[nameof(IssueIdentifier)];
            set => ViewState[nameof(IssueIdentifier)] = value;
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
        }

        private void IssueFileRequirementList_Changed(object sender, EventArgs e)
        {
            BindModelToControls(IssueIdentifier, RespondentUserId);
        }

        public void BindModelToControls(Guid issueIdentifier, Guid? respondentUserId)
        {
            IssueIdentifier = issueIdentifier;
            RespondentUserId = respondentUserId;

            CaseDocumentList.BindIssueFiles(issueIdentifier, respondentUserId);

            RequestsSection.Visible = CaseFileRequirementList.BindIssueRequests(issueIdentifier);

            AddAttachmentButton.NavigateUrl = $"/ui/admin/assets/files/upload?case={issueIdentifier}";
            AddRequestButton.NavigateUrl = $"/ui/admin/workflow/attachments/request?case={issueIdentifier}";
        }
    }
}