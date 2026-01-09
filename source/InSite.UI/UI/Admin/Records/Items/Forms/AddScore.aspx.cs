using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Gradebooks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Items.Forms
{
    public partial class AddScore : AdminBasePage, IHasParentLinkParameters
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

        private void CodeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            args.IsValid = !data.ContainsCode(args.Value);
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
            var item = UniqueIdentifier.Create();

            var commands = new List<ICommand>();

            commands.Add(new AddGradeItem(
                GradebookIdentifier,
                item,
                score.Code,
                score.Name,
                score.ShortName,
                score.IncludeToReport,
                score.Format,
                GradeItemType.Score,
                GradeItemWeighting.None,
                null,
                score.ParentItem
            ));

            if (!string.IsNullOrEmpty(score.Reference))
                commands.Add(new ReferenceGradeItem(GradebookIdentifier, item, score.Reference));

            if (!string.IsNullOrEmpty(score.Hook))
                commands.Add(new ChangeGradeItemHook(GradebookIdentifier, item, score.Hook));

            if ((data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards) && score.Standards != null)
                commands.Add(new ChangeGradeItemCompetencies(GradebookIdentifier, item, score.Standards));

            if (score.MaxPoint.HasValue)
                commands.Add(new ChangeGradeItemMaxPoints(GradebookIdentifier, item, score.MaxPoint));

            if (score.PassPercent.HasValue)
                commands.Add(new ChangeGradeItemPassPercent(GradebookIdentifier, item, score.PassPercent));

            if (score.Achievement != null)
                commands.Add(new ChangeGradeItemAchievement(GradebookIdentifier, item, score.Achievement.Achievement));

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
