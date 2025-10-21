using System;
using System.Web.UI;

using InSite.Application.Records.Read;

namespace InSite.UI.Admin.Records.Achievements.Controls
{
    public partial class AchievementDetailsV2 : UserControl
    {
        public void BindAchievement(QAchievement achievement, TimeZoneInfo tz, bool showExpiration = true)
        {
            AchievementLink.HRef = $"/ui/admin/records/achievements/outline?id={achievement.AchievementIdentifier}";
            Title.Text = achievement.AchievementTitle;
            Label.Text = achievement.AchievementLabel;
            Expiration.Text = GetExpiryText(achievement, tz);
            Status.Text = !achievement.AchievementIsEnabled ? "<span class='badge bg-danger'><i class='far fa-lock'></i> Locked</span>" :
                "<span class='badge bg-success'><i class='far fa-lock-open'></i> Unlocked</span>";

            ExpirationField.Visible = showExpiration;
        }

        private string GetExpiryText(QAchievement i, TimeZoneInfo tz)
        {
            var achievementExpiration = new Domain.Records.Expiration(
                i.ExpirationType, i.ExpirationFixedDate,
                i.ExpirationLifetimeQuantity, i.ExpirationLifetimeUnit);

            return achievementExpiration.ToString(tz);
        }
    }
}