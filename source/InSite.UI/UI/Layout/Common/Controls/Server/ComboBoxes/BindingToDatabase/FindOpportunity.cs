using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

namespace InSite.Common.Web.UI
{
    public class FindOpportunity : BaseFindEntity<TOpportunityFilter>
    {
        protected override string GetEntityName() => "Job";

        protected override TOpportunityFilter GetFilter(string keyword)
        {
            return new TOpportunityFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                JobTitle = keyword
            };
        }

        protected override int Count(TOpportunityFilter filter)
        {
            return TOpportunitySearch.Count(filter);
        }

        protected override DataItem[] Select(TOpportunityFilter filter)
        {
            filter.OrderBy = nameof(TOpportunity.JobTitle) + "," + nameof(TOpportunity.EmployerGroupName);

            return TOpportunitySearch.SelectByJobFilter(filter)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return TOpportunitySearch.Select(ids)
                .Select(GetDataItem)
                .ToArray();
        }

        private static DataItem GetDataItem(TOpportunity x) => new DataItem
        {
            Value = x.OpportunityIdentifier,
            Text = x.JobTitle + " - " + x.EmployerGroupName
        };
    }
}