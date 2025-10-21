using System;
using System.ComponentModel;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Accounts.Developers.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QPersonSecretFilter>
    {
        protected override int SelectCount(QPersonSecretFilter filter)
        {
            return ServiceLocator.PersonSecretSearch.Count(filter);
        }

        protected override IListSource SelectData(QPersonSecretFilter filter)
        {
            filter.OrderBy = "Person.User.FullName";

            var results = ServiceLocator.PersonSecretSearch.Bind(x => new
            {
                TokenIdentifier = x.SecretIdentifier,
                TokenExpiry = x.SecretExpiry,
                DeveloperIdentifier = x.Person.UserIdentifier,
                DeveloperName = x.Person.User.FullName,
                OrganizationCode = x.Person.Organization.OrganizationCode
            }, filter,
                x => x.Person,
                x => x.Person.User,
                x => x.Person.Organization
            );

            return results.ToSearchResult();
        }

        protected static string DateToHtml(object date)
            => (date == null) ? string.Empty : TimeZones.Format((DateTimeOffset)date, User.TimeZone, true);

        protected static string TimeDifferenceToHtml(object start, object end)
        {
            if (start == null || end == null)
                return string.Empty;

            var startDate = (DateTimeOffset)start;
            var endDate = (DateTimeOffset)end;

            if (startDate >= endDate)
                return string.Empty;

            var diff = endDate - startDate;

            return "Issued for " + diff.Humanize(maxUnit: TimeUnit.Year);
        }
    }
}