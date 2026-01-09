using System;

using InSite.Application.Achievements.Write;
using InSite.Cmds.Admin.Achievements.Models;
using InSite.Common.Web;
using InSite.Domain.Records;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Admin.Achievements.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/cmds/admin/achievements/edit";
        private const string SearchUrl = "/ui/cmds/admin/achievements/search";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                AchievementDetails.SetDefaultValues();

                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void Save()
        {
            var info = AchievementDetails.GetInputValues();

            var expiration = info.LifetimeMonths.HasValue
                ? new Expiration { Type = ExpirationType.Relative, Lifetime = new Lifetime { Quantity = info.LifetimeMonths.Value, Unit = "Month" } }
                : null;

            var command = new CreateAchievement(UniqueIdentifier.Create(), Organization.Identifier, info.AchievementLabel, info.AchievementTitle, info.AchievementDescription, expiration);
            command.OriginOrganization = info.OrganizationIdentifier;

            ServiceLocator.SendCommand(command);

            AchievementDetails.SaveAchievementCategoriesAndAccessControl(command.AggregateIdentifier);

            var after = new CmdsAchievementChanged
            {
                OrganizationIdentifier = info.OrganizationIdentifier,
                Type = info.AchievementLabel,
                Title = info.AchievementTitle,
                Lifetime = info.LifetimeMonths ?? 0,
                Description = info.AchievementDescription,
                Hours = 0m
            };

            var author = new EmailAddress(User.Email, User.FullName);

            var observer = new AchievementObserver(ServiceLocator.ChangeQueue, author, null);

            observer.Created(after);

            var editUrl = string.Format("{0}?id={1}&status=saved", EditUrl, command.AggregateIdentifier);
            HttpResponseHelper.Redirect(editUrl);
        }

        public override void ApplyAccessControlForCmds() { }
    }
}
