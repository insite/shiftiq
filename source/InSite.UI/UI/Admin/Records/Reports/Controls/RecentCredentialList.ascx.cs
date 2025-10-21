using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Records.Reports.Controls
{
    public partial class RecentCredentialList : BaseUserControl
    {
        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        public void LoadData(int count)
        {
            var filter = new VCredentialFilter { OrganizationIdentifier = Organization.OrganizationIdentifier };
            var credentials = ServiceLocator.AchievementSearch.GetRecentCredentials(filter, count);
            ItemCount = credentials.Count;

            CredentialRepeater.DataSource = credentials.Select(x =>
            {
                return new
                {
                    x.CredentialIdentifier,
                    x.AchievementTitle,
                    x.AchievementLabel,
                    LastChangeTimestamp = $"Granted to {x.UserFullName} {Shift.Common.Humanizer.Humanize(x.CredentialGranted)}"
                };
            });
            CredentialRepeater.DataBind();
        }
    }
}