using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

namespace InSite.Common.Web.UI
{
    public class FindPeriod : BaseFindEntity<QPeriodFilter>
    {
        #region Properties

        public QPeriodFilter Filter => (QPeriodFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QPeriodFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        #endregion

        protected override string GetEntityName() => "Period";

        protected override QPeriodFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.PeriodName = keyword;

            return filter;
        }

        protected override int Count(QPeriodFilter filter)
        {
            return ServiceLocator.PeriodSearch.CountPeriods(filter);
        }

        protected override DataItem[] Select(QPeriodFilter filter)
        {
            filter.OrderBy = nameof(QPeriod.PeriodName) + " desc";

            return ServiceLocator.PeriodSearch
                .GetPeriods(filter)
                .Select(x => new DataItem
                {
                    Value = x.PeriodIdentifier,
                    Text = x.PeriodName,
                })
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            var filter = new QPeriodFilter { Identifiers = new HashSet<Guid>(ids) };
            return Select(filter);
        }
    }
}