using System;
using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Identities.Departments.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<DepartmentFilter>
    {
        protected override int SelectCount(DepartmentFilter filter)
        {
            return DepartmentSearch.Count(filter);
        }

        protected override IListSource SelectData(DepartmentFilter filter)
        {
            filter.OrderBy = "DepartmentName";

            return DepartmentSearch
                .Bind(
                    x => new
                    {
                        x.DepartmentIdentifier,
                        x.DepartmentName,
                        x.DepartmentCode,
                        x.DivisionIdentifier,
                        DivisionName = x.Division != null ? x.Division.DivisionName : null,
                        Created = x.GroupCreated,
                        x.OrganizationIdentifier,
                        OrganizationName = x.Organization.CompanyName
                    }, filter)
                .ToSearchResult();
        }

        protected static string DateToHtml(object date)
            => (date == null) ? string.Empty : TimeZones.Format((DateTimeOffset)date, User.TimeZone, true);
    }
}