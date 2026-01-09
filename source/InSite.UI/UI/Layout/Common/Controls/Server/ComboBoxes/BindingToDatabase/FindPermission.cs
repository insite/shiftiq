using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

namespace InSite.Common.Web.UI
{
    public class FindPermission : BaseFindEntity<TActionFilter>
    {
        #region Properties

        public TActionFilter Filter => (TActionFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new TActionFilter()));

        #endregion

        protected override string GetEntityName() => "Permission";

        protected override TActionFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.ActionName = keyword;
            filter.ActionType = "Permission";

            return filter;
        }

        protected override int Count(TActionFilter filter)
        {
            return TActionSearch.Count(filter);
        }

        protected override DataItem[] Select(TActionFilter filter)
        {
            filter.OrderBy = nameof(TAction.ActionName);

            return TActionSearch
                .Search(filter)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return TActionSearch.Search(x => ids.Contains(x.ActionIdentifier)).Select(GetDataItem);
        }

        private static DataItem GetDataItem(TAction x) => new DataItem
        {
            Value = x.ActionIdentifier,
            Text = x.ActionName,
        };
    }
}
