using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class AchievementComboBox : ComboBox
    {
        public QAchievementFilter ListFilter => (QAchievementFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new QAchievementFilter()));

        protected override ListItemArray CreateDataSource()
        {
            ListFilter.OrganizationIdentifiers.Add(CurrentSessionState.Identity.Organization.Identifier);

            var list = new ListItemArray();

            foreach (var achievement in ServiceLocator.AchievementSearch.GetAchievements(ListFilter))
                list.Add(new ListItem
                {
                    Value = achievement.AchievementIdentifier.ToString(),
                    Text = achievement.AchievementTitle
                });

            return list;
        }
    }
}