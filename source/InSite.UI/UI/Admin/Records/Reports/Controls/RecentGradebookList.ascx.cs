using System;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Records.Reports.Controls
{
    public partial class RecentGradebookList : BaseUserControl
    {
        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        public void LoadData(int count)
        {
            var filter = new QGradebookFilter { OrganizationIdentifier = Organization.OrganizationIdentifier };
            var gradebooks = ServiceLocator.RecordSearch.GetRecentGradebooks(filter, count);
            ItemCount = gradebooks.Count;

            GradebookRepeater.DataSource = gradebooks.Select(x =>
            {
                return new
                {
                    x.AchievementIdentifier,
                    x.GradebookIdentifier,
                    x.GradebookTitle,
                    LastChangeTimestamp = $"{Shift.Common.Humanizer.SentenceCase(x.LastChangeType)} by {x.LastChangeUser} {Shift.Common.Humanizer.Humanize(x.LastChangeTime)}"
                };
            });
            GradebookRepeater.DataBind();
        }

        protected string GetAchievementHtml(Guid? achievementIdentifier)
        {
            if (achievementIdentifier.HasValue)
            {
                var achievement = ServiceLocator.AchievementSearch.GetAchievement(achievementIdentifier.Value);
                if (achievement != null)
                    return $"<a href='/ui/admin/records/achievements/outline?id={achievementIdentifier}'><i class='far fa-trophy me-2'></i>{achievement.AchievementTitle}</a>";
            }

            return string.Empty;
        }
    }
}