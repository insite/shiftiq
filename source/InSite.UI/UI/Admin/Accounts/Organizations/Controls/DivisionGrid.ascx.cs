using System;
using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Accounts.Organizations.Controls
{
    public partial class DivisionGrid : SearchResultsGridViewController<DivisionFilter>
    {
        protected override bool IsFinder => false;

        public void LoadData(Guid organizationId)
        {
            Search(new DivisionFilter { OrganizationIdentifier = organizationId });
        }

        protected override int SelectCount(DivisionFilter filter)
        {
            var count = DivisionSearch.Count(filter);

            EmptyGrid.Visible = count == 0;

            return count;
        }

        protected override IListSource SelectData(DivisionFilter filter)
        {
            filter.OrderBy = "DivisionName";

            return DivisionSearch.BindByFilter(x => new
                    {
                        x.DivisionIdentifier,
                        x.DivisionName,
                        x.DivisionCode,
                        Created = x.GroupCreated
                    },
                    filter
                )
                .ToSearchResult();
        }

        protected static string LocalDate(object date)
            => (date == null) ? string.Empty : TimeZones.Format((DateTimeOffset)date, User.TimeZone, true);
    }
}