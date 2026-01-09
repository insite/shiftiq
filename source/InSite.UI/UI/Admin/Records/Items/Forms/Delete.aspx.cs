using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Items.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        private Guid ItemKey => Guid.TryParse(Request["item"], out var value) ? value : Guid.Empty;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"id={GradebookIdentifier}&panel=config" : null;

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

            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Achievement);
            if (gradebook == null)
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");

            var outlineUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=config";
            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

            if (!BindItem(gradebook, data, ItemKey))
                HttpResponseHelper.Redirect(outlineUrl);

            CancelButton.NavigateUrl = outlineUrl;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            Server.ScriptTimeout = 60 * 5; // 5 minutes

            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            var item = data?.FindItem(ItemKey);

            if (item != null && data.IsOpen)
                DeleteItem(item);

            HttpResponseHelper.Redirect(CancelButton.NavigateUrl);
        }

        private void DeleteItem(GradeItem item)
        {
            var children = new List<Guid>();
            GetChildren(item, children);

            var progresses = ServiceLocator.RecordSearch
                .GetGradebookScores(new QProgressFilter { GradebookIdentifier = GradebookIdentifier })
                .Where(x => x.GradeItemIdentifier == item.Identifier || children.Contains(x.GradeItemIdentifier))
                .ToList();

            var commands = new List<ICommand>();

            foreach (var progress in progresses)
                commands.Add(new DeleteProgress(progress.ProgressIdentifier));

            for (int i = children.Count - 1; i >= 0; i--)
                commands.Add(new DeleteGradeItem(GradebookIdentifier, children[i]));

            commands.Add(new DeleteGradeItem(GradebookIdentifier, ItemKey));

            ServiceLocator.SendCommands(commands);

            Course2Store.ClearCache(Organization.Identifier);
        }

        private bool BindItem(QGradebook gradebook, GradebookState data, Guid itemKey)
        {
            var item = data.FindItem(itemKey);

            if (item == null)
                return false;

            var title = $"{item.Name} <span class='form-text'>for {data.Name}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            ItemCode.Text = item.Code;
            ItemName.Text = item.Name;
            ParentItemName.Text = item.Parent?.Name ?? "None";

            AchievementName.Text = "None";
            if (gradebook.Achievement != null)
                AchievementName.Text = $"<a href=\"/ui/admin/records/achievements/outline?id={gradebook.Achievement?.AchievementIdentifier}\">{gradebook.Achievement?.AchievementTitle}</a>";

            GradebookName.Text = $"<a href=\"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}\">{gradebook.GradebookTitle}</a>";

            var children = new List<Guid>();
            GetChildren(item, children);

            ChildrenCount.Text = $"{children.Count:n0}";

            var progressCount = ServiceLocator.RecordSearch
                .GetGradebookScores(new QProgressFilter { GradebookIdentifier = GradebookIdentifier })
                .Where(x => x.GradeItemIdentifier == item.Identifier || children.Contains(x.GradeItemIdentifier))
                .Count();

            ScoresCount.Text = $"{progressCount:n0}";

            NoDelete.Visible = progressCount > 0;
            DeleteScoresCheckBox.Visible = progressCount > 0;
            DeleteChildrenCheckbox.Text = children.Count > 0 ? "Delete Item with all its children" : "Delete Item";

            return true;
        }

        private static void GetChildren(GradeItem item, List<Guid> result)
        {
            if (item.Children.IsEmpty())
                return;

            foreach (var child in item.Children)
            {
                result.Add(child.Identifier);

                GetChildren(child, result);
            }
        }
    }
}