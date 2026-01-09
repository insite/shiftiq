using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Gradebooks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Items.Forms
{
    public partial class AddCalculation : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

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

                Detail.InitGradebook(gradebook, data, data.GetNextCode());

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
            var item = UniqueIdentifier.Create();

            var commands = new List<ICommand>();

            commands.Add(new AddGradeItem(
                GradebookIdentifier,
                item,
                calculation.Code,
                calculation.Name,
                calculation.ShortName,
                calculation.IncludeToReport,
                GradeItemFormat.None,
                GradeItemType.Calculation,
                calculation.Weighting,
                null,
                calculation.ParentItem
            ));

            if (calculation.Parts.IsNotEmpty())
                commands.Add(new ChangeGradeItemCalculation(GradebookIdentifier, item, calculation.Parts));

            if (calculation.Reference.IsNotEmpty())
                commands.Add(new ReferenceGradeItem(GradebookIdentifier, item, calculation.Reference));

            if (calculation.Hook.IsNotEmpty())
                commands.Add(new ChangeGradeItemHook(GradebookIdentifier, item, calculation.Hook));

            if ((data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards) && calculation.Standards != null)
                commands.Add(new ChangeGradeItemCompetencies(GradebookIdentifier, item, calculation.Standards));

            if (calculation.PassPercent.HasValue)
                commands.Add(new ChangeGradeItemPassPercent(GradebookIdentifier, item, calculation.PassPercent));

            if (calculation.Achievement != null)
                commands.Add(new ChangeGradeItemAchievement(GradebookIdentifier, item, calculation.Achievement.Achievement));

            ServiceLocator.SendCommands(commands);

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
