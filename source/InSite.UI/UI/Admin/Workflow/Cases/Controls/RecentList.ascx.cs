using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Issues.Reports.Controls
{
    public partial class RecentList : BaseUserControl
    {
        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        public void LoadData(QIssueFilter filter, int count)
        {
            filter.Paging = Paging.SetSkipTake(0, count);

            var issues = ServiceLocator.IssueSearch.GetIssues(filter);
            ItemCount = issues.Count;

            IssueRepeater.DataSource = issues;
            IssueRepeater.DataBind();

            NoChanges.Visible = issues.Count == 0;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IssueRepeater.ItemDataBound += Repeater_ItemDataBound;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
            {
                return;
            }

            var lastLiteral = (ITextControl)e.Item.FindControl("LastChangeTimestamp");
            var item = (VIssue)e.Item.DataItem;

            var who = UserSearch.GetFullName(item.LastChangeUser);
            var timestamp = UserSearch.GetTimestampHtml(item.LastChangeType, item.LastChangeTime, who);

            lastLiteral.Text = timestamp;
        }
    }
}