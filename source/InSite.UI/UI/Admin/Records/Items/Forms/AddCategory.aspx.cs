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
    public partial class AddCategory : AdminBasePage, IHasParentLinkParameters
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

            var category = Detail.GetInputValues();
            var item = UniqueIdentifier.Create();

            var commands = new List<ICommand>();

            commands.Add(new AddGradeItem(
                GradebookIdentifier,
                item,
                category.Code,
                category.Name,
                category.ShortName,
                category.IncludeToReport,
                GradeItemFormat.None,
                GradeItemType.Category,
                category.Weighting,
                null,
                category.ParentItem
            ));

            if (!string.IsNullOrEmpty(category.Reference))
                commands.Add(new ReferenceGradeItem(GradebookIdentifier, item, category.Reference));

            if (!string.IsNullOrEmpty(category.Hook))
                commands.Add(new ChangeGradeItemHook(GradebookIdentifier, item, category.Hook));

            if ((data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards) && category.Standards != null)
                commands.Add(new ChangeGradeItemCompetencies(GradebookIdentifier, item, category.Standards));

            if (category.PassPercent.HasValue)
                commands.Add(new ChangeGradeItemPassPercent(GradebookIdentifier, item, category.PassPercent));

            if (category.Achievement != null)
                commands.Add(new ChangeGradeItemAchievement(GradebookIdentifier, item, category.Achievement.Achievement));

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
