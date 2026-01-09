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
    public partial class ChangeCalculation : AdminBasePage, IHasParentLinkParameters
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

            if (!IsPostBack)
            {
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

            var calculation = Detail.GetInputValues();
            var item = data.FindItem(ItemKey);

            var commands = new List<ICommand>();

            commands.Add(new ChangeGradeItem(
                GradebookIdentifier,
                ItemKey,
                calculation.Code,
                calculation.Name,
                calculation.ShortName,
                calculation.IncludeToReport,
                GradeItemFormat.None,
                GradeItemType.Calculation,
                calculation.Weighting,
                calculation.ParentItem
            ));

            commands.Add(new ChangeGradeItemCalculation(GradebookIdentifier, ItemKey, calculation.Parts));

            if (!string.Equals(calculation.Reference, item.Reference))
                commands.Add(new ReferenceGradeItem(GradebookIdentifier, ItemKey, calculation.Reference));

            if (!string.IsNullOrEmpty(calculation.Hook))
                commands.Add(new ChangeGradeItemHook(GradebookIdentifier, ItemKey, calculation.Hook));

            if (data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards)
                commands.Add(new ChangeGradeItemCompetencies(GradebookIdentifier, ItemKey, calculation.Standards));

            commands.Add(new ChangeGradeItemPassPercent(GradebookIdentifier, ItemKey, calculation.PassPercent));
            commands.Add(new ChangeGradeItemAchievement(GradebookIdentifier, ItemKey, calculation.Achievement.Achievement));

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
