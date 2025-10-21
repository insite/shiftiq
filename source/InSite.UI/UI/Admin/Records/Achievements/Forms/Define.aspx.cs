using System;
using System.Web.UI;

using InSite.Application.Achievements.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Achievements.Achievements.Forms
{
    public partial class Define : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One);

            CancelButton.NavigateUrl = "/ui/admin/records/achievements/search";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var achievementID = UniqueIdentifier.Create();
            var expiration = ExpirationField.GetExpiration();
            var command = new CreateAchievement(achievementID, Organization.Identifier, AchievementLabel.Text, AchievementTitle.Text, AchievementDescription.Text, expiration);

            ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect($"/ui/admin/records/achievements/outline?id={achievementID}");
        }
    }
}
