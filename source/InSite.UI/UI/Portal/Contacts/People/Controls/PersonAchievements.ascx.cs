using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class PersonAchievements : BaseUserControl
    {
        public int LoadData(Guid organizationId, Guid userId)
        {
            var filter = new VCredentialFilter
            {
                OrganizationIdentifier = organizationId,
                UserIdentifier = userId
            };

            var credentials = ServiceLocator.AchievementSearch.GetCredentials(filter).Select(x => new
            {
                AchievementTitle = x.AchievementTitle,
                CredentialStatus = x.CredentialStatus,
                CredentialExpired = x.CredentialExpired,
                CredentialGranted = x.CredentialGranted,
                CredentialRevoked = x.CredentialRevoked
            }).ToList();

            AchievementGrid.DataSource = credentials;
            AchievementGrid.DataBind();
            AchievementGrid.Visible = credentials.Count > 0;

            NoAchievements.Visible = credentials.Count == 0;

            return credentials.Count;
        }

        protected string LocalizeDate(object date)
        {
            return date != null
                ? TimeZones.FormatDateOnly((DateTimeOffset)date, User.TimeZone)
                : string.Empty;
        }
    }
}