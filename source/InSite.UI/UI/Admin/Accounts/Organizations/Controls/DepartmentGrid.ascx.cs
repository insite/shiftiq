using System;
using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Accounts.Organizations.Controls
{
    public partial class DepartmentGrid : SearchResultsGridViewController<DepartmentFilter>
    {
        protected override bool IsFinder => false;

        public void LoadData(Guid organizationId)
        {
            Search(new DepartmentFilter { OrganizationIdentifier = organizationId });
        }

        protected override int SelectCount(DepartmentFilter filter)
        {
            var count = DepartmentSearch.Count(filter);

            EmptyGrid.Visible = count == 0;

            return count;
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
                        Created = x.GroupCreated
                    }, 
                    filter)
                .ToSearchResult();
        }

        protected static string LocalDate(object date)
            => (date == null) ? string.Empty : TimeZones.Format((DateTimeOffset)date, User.TimeZone, true);
    }
}