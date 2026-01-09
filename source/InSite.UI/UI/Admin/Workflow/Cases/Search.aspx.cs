using System;
using System.Collections.Generic;

using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Admin.Issues.Issues
{
    public partial class Search : SearchPage<QIssueFilter>
    {
        public class FilterIssueTypeEventArgs : EventArgs
        {
            public bool HasValue { get; }
            public string IssueType { get; set; }

            public FilterIssueTypeEventArgs(bool hasValue, string issueType)
            {
                HasValue = hasValue;
                IssueType = issueType;
            }
        }

        public override string EntityName => "Case";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new DownloadColumn[]
            {
                new DownloadColumn("IssueClosedby", "Case Closed by"),
                new DownloadColumn("IssueIdentifier", "Case Identifier"),
                new DownloadColumn("IssueOpenedby", "Case Opened by"),
                new DownloadColumn("TopicUserIdentifier"),
                new DownloadColumn("AdministratorUserName"),
                new DownloadColumn("IssueDescription", "Case Description"),
                new DownloadColumn("IssueEmployerGroupName", "Case Employer Group Name"),
                new DownloadColumn("IssueEmployerGroupParentGroupName", "Case Employer Group Parent Group Name"),
                new DownloadColumn("IssueSource", "Case Source"),
                new DownloadColumn("IssueStatusCategory", "Case Status Category"),
                new DownloadColumn("IssueStatusName", "Case Status Name"),
                new DownloadColumn("IssueTitle", "Case Title"),
                new DownloadColumn("IssueType", "Case Type"),
                new DownloadColumn("LawyerUserName"),
                new DownloadColumn("OwnerUserEmail"),
                new DownloadColumn("OwnerUserName"),
                new DownloadColumn("TopicAccountStatus"),
                new DownloadColumn("TopicEmployerGroupName"),
                new DownloadColumn("TopicUserName"),
                new DownloadColumn("TopicUserEmail"),
                new DownloadColumn("IssueNumber", "Case Number"),
                new DownloadColumn("IssueStatusSequence", "Case Status Sequence"),
                new DownloadColumn("IssueClosed", "Case Closed"),
                new DownloadColumn("IssueOpened", "Case Opened"),
                new DownloadColumn("IssueReported", "Case Reported"),
                new DownloadColumn("LastChangeTime"),
                new DownloadColumn("LastChangeType"),
                new DownloadColumn("LastChangeUser"),
                new DownloadColumn("LastChangeUserName"),
            };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.OwnerAssigned += SearchResults_OwnerAssigned;
            SearchCriteria.IssueTypeSet += SearchCriteria_IssueTypeSet;

            SearchResults.Searched += (source, args) =>
            {
                var filter = SearchCriteria.Filter;
                var recipientIds = ServiceLocator.IssueSearch.GetIssuesTopicUserIdentifiers(filter);

                BuildMessageTab.Visible = recipientIds.IsNotEmpty();
                SendEmail.BindModelToControls("Topic Member", recipientIds);
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("New Case", "/ui/admin/workflow/cases/open", null, null));
        }

        private void SearchResults_OwnerAssigned(object sender, EventArgs e)
        {
            ScreenStatus.AddMessage(AlertType.Success, "Owner was assigned to the selected cases");

            SearchResults.SearchWithCurrentPageIndex(SearchResults.Filter);
        }

        private void SearchCriteria_IssueTypeSet(object sender, FilterIssueTypeEventArgs e)
        {
            SearchResults.IssuTypeSet(e.HasValue, e.IssueType);
        }
    }
}