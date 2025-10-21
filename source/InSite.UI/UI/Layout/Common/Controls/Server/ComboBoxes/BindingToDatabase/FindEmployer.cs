using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contacts.Read;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class FindEmployer : BaseFindEntity<QGroupSelectorFilter>
    {
        #region Properties

        public QGroupSelectorFilter Filter => (QGroupSelectorFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QGroupSelectorFilter()));

        #endregion

        protected override string GetEntityName() => "Employer";

        protected override QGroupSelectorFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            if (MaxSelectionCount == 1)
                filter.AlwaysIncludeGroupIdentifiers = Value.HasValue ? new[] { Value.Value } : null;
            else if (SelectedCount > 0)
                filter.AlwaysIncludeGroupIdentifiers = Values;
            else
                filter.AlwaysIncludeGroupIdentifiers = null;

            filter.GroupType = GroupTypes.Employer;
            filter.Keyword = keyword;

            return filter;
        }

        protected override int Count(QGroupSelectorFilter filter)
        {
            return ServiceLocator.GroupSearch.CountSelectorGroups(filter);
        }

        protected override DataItem[] Select(QGroupSelectorFilter filter)
        {
            var groups = ServiceLocator.GroupSearch.GetSelectorGroups(filter, false);

            return groups.Select(GetDataItem).ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            var filter = new QGroupSelectorFilter { IncludeGroupIdentifiers = ids };
            var groups = ServiceLocator.GroupSearch.GetSelectorGroups(filter, false);

            return groups.Select(GetDataItem).ToArray();
        }

        private static DataItem GetDataItem(GroupSelectorItem x) => new DataItem
        {
            Value = x.GroupIdentifier,
            Text = x.GroupName + (!string.IsNullOrEmpty(x.GroupCode) ? " " + x.GroupCode : string.Empty)
        };
    }
}