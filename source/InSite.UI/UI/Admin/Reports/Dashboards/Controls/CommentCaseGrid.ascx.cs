using System;
using System.ComponentModel;

using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Reports.Dashboards.Controls
{
    public partial class CommentCaseGrid : SearchResultsGridViewController<QIssueCommentFilter>
    {
        #region Properties
        protected override int DefaultPageSize => 10;

        #endregion

        #region Methods (loading)

        public void LoadData(QIssueCommentFilter filter)
        {
            Search(filter);
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(QIssueCommentFilter filter)
        {
            var count = ServiceLocator.IssueSearch.CountComments(filter);

            EmptyGrid.Visible = count == 0;

            return count;
        }

        protected override IListSource SelectData(QIssueCommentFilter filter)
        {
            return ServiceLocator.IssueSearch.GetComments(filter).ToSearchResult();
        }

        protected string GetCommentHtml(object item)
        {
            return Markdown.ToHtml(item.ToString());
        }

        protected string GetIssueNumber(Guid IssueId)
        {
            return ServiceLocator.IssueSearch.GetIssue(IssueId).IssueNumber.ToString();
        }

        #endregion
    }
}