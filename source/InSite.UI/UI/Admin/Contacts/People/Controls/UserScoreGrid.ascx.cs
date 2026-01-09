using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class UserScoreGrid : UserControl
    {
        public class Result
        {
            public int All { get; set; }
            public int IncludeToReport { get; set; }
        }

        [Serializable]
        private class GradeGridItem
        {
            public string Url { get; set; }
            public Guid GradebookIdentifier { get; set; }
            public Guid? GradebookItemKey { get; set; }
            public string Name { get; set; }
            public Guid? AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public int Level { get; set; }
            public string ScoreValue { get; set; }
            public string Comment { get; set; }
            public bool IncludeToReport { get; set; }
        }

        protected Guid UserIdentifier
        {
            get => (Guid)ViewState[nameof(UserIdentifier)];
            set => ViewState[nameof(UserIdentifier)] = value;
        }

        private List<GradeGridItem> Items
        {
            get => (List<GradeGridItem>)ViewState[nameof(Items)];
            set => ViewState[nameof(Items)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;

            ShowNotIncludedToReport.AutoPostBack = true;
            ShowNotIncludedToReport.SelectedIndexChanged += ShowNotIncludedToReport_SelectedIndexChanged;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void ShowNotIncludedToReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        public Result LoadData(Guid userIdentifier)
        {
            UserIdentifier = userIdentifier;

            Items = new List<GradeGridItem>();

            var scores = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter
            {
                StudentUserIdentifier = userIdentifier,
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier
            }, x => x.Gradebook);

            var gradebookIdentifiers = scores.Select(x => x.GradebookIdentifier).Distinct().ToList();

            foreach (var gradebookIdentifier in gradebookIdentifiers)
            {
                var gradebook = ServiceLocator.RecordSearch.GetGradebookState(gradebookIdentifier);
                var gradebookQuery = scores.Find(x => x.GradebookIdentifier == gradebookIdentifier).Gradebook;

                var gradebookItem = new GradeGridItem
                {
                    Url = $"/ui/admin/records/gradebooks/outline?id={gradebookIdentifier}",
                    GradebookIdentifier = gradebookIdentifier,
                    Name = gradebook.Name,
                    AchievementIdentifier = gradebook.Achievement,
                    AchievementTitle = gradebookQuery.Achievement?.AchievementTitle,
                    Level = 0,
                    ScoreValue = null
                };

                Items.Add(gradebookItem);

                AddItems(userIdentifier, gradebookIdentifier, gradebook, Items, gradebook.RootItems, 1, true);
            }

            NoScores.Visible = Items.Count == 0;
            ScorePanel.Visible = Items.Count > 0;

            LoadData();

            return new Result
            {
                All = Items.Count(x => !string.IsNullOrEmpty(x.ScoreValue)),
                IncludeToReport = Items.Count(x => !string.IsNullOrEmpty(x.ScoreValue) && x.IncludeToReport)
            };
        }

        private void LoadData()
        {
            var filteredItems = Items;

            if (!string.IsNullOrEmpty(FilterText.Text))
                filteredItems = filteredItems.Where(x => x.Name.IndexOf(FilterText.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            if (ShowNotIncludedToReport.SelectedValue == "Hide")
                filteredItems = filteredItems.Where(x => x.Level == 0 || x.IncludeToReport).ToList();

            ItemRepeater.DataSource = filteredItems;
            ItemRepeater.DataBind();
        }

        private void AddItems(Guid userIdentifier, Guid gradebookIdentifier, GradebookState dataGradebook, List<GradeGridItem> output, List<GradeItem> input, int level, bool parentIncludeToReport)
        {
            if (input.IsEmpty())
                return;

            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(
                new QProgressFilter { GradebookIdentifier = gradebookIdentifier, StudentUserIdentifier = UserIdentifier },
                x => x.GradeItem);

            foreach (var inputItem in input)
            {
                var achievementTitle = inputItem.Achievement != null
                    ? ServiceLocator.AchievementSearch.GetAchievement(inputItem.Achievement.Achievement)?.AchievementTitle
                    : null;

                var score = progresses.Find(x => x.GradeItemIdentifier == inputItem.Identifier && x.UserIdentifier == userIdentifier);
                var scoreValue = GradebookHelper.GetScoreValue(score, inputItem);

                var outputItem = new GradeGridItem
                {
                    Name = inputItem.Name,
                    GradebookIdentifier = gradebookIdentifier,
                    GradebookItemKey = inputItem.Identifier,
                    AchievementIdentifier = inputItem.Achievement?.Achievement,
                    AchievementTitle = achievementTitle,
                    Level = level,
                    ScoreValue = scoreValue,
                    Comment = score?.ProgressComment,
                    IncludeToReport = parentIncludeToReport && inputItem.IsReported
                };

                output.Add(outputItem);

                AddItems(userIdentifier, gradebookIdentifier, dataGradebook, output, inputItem.Children, level + 1, outputItem.IncludeToReport);
            }
        }

        protected static string GetLocalTime(object item)
        {
            if (item == null)
                return null;

            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone, nullValue: string.Empty);
        }
    }
}