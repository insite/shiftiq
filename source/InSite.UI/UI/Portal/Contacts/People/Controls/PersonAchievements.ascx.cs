using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class PersonAchievements : SearchResultsGridViewController<VCredentialFilter>
    {
        protected override bool IsFinder => false;

        public void LoadData(Guid organizationId, Guid userId)
        {
            Search(new VCredentialFilter
            {
                OrganizationIdentifier = organizationId,
                UserIdentifier = userId
            });
        }

        protected override IListSource SelectData(VCredentialFilter filter)
        {
            filter.OrderBy = nameof(VCredential.AchievementLabel) + "," + nameof(VCredential.AchievementTitle);

            var list = ServiceLocator.AchievementSearch.GetCredentials(filter)
                .Select(x => new
                {
                    AchievementTitle = x.AchievementTitle,
                    AchievementLabel = x.AchievementLabel,
                    CredentialStatus = x.CredentialStatus,
                    CredentialExpired = x.CredentialExpired,
                    CredentialGranted = x.CredentialGranted,
                    CredentialRevoked = x.CredentialRevoked
                })
                .ToList();

            NoAchievements.Visible = list.Count == 0;

            return list.ToSearchResult();
        }

        protected override int SelectCount(VCredentialFilter filter)
        {
            return ServiceLocator.AchievementSearch.CountCredentials(filter);
        }

        protected string LocalizeDate(object date)
        {
            return date != null
                ? TimeZones.FormatDateOnly((DateTimeOffset)date, User.TimeZone)
                : string.Empty;
        }
    }
}
