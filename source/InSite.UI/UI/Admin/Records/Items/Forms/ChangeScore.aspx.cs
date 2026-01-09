using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Gradebooks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Items.Forms
{
    public partial class ChangeScore : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        private Guid ItemKey => Guid.TryParse(Request["item"], out var value) ? value : Guid.Empty;

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

            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Achievement);
            if (gradebook == null)
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");

            PageHelper.AutoBindHeader(this);

            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

            Detail.InitGradebook(gradebook, data, null);

            if (!Detail.SetInputValue(GradebookIdentifier, data, ItemKey))
                HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=config");

            CancelButton.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=config";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            if (data.IsLocked)
            {
                Status.AddMessage(AlertType.Error, "The gradebook is locked and therefore cannot be modified");
                return;
            }

            var score = Detail.GetInputValues();
            var item = data.FindItem(ItemKey);

            var commands = new List<ICommand>();

            commands.Add(new ChangeGradeItem(
                GradebookIdentifier,
                ItemKey,
                score.Code,
                score.Name,
                score.ShortName,
                score.IncludeToReport,
                score.Format,
                GradeItemType.Score,
                GradeItemWeighting.None,
                score.ParentItem
            ));

            if (!string.Equals(score.Reference, item.Reference))
                commands.Add(new ReferenceGradeItem(GradebookIdentifier, ItemKey, score.Reference));

            if (!string.Equals(score.Hook, item.Hook))
                commands.Add(new ChangeGradeItemHook(GradebookIdentifier, ItemKey, score.Hook));

            if (data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards)
                commands.Add(new ChangeGradeItemCompetencies(GradebookIdentifier, ItemKey, score.Standards));

            if (score.MaxPoint != item.MaxPoints)
                commands.Add(new ChangeGradeItemMaxPoints(GradebookIdentifier, ItemKey, score.MaxPoint));

            commands.Add(new ChangeGradeItemPassPercent(GradebookIdentifier, ItemKey, score.PassPercent));
            commands.Add(new ChangeGradeItemAchievement(GradebookIdentifier, ItemKey, score.Achievement.Achievement));
            commands.Add(new ChangeGradeItemNotifications(GradebookIdentifier, ItemKey, score.Notifications));

            ServiceLocator.SendCommands(commands);

            Course2Store.ClearCache(Organization.Identifier);

            HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=config");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={GradebookIdentifier}&panel=config"
                : null;
        }
    }
}
