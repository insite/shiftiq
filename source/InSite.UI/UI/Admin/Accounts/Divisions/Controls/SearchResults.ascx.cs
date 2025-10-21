using System;
using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Accounts.Divisions.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<DivisionFilter>
    {
        protected override int SelectCount(DivisionFilter filter)
        {
            return DivisionSearch.Count(filter);
        }

        protected override IListSource SelectData(DivisionFilter filter)
        {
            filter.OrderBy = "DivisionName";

            return DivisionSearch.BindByFilter(x => new
                    {
                        x.DivisionIdentifier,
                        x.DivisionName,
                        x.DivisionCode,
                        Created = x.GroupCreated,
                        x.OrganizationIdentifier,
                        OrganizationName = x.Organization.CompanyName
                    },
                    filter
                )
                .ToSearchResult();
        }

        protected static string DateToHtml(object date)
            => (date == null) ? string.Empty : TimeZones.Format((DateTimeOffset)date, User.TimeZone, true);
    }
}