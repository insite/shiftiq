using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Home
{
    public partial class Grades : PortalBasePage
    {
        public bool HideHeading { get; set; }

        private Guid GetUserIdentifier()
        {
            if (Guid.TryParse(Request.QueryString["user"], out Guid user))
                return user;
            return User.UserIdentifier;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GradebookComboBox.AutoPostBack = true;
            GradebookComboBox.ValueChanged += GradebookComboBox_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GradebookComboBox.ListFilter.OrganizationIdentifier = Organization.Identifier;
            GradebookComboBox.ListFilter.StudentIdentifier = GetUserIdentifier();

            if (IsPostBack)
                return;

            GradebookComboBox.RefreshData();

            var model = CreateModel(GradebookComboBox.ValueAsGuid);

            if (!model.HasGrades)
            {
                GradebookComboBox.Visible = false;
                StatusAlert.AddMessage(AlertType.Information, GetDisplayText("You have no Gradebooks published to your learner profile. Start a course to see your progress here."));
            }
            else
            {
                BindModelToControls(model);
            }

            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);            
        }

        private MyGradesModel CreateModel(Guid? gradebook)
        {
            var model = new MyGradesModel();

            if (gradebook.HasValue)
            {
                var filter = new QProgressFilter { GradebookIdentifier = gradebook.Value, StudentUserIdentifier = GetUserIdentifier() };
                model.Scores = ServiceLocator.RecordSearch.GetGradebookScores(filter);

                var html = new StringBuilder();
                var items = ServiceLocator.RecordSearch.GetGradeItemHierarchies(gradebook.Value).OrderBy(x => x.PathSequence).ToList();

                html.AppendLine("<table class='table table-striped'>");
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];

                    html.Append("<tr>");

                    html.Append($"<td style='padding-left: {(item.PathDepth) * 25}px;'>");
                    html.AppendLine($"<span>{item.GradeItemName}</span>");
                    html.Append("</td>");

                    var score = model.Scores.FirstOrDefault(x => x.GradeItemIdentifier == item.GradeItemIdentifier);

                    html.Append($"<td class='text-end'>");
                    if (score != null)
                    {
                        var value = score.ProgressStatus;
                        var style = "text-dark";

                        if (score.ProgressPercent.HasValue)
                        {
                            value = $"{score.ProgressPercent:p0}";

                            if (score.ProgressPercent.Value < 0.60m)
                                style = "text-danger";
                            else if (score.ProgressPercent.Value < 0.80m)
                                style = "text-warning";
                            else
                                style = "text-success";
                        }

                        html.AppendLine($"<span class='{style}'>{value}</span>");
                    }
                    html.Append("</td>");

                    html.Append("</tr>");
                }
                html.AppendLine("</table>");

                model.ScoresHtml = html.ToString();
                model.HasGrades = true;
            }

            return model;
        }

        private void GradebookComboBox_ValueChanged(object sender, Shift.Sdk.UI.ComboBoxValueChangedEventArgs e)
        {
            BindModelToControls(CreateModel(GradebookComboBox.ValueAsGuid));
        }

        private void BindModelToControls(MyGradesModel model)
        {
            GradeItemsHtml.Text = model.ScoresHtml;
        }
    }

    public class MyGradesModel
    {
        public bool HasGrades { get; set; }
        public List<QProgress> Scores { get; set; }
        public string ScoresHtml { get; set; }
    }
}