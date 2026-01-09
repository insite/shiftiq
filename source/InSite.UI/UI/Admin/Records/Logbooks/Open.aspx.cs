using System;
using System.Collections.Generic;
using System.Web.UI;

using Shift.Common.Timeline.Commands;

using InSite.Application.JournalSetups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Logbooks
{
    public partial class Open : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventIdentifier.AutoPostBack = true;
            EventIdentifier.ValueChanged += EventIdentifier_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            EventIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            EventIdentifier.Filter.EventType = "Class";

            StandardIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            StandardIdentifier.Filter.StandardTypes = new[] { Shift.Constant.StandardType.Framework };

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One);

            CancelButton.NavigateUrl = "/ui/admin/records/logbooks/search";
        }

        private void EventIdentifier_ValueChanged(object sender, EventArgs e)
        {
            if (!EventIdentifier.HasValue)
                return;

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value.Value);
            var achievement = @event.AchievementIdentifier.HasValue ? ServiceLocator.AchievementSearch.GetAchievement(@event.AchievementIdentifier.Value) : null;

            if (achievement != null)
                AchievementIdentifier.Value = achievement.AchievementIdentifier;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var identifier = UniqueIdentifier.Create();
            var @class = EventIdentifier.Value;
            var achievement = AchievementIdentifier.Value;
            var framework = StandardIdentifier.Value;

            var content = new ContentContainer();
            content.Title.Text.Default = TitleInput.Text;

            var commands = new List<Command>();
            commands.Add(new CreateJournalSetup(identifier, Organization.OrganizationIdentifier, JournalSetupName.Text));
            commands.Add(new ChangeJournalSetupContent(identifier, content));

            if (@class.HasValue)
                commands.Add(new ChangeJournalSetupEvent(identifier, @class));

            if (achievement.HasValue)
                commands.Add(new ChangeJournalSetupAchievement(identifier, achievement));

            if (framework.HasValue)
                commands.Add(new ChangeJournalSetupFramework(identifier, framework));

            ServiceLocator.SendCommands(commands);

            var url = $"/ui/admin/records/logbooks/outline?journalsetup={identifier}";

            HttpResponseHelper.Redirect(url);
        }
    }
}
