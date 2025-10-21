using System;
using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class UserAuthenticationGrid : SearchResultsGridViewController<TUserSessionFilter>
    {
        protected override bool IsFinder => false;

        public void LoadData(Guid user, Guid organization)
        {
            var filter = new TUserSessionFilter
            {
                UserIdentifier = user,
                OrganizationIdentifier = organization
            };

            Search(filter);
        }

        protected override int SelectCount(TUserSessionFilter filter)
        {
            return TUserSessionSearch.Count(filter);
        }

        protected override IListSource SelectData(TUserSessionFilter filter)
        {
            filter.OrderBy = "SessionStarted DESC";

            return TUserSessionSearch.Bind(x => x, filter).ToSearchResult();
        }

        protected string GetOrganizationCode(Guid organizationIdentifier)
        {
            var organization = OrganizationSearch.Select(organizationIdentifier);

            if (organization != null)
            {
                return organization.Code;
            }

            return null;
        }

        protected string GetDateString(DateTimeOffset date)
        {
            return TimeZones.Format(date, Identity.User.TimeZone, true, false);
        }
    }
}