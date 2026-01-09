using System;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Sdk.UI;

namespace InSite.UI.Portal.Issues
{
    public partial class Outline : PortalBasePage, IHasTitle
    {
        private const string SearchUrl = "/ui/portal/workflow/cases/search";

        private Guid IssueIdentifier => Guid.TryParse(Request.QueryString["id"], out var id) ? id : Guid.Empty;

        private int IssueNumber
        {
            get => (int)ViewState[nameof(IssueNumber)];
            set => ViewState[nameof(IssueNumber)] = value;
        }

        private Guid? TopicUserIdentifier
        {
            get => (Guid?)ViewState[nameof(TopicUserIdentifier)];
            set => ViewState[nameof(TopicUserIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CaseDocumentList.FileUploaded += DocumentList_FileUploaded;
            CaseFileRequirementList.FileUploaded += FileRequirementList_FileUploaded;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void DocumentList_FileUploaded(object sender, EventArgs e)
        {
            BindAttachments();
        }

        private void FileRequirementList_FileUploaded(object sender, EventArgs e)
        {
            BindAttachments();
            BindFileRequirements();
        }

        private void LoadData()
        {
            var model = ServiceLocator.IssueSearch.GetIssue(IssueIdentifier);
            if (model == null
                || model.OrganizationIdentifier != Organization.Identifier
                || model.TopicUserIdentifier != User.Identifier
                )
            {
                HttpResponseHelper.Redirect(SearchUrl);
            }

            IssueNumber = model.IssueNumber;
            TopicUserIdentifier = model.TopicUserIdentifier;

            PageHelper.AutoBindHeader(this);

            IssueType.Text = model.IssueType;
            IssueStatusName.Text = model.IssueStatusName;
            IssueTitle.Text = model.IssueTitle;
            IssueDescriptionHtml.Text = model.IssueDescriptionHtml;
            NewAttachmentPanel.Visible = model.IssueStatusCategory != "Closed";

            BindAttachments();
            BindFileRequirements();
        }

        private void BindAttachments()
        {
            CaseDocumentList.BindFiles(IssueIdentifier, TopicUserIdentifier);
        }

        private void BindFileRequirements()
        {
            RequestPanel.Visible = CaseFileRequirementList.BindIssueRequests(IssueIdentifier);
        }

        public string GetTitle()
            => $"Case #{IssueNumber}";
    }
}