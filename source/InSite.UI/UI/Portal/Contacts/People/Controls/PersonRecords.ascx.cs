using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class PersonRecords : BaseUserControl
    {
        private class ScoreItem
        {
            public string GradeItemNameStyle { get; set; }
            public string GradeItemName { get; set; }
            public string ProgressStatusClass { get; set; }
            public string ProgressStatus { get; set; }
        }

        private Guid UserId
        {
            get => (Guid)ViewState[nameof(UserId)];
            set => ViewState[nameof(UserId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GradebookComboBox.AutoPostBack = true;
            GradebookComboBox.ValueChanged += GradebookComboBox_ValueChanged;
        }

        private void GradebookComboBox_ValueChanged(object sender, Shift.Sdk.UI.ComboBoxValueChangedEventArgs e)
        {
            LoadGradebook();
        }

        public int LoadData(Guid organizationId, Guid userId)
        {
            UserId = userId;

            GradebookComboBox.ListFilter.OrganizationIdentifier = organizationId;
            GradebookComboBox.ListFilter.StudentIdentifier = userId;
            GradebookComboBox.RefreshData();

            var gradebookId = GradebookComboBox.ValueAsGuid;

            ScoreRepeater.Visible = gradebookId.HasValue;
            GradebookComboBox.Visible = gradebookId.HasValue;
            NoGradebooks.Visible = gradebookId == null;

            if (gradebookId == null)
                return 0;

            LoadGradebook();

            return GradebookComboBox.Items.Count;
        }

        private void LoadGradebook()
        {
            var gradebookId = GradebookComboBox.ValueAsGuid;

            var scores = GetScores(gradebookId.Value, UserId);

            ScoreRepeater.DataSource = scores;
            ScoreRepeater.DataBind();
        }

        private static List<ScoreItem> GetScores(Guid gradebookId, Guid userId)
        {
            var filter = new QProgressFilter
            {
                GradebookIdentifier = gradebookId,
                StudentUserIdentifier = userId,
            };

            var scores = ServiceLocator.RecordSearch.GetGradebookScores(filter);
            var items = ServiceLocator.RecordSearch.GetGradeItemHierarchies(gradebookId).OrderBy(x => x.PathSequence).ToList();

            var result = new List<ScoreItem>();

            foreach (var gradeItem in items)
            {
                var score = scores.FirstOrDefault(x => x.GradeItemIdentifier == gradeItem.GradeItemIdentifier);
                var scoreItem = GetScoreItem(gradeItem, score);

                result.Add(scoreItem);
            }

            return result;
        }

        private static ScoreItem GetScoreItem(VGradeItemHierarchy gradeItem, QProgress score)
        {
            var scoreItem = new ScoreItem
            {
                GradeItemNameStyle = $"padding-left: {12 + gradeItem.PathDepth * 25}px;",
                GradeItemName = gradeItem.GradeItemName,
            };

            if (score == null)
                return scoreItem;

            if (score.ProgressPercent.HasValue)
            {
                scoreItem.ProgressStatus = $"{score.ProgressPercent:p0}";

                if (score.ProgressPercent.Value < 0.60m)
                    scoreItem.ProgressStatusClass = "text-danger";
                else if (score.ProgressPercent.Value < 0.80m)
                    scoreItem.ProgressStatusClass = "text-warning";
                else
                    scoreItem.ProgressStatusClass = "text-success";
            }
            else
            {
                scoreItem.ProgressStatus = score.ProgressStatus;
                scoreItem.ProgressStatusClass = "text-dark";
            }

            return scoreItem;
        }
    }
}