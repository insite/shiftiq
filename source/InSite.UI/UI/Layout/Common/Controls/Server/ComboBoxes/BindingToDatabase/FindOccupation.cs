using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

namespace InSite.Common.Web.UI
{
    public class FindOccupation : BaseFindEntity<StandardFilter>
    {
        public StandardFilter Filter => (StandardFilter)(ViewState[nameof(Filter)]
           ?? (ViewState[nameof(Filter)] = new StandardFilter()));

        public bool CurrentOrganizationOnly
        {
            get { return ViewState[nameof(CurrentOrganizationOnly)] as bool? ?? false; }
            set { ViewState[nameof(CurrentOrganizationOnly)] = value; }
        }

        protected override int Count(StandardFilter filter)
        {
            if (CurrentOrganizationOnly)
                filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;

            return StandardSearch.Count((StandardFilter)filter);
        }

        protected override string GetEntityName() => "Occupation";

        protected override StandardFilter GetFilter(string keyword)
        {
            StandardFilter filter = this.Filter.Clone();

            return filter;
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return StandardSearch.Select(new StandardFilter { StandardIdentifiers = ids }).ToArray().Select(x => new DataItem
            {
                Value = x.StandardIdentifier,
                Text = x.ContentTitle
            }).ToArray();
        }

        protected override DataItem[] Select(StandardFilter filter)
        {
            if (CurrentOrganizationOnly)
                filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;

            var data = StandardSearch
                .Select(Filter)
                .Select(x => new DataItem
                {
                    Value = x.StandardIdentifier,
                    Text = x.ContentTitle
                })
                .ToArray();

            return data;
        }

    }
}