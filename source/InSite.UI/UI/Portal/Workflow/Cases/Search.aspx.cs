using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Application.Issues.Read;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Issues
{
    public partial class Search : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IssueRepeater.ItemDataBound += IssueRepeater_ItemDataBound;
        }

        private void IssueRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            BindModelToControls((VIssue)e.Item.DataItem, e.Item);
        }

        private void BindModelToControls(VIssue issue, RepeaterItem item)
        {
            var viewIssue = (HyperLink)item.FindControl("ViewIssueLink");
            viewIssue.NavigateUrl = $"/ui/portal/workflow/cases/outline?id={issue.IssueIdentifier}";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            var issues = GetIssues();

            NoIssuesAlert.Visible = issues.Count == 0;
            IssueRepeater.Visible = !NoIssuesAlert.Visible;
            IssueRepeater.DataSource = issues;
            IssueRepeater.DataBind();
        }

        private List<VIssue> GetIssues()
        {
            if((Organization.Toolkits?.Issues?.EnableWorkflowManagement ?? false))
                return ServiceLocator.IssueSearch.GetIssuesWithCommentMentions(Identity.Organization.Identifier, User.Identifier);
            else
            {
                var filter = new QIssueFilter
                {
                    OrganizationIdentifier = Identity.Organization.Identifier,
                    TopicUserIdentifier = User.Identifier
                };
                return ServiceLocator.IssueSearch.GetIssues(filter);
            }
        }
    }
}