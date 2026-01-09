using System;
using System.Linq;

using InSite.Application.Achievements.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Achievements.Achievements.Forms
{
    public partial class DeletePrerequisite : AdminBasePage, IHasParentLinkParameters
    {
        private Guid AchievementIdentifier => Guid.TryParse(Request["achievement"], out var value) ? value : Guid.Empty;
        private Guid PrerequisiteIdentifier => Guid.TryParse(Request["prerequisite"], out var value) ? value : Guid.Empty;

        public string GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"id={AchievementIdentifier}" : null;

        public string OutlineUrl
            => $"/ui/admin/records/achievements/outline?id={AchievementIdentifier}";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier, x => x.Prerequisites);
            if (achievement == null || achievement.OrganizationIdentifier != Organization.Identifier
                || !achievement.Prerequisites.Any(x => x.PrerequisiteIdentifier == PrerequisiteIdentifier)
                )
            {
                HttpResponseHelper.Redirect($"/ui/admin/records/achievements/search");
            }

            PageHelper.AutoBindHeader(this, null, achievement.AchievementTitle);

            CancelButton.NavigateUrl = OutlineUrl;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new DeleteAchievementPrerequisite(AchievementIdentifier, PrerequisiteIdentifier));
            HttpResponseHelper.Redirect(OutlineUrl);
        }
    }
}
