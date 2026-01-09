using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Issues.Read;

namespace InSite.Common.Web.UI
{
    public class FindCase : BaseFindEntity<QIssueFilter>
    {
        #region Properties

        public QIssueFilter Filter => (QIssueFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QIssueFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        public bool OrderByOpened
        {
            get => ViewState[nameof(OrderByOpened)] as bool? ?? false;
            set => ViewState[nameof(OrderByOpened)] = value;
        }

        #endregion

        protected override string GetEntityName() => "Case";

        protected override QIssueFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.IssueTitle = keyword;

            return filter;
        }

        protected override int Count(QIssueFilter filter)
        {
            return ServiceLocator.IssueSearch.CountIssues(filter);
        }

        protected override DataItem[] Select(QIssueFilter filter)
        {
            filter.OrderBy = OrderByOpened
                ? nameof(QIssue.IssueOpened) + " desc"
                : nameof(QIssue.IssueTitle);

            return ServiceLocator.IssueSearch
                .GetIssues(filter)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.IssueSearch
                .GetIssues(ids)
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(VIssue x) => new DataItem
        {
            Value = x.IssueIdentifier,
            Text = x.IssueTitle,
        };
    }
}