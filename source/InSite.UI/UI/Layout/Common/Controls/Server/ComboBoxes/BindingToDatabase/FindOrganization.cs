using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

namespace InSite.Common.Web.UI
{
    public class FindOrganization : BaseFindEntity<OrganizationFilter>
    {
        #region Properties

        public OrganizationFilter Filter => (OrganizationFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new OrganizationFilter()));

        #endregion

        protected override string GetEntityName() => "Organization";

        protected override string GetEditorUrl() => "/ui/admin/accounts/organizations/edit?organization={value}";

        protected override OrganizationFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.CompanyName = keyword;

            return filter;
        }

        protected override int Count(OrganizationFilter filter)
        {
            return OrganizationSearch.Count(filter);
        }

        protected override DataItem[] Select(OrganizationFilter filter)
        {
            filter.OrderBy = "CompanyName";

            return OrganizationSearch
                .Search(filter)
                .Select(x => new DataItem
                {
                    Value = x.OrganizationIdentifier,
                    Text = x.Name,
                })
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return Select(new OrganizationFilter { OrganizationIdentifiers = ids });
        }
    }
}