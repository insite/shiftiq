using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Records;
using InSite.Application.Records.Read;
using InSite.Application.Records.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class ScoreControl : UserControl
    {
        #region Constants

        public const int MaxStudents = 50;

        private const int ItemsPerPage = 11;
        protected const int StudentColumnWidth = 210;
        protected const int ScoreColumnWidth = 87;

        #endregion

        #region Classes

        [Serializable]
        private class ScoreItem
        {
            public Guid ItemKey { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
            public GradeItemFormat Format { get; set; }
            public string Abbreviation { get; set; }
            public decimal? MaxPoint { get; set; }

            public bool ShowMaxPoint => Format == GradeItemFormat.Point;

            public string Header => (ShortName ?? Abbreviation) + (Format == GradeItemFormat.Percent ? " (%)" : "");
        }

        [Serializable]
        private class StudentItem
        {
            public Guid Identifier { get; set; }
            public string FullName { get; set; }
            public string Notes { get; set; }
        }

        [Serializable]
        private class Score
        {
            public string Value { get; set; }
            public bool IsBoolean { get; set; }
            public string Comment { get; set; }
        }

        [Serializable]
        private class CategoryItem
        {
            public List<ScoreItem> ScoreItems { get; set; }
            public Dictionary<Guid, List<Score>> Scores { get; set; }
        }

        private class SavedScores
        {
            public List<ICommand> Commands { get; set; }
        }

        #endregion

        #region Properties

        private Guid GradebookIdentifier
        {
            get => (Guid)ViewState[nameof(GradebookIdentifier)];
            set => ViewState[nameof(GradebookIdentifier)] = value;
        }

        private Guid? ItemKey
        {
            get => (Guid?)ViewState[nameof(ItemKey)];
            set => ViewState[nameof(ItemKey)] = value;
        }

        private string ReturnUrl
        {
            get => (string)ViewState[nameof(ReturnUrl)];
            set => ViewState[nameof(ReturnUrl)] = value;
        }

        private Dictionary<Guid, CategoryItem> Categories
            => (Dictionary<Guid, CategoryItem>)(ViewState[nameof(Categories)] ?? (ViewState[nameof(Categories)] = new Dictionary<Guid, CategoryItem>()));

        private List<StudentItem> Students
        {
            get => (List<StudentItem>)ViewState[nameof(Students)];
            set => ViewState[nameof(Students)] = value;
        }

        public Guid[] Learners
        {
            get => ViewState[nameof(Learners)] as Guid[];
            set => ViewState[nameof(Learners)] = value;
        }

        private Guid? SelectedCategoryKey
        {
            get => ViewState[nameof(SelectedCategoryKey)] as Guid?;
            set => ViewState[nameof(SelectedCategoryKey)] = value;
        }

        private int PageIndex
        {
            get => ViewState[nameof(PageIndex)] as int? ?? 0;
            set => ViewState[nameof(PageIndex)] = value;
        }

        protected int PageCount
        {
            get
            {
                var itemCount = SelectedCategoryKey.HasValue ? Categories[SelectedCategoryKey.Value].ScoreItems?.Count ?? 0 : 0;
                var pageCount = itemCount / ItemsPerPage;

                if ((itemCount % ItemsPerPage) != 0)
                    pageCount++;

                return pageCount;
            }
        }

        protected int VisibleItems
        {
            get
            {
                if (PageIndex < PageCount - 1)
                    return ItemsPerPage;

                var remained = Categories[SelectedCategoryKey.Value].ScoreItems.Count % ItemsPerPage;

                return remained == 0 ? ItemsPerPage : remained;
            }
        }

        public static bool HideCommentsGlobal
        {
            get => HttpContext.Current.Session["InSite.Admin.Grades.Gradebooks.Controls.ScoreControl.HideComments"] as bool? ?? true;
            set => HttpContext.Current.Session["InSite.Admin.Grades.Gradebooks.Controls.ScoreControl.HideComments"] = value;
        }

        public static bool HideDatesGlobal
        {
            get => HttpContext.Current.Session["InSite.Admin.Grades.Gradebooks.Controls.ScoreControl.HideDates"] as bool? ?? true;
            set => HttpContext.Current.Session["InSite.Admin.Grades.Gradebooks.Controls.ScoreControl.HideDates"] = value;
        }

        public static bool HideIgnoreGlobal
        {
            get => HttpContext.Current.Session["InSite.Admin.Grades.Gradebooks.Controls.ScoreControl.HideIgnore"] as bool? ?? true;
            set => HttpContext.Current.Session["InSite.Admin.Grades.Gradebooks.Controls.ScoreControl.HideIgnore"] = value;
        }

        protected bool HideComments => HideCommentsCheckBox.Checked;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GradeItem.AutoPostBack = true;
            GradeItem.ValueChanged += GradebookItem_ValueChanged;

            ScoreItemHeaderRepeater.DataBinding += ScoreItemHeaderRepeater_DataBinding;

            StudentRepeater.DataBinding += StudentRepeater_DataBinding;
            StudentRepeater.ItemDataBound += StudentRepeater_ItemDataBound;

            NextButton.Click += NextButton_Click;
            PrevButton.Click += PrevButton_Click;

            SaveButton.Click += SaveButton_Click;

            HideCommentsCheckBox.AutoPostBack = true;
            HideCommentsCheckBox.CheckedChanged += HideCommentsCheckBox_CheckedChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                SaveScoresToCache();
        }

        public bool LoadData(Guid gradebookIdentifier, Guid? itemKey, string returnUrl, Guid[] learners = null)
        {
            GradebookIdentifier = gradebookIdentifier;
            ItemKey = itemKey;
            ReturnUrl = returnUrl;
            Learners = learners;

            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

            if (data.IsLocked)
                AlertStatus.AddMessage(AlertType.Error, "This gradebook is locked and cannot be modified");

            if (data.Enrollments.IsEmpty())
            {
                StudentPanel.Visible = false;
                CloseButton.Visible = true;

                SaveButton.Visible = false;
                CancelButton.Visible = false;
            }
            else if (data.Enrollments.Count > MaxStudents)
            {
                return false;
            }
            else
            {
                NoStudentsAlert.Visible = false;

                GradeItem.GradebookIdentifier = GradebookIdentifier;
                GradeItem.EnableOnlyWithChildScoreItems = true;
                GradeItem.RefreshData();
                GradeItem.ValueAsGuid = FindItemForScoringFromQueryString(data) ?? FindItemForScoring(data);

                HideCommentsCheckBox.Checked = HideCommentsGlobal;

                BindScores();
            }

            SetCancelLink();

            return true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            PageNumberLiteral.Text = (PageIndex + 1).ToString();
            PageCountLiteral.Text = PageCount.ToString();

            NextButton.Enabled = PageIndex < PageCount - 1;
            PrevButton.Enabled = PageIndex > 0;

            if (GetChanges().Commands.Any(x => !(x is AddProgress)))
                ScriptManager.RegisterStartupScript(this, GetType(), "SetLeaveConfirmation", "scoresForm.setLeaveConfirmation();", true);
        }

        #endregion

        #region Event handlers

        private void GradebookItem_ValueChanged(object sender, EventArgs e)
        {
            SetCancelLink();

            BindScores();
        }

        private void ScoreItemHeaderRepeater_DataBinding(object sender, EventArgs e)
        {
            var category = Categories[SelectedCategoryKey.Value];
            var pageScoreItems = category.ScoreItems.Skip(PageIndex * ItemsPerPage).Take(ItemsPerPage).ToList();

            ScoreItemHeaderRepeater.DataSource = pageScoreItems;
        }

        private void StudentRepeater_DataBinding(object sender, EventArgs e)
        {
            StudentRepeater.DataSource = Students;
        }

        private void StudentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var registration = (StudentItem)e.Item.DataItem;
            var contactScores = Categories[SelectedCategoryKey.Value].Scores[registration.Identifier];
            var pageScores = contactScores.Skip(PageIndex * ItemsPerPage).Take(ItemsPerPage).ToList();

            var contactScoreRepeater = (Repeater)e.Item.FindControl("ContactScoreRepeater");
            contactScoreRepeater.ItemDataBound += ContactScoreRepeater_ItemDataBound;
            contactScoreRepeater.DataSource = pageScores;
            contactScoreRepeater.DataBind();
        }

        private void ContactScoreRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var score = (Score)e.Item.DataItem;
            var color = !string.IsNullOrEmpty(score.Comment) ? "#337ab7" : "gray";

            var commentButton = (IconButton)e.Item.FindControl("CommentButton");
            commentButton.ToolTip = "Edit Comment" + (!string.IsNullOrEmpty(score.Comment) ? ":\n" + score.Comment : "");
            commentButton.Name = "comment";
            commentButton.Style["color"] = color;

            if (score.IsBoolean)
            {
                var scoreStatus = (DropDownList)e.Item.FindControl("ScoreStatus");
                if (string.IsNullOrEmpty(score.Value))
                    scoreStatus.SelectedIndex = 0;
                else if (string.Equals(score.Value, "Completed", StringComparison.OrdinalIgnoreCase))
                    scoreStatus.SelectedValue = "Completed";
                else if (string.Equals(score.Value, "Incomplete", StringComparison.OrdinalIgnoreCase))
                    scoreStatus.SelectedValue = "Incomplete";
                else
                    scoreStatus.SelectedValue = "Started";
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            PageIndex++;

            ScoreItemHeaderRepeater.DataBind();
            StudentRepeater.DataBind();
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            PageIndex--;

            ScoreItemHeaderRepeater.DataBind();
            StudentRepeater.DataBind();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
                HttpResponseHelper.Redirect(ReturnUrl);
        }

        private void HideCommentsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            HideCommentsGlobal = HideCommentsCheckBox.Checked;
        }

        #endregion

        #region Helper methods

        private void SetCancelLink()
        {
            var url = ReturnUrl;

            if (GradeItem.ValueAsGuid.HasValue)
                url += $"&scoreItem={GradeItem.ValueAsGuid}";

            CancelButton.NavigateUrl = url;
            CloseButton.NavigateUrl = url;
        }

        private Guid? FindItemForScoringFromQueryString(GradebookState data)
        {
            if (ItemKey == null)
                return null;

            var item = data.FindItem(ItemKey.Value);

            while (
                item != null
                && (
                    item.Children == null
                    || item.Children.Count == 0
                    || item.Children.Find(x => x.Type == GradeItemType.Score) == null
                )
            )
            {
                item = item.Parent;
            }

            return item?.Identifier;
        }

        private Guid? FindItemForScoring(GradebookState data)
        {
            return data.RootItems.Any(x => x.Type == GradeItemType.Score)
                ? null
                : FindItemForScoring(data.RootItems);
        }

        private Guid? FindItemForScoring(List<GradeItem> items)
        {
            foreach (var item in items)
            {
                if (item.Children.IsEmpty())
                    continue;

                if (item.Children.Find(x => x.Type == GradeItemType.Score) != null)
                    return item.Identifier;

                var childKey = FindItemForScoring(item.Children);

                if (childKey.HasValue)
                    return childKey;
            }

            return null;
        }

        private void BindScores()
        {
            SelectedCategoryKey = GradeItem.ValueAsGuid.HasValue
                ? GradeItem.ValueAsGuid.Value
                : Guid.Empty;

            var category = LoadCategory();

            var hasScoreItems = category.ScoreItems.IsNotEmpty();

            NoScoresAlert.Visible = !hasScoreItems && SelectedCategoryKey != Guid.Empty;
            ScorePanel.Visible = hasScoreItems;

            if (!hasScoreItems)
            {
                PagingPanel.Visible = false;
                return;
            }

            PageIndex = 0;

            PagingPanel.Visible = PageCount > 1;

            ScoreItemHeaderRepeater.DataBind();
            StudentRepeater.DataBind();
        }

        private CategoryItem LoadCategory()
        {
            if (Students == null)
            {
                Students = ServiceLocator.RecordSearch
                    .GetEnrollments(new QEnrollmentFilter { GradebookIdentifier = GradebookIdentifier })
                    .Select(x => new StudentItem { Identifier = x.LearnerIdentifier, FullName = x.Learner.UserFullName, Notes = x.EnrollmentComment })
                    .ToList();

                if (Learners != null)
                {
                    var exclusions = Students.Where(x => !Learners.Any(learner => learner == x.Identifier)).ToList();
                    foreach (var exclusion in exclusions)
                        Students.Remove(exclusion);
                }
            }

            if (Categories.TryGetValue(SelectedCategoryKey.Value, out var category))
                return category;

            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter { GradebookIdentifier = GradebookIdentifier });
            var children = SelectedCategoryKey != Guid.Empty ? data.FindItem(SelectedCategoryKey.Value).Children : data.RootItems;
            var scoreItems = children != null ? children.Where(x => x.Type == GradeItemType.Score).ToList() : null;

            category = new CategoryItem();

            if (scoreItems.IsNotEmpty())
            {
                var scores = new Dictionary<Guid, List<Score>>();
                foreach (var student in Students)
                {
                    var contactScores = scoreItems.Select(x =>
                    {
                        var score = progresses.Find(y => y.GradeItemIdentifier == x.Identifier && y.UserIdentifier == student.Identifier);
                        var value = GetScoreValue(x, score);
                        return new Score
                        {
                            Value = value,
                            IsBoolean = x.Format == GradeItemFormat.Boolean,
                            Comment = score?.ProgressComment
                        };
                    }).ToList();

                    scores.Add(student.Identifier, contactScores);
                }

                category.ScoreItems = scoreItems.Select(x => new ScoreItem
                {
                    ItemKey = x.Identifier,
                    Name = x.Name,
                    ShortName = !string.IsNullOrEmpty(x.ShortName) ? x.ShortName : null,
                    Abbreviation = x.Abbreviation,
                    Format = x.Format,
                    MaxPoint = x.MaxPoints
                }).ToList();

                category.Scores = scores;
            }

            Categories.Add(SelectedCategoryKey.Value, category);

            return category;
        }

        private static string GetScoreValue(GradeItem item, QProgress score)
        {
            if (score == null)
                return null;

            if (item.Type == GradeItemType.Score && item.Format == GradeItemFormat.Boolean)
                return score.ProgressStatus;

            if (score.ProgressNumber.HasValue)
                return $"{score.ProgressNumber:n2}";

            if (!string.IsNullOrEmpty(score.ProgressText))
                return score.ProgressText;

            if (score.ProgressPercent.HasValue)
                return $"{score.ProgressPercent * 100:n2}";

            if (item.Type != GradeItemType.Calculation && item.Type != GradeItemType.Category && score.ProgressPoints.HasValue)
                return $"{score.ProgressPoints:n2}";

            return null;
        }

        private void SaveScoresToCache()
        {
            if (SelectedCategoryKey == null)
                return;

            var category = Categories[SelectedCategoryKey.Value];
            if (category.ScoreItems.IsEmpty())
                return;

            var scoreStart = PageIndex * ItemsPerPage;

            for (int studentIndex = 0; studentIndex < StudentRepeater.Items.Count; studentIndex++)
            {
                var studentRepeaterItem = StudentRepeater.Items[studentIndex];
                var student = Students[studentIndex];
                var studentIdentifier = student.Identifier;
                var studentScores = category.Scores[studentIdentifier];
                var notesField = (HiddenField)studentRepeaterItem.FindControl("NotesField");

                student.Notes = notesField.Value;

                var scoreRepeater = (Repeater)studentRepeaterItem.FindControl("ContactScoreRepeater");
                for (int scoreIndex = 0; scoreIndex < scoreRepeater.Items.Count; scoreIndex++)
                {
                    var scoreItem = category.ScoreItems[scoreIndex];
                    var scoreRepeaterItem = scoreRepeater.Items[scoreIndex];
                    var score = studentScores[scoreStart + scoreIndex];

                    GetScoreFromRepeaterItem(scoreItem, scoreRepeaterItem, score);
                }
            }
        }

        private static void GetScoreFromRepeaterItem(ScoreItem scoreItem, RepeaterItem repeaterItem, Score score)
        {
            string value;

            if (scoreItem.Format == GradeItemFormat.Boolean)
            {
                var scoreStatus = (DropDownList)repeaterItem.FindControl("ScoreStatus");
                value = scoreStatus.SelectedValue;
            }
            else
            {
                var scoreTextBox = (ITextControl)repeaterItem.FindControl("ScoreTextBox");
                if (scoreItem.Format == GradeItemFormat.Text)
                    value = scoreTextBox.Text;
                else if (decimal.TryParse(scoreTextBox.Text, out var _))
                    value = scoreTextBox.Text;
                else
                    value = null;
            }

            var commentField = (HiddenField)repeaterItem.FindControl("CommentField");

            score.Value = value;
            score.Comment = !string.IsNullOrEmpty(commentField.Value) ? commentField.Value : null;
        }

        private bool SaveChanges()
        {
            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

            if (data.IsLocked)
            {
                AlertStatus.AddMessage(AlertType.Error, "Gradebook must be unlocked to save changes");
                return false;
            }

            var savedScores = GetChanges();

            if (savedScores.Commands.Count == 0)
            {
                AlertStatus.AddMessage(AlertType.Error, "No changes were made");
                return false;
            }

            foreach (var command in savedScores.Commands)
                ServiceLocator.SendCommand(command);

            ICommand[] calculateCommands;

            try
            {
                calculateCommands = GradebookCalculator.Calculate(GradebookIdentifier, null, false, ServiceLocator.RecordSearch);
            }
            catch (CalculateScoreException ex)
            {
                AlertStatus.AddMessage(AlertType.Error, ex.Message);
                return false;
            }

            ServiceLocator.SendCommands(calculateCommands);

            return true;
        }

        private SavedScores GetChanges()
        {
            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            var commands = new List<ICommand>();

            GetStudentChanges(data, commands);

            foreach (var categoryKeyValue in Categories)
            {
                var category = categoryKeyValue.Value;

                GetScoreChanges(category, commands);
            }

            return new SavedScores { Commands = commands };
        }

        private void GetStudentChanges(GradebookState data, List<ICommand> commands)
        {
            if (Students != null)
            {
                foreach (var student in Students)
                {
                    var notes = string.IsNullOrEmpty(student.Notes) ? null : student.Notes;
                    var dataStudent = data.Enrollments.Find(x => x.Learner == student.Identifier);

                    if (dataStudent != null && !string.Equals(dataStudent.Comment, notes))
                        commands.Add(new NoteGradebookUser(GradebookIdentifier, student.Identifier, student.Notes, null));
                }
            }
        }

        private void GetScoreChanges(CategoryItem category, List<ICommand> commands)
        {
            if (category.Scores == null)
                return;

            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter { GradebookIdentifier = GradebookIdentifier });

            foreach (var studentScoreKeyValue in category.Scores)
            {
                var studentIdentifier = studentScoreKeyValue.Key;
                var newScores = studentScoreKeyValue.Value;

                for (int i = 0; i < category.ScoreItems.Count; i++)
                {
                    var scoreItem = category.ScoreItems[i];
                    var itemKey = scoreItem.ItemKey;
                    var newScore = newScores[i];
                    var existingScore = progresses.Find(x => x.GradeItemIdentifier == itemKey && x.UserIdentifier == studentIdentifier);

                    if (string.IsNullOrWhiteSpace(newScore.Value)
                        && string.IsNullOrWhiteSpace(newScore.Comment)
                        && existingScore == null
                        && !progresses.Any(x => x.UserIdentifier == studentIdentifier)
                        && !HasNonEmptyChanges(studentIdentifier)
                        )
                    {
                        continue;
                    }

                    Guid progressIdentifier;
                    if (existingScore != null)
                    {
                        progressIdentifier = existingScore.ProgressIdentifier;
                    }
                    else
                    {
                        var command = ServiceLocator.RecordSearch.CreateCommandToAddProgress(null, GradebookIdentifier, itemKey, studentIdentifier);

                        commands.Add(command);

                        progressIdentifier = command.AggregateIdentifier;
                    }

                    CreateChangeCommand(scoreItem, existingScore, progressIdentifier, newScore, commands);

                    if (existingScore == null && !string.IsNullOrWhiteSpace(newScore.Comment)
                        || existingScore != null && !string.Equals(existingScore.ProgressComment, newScore.Comment)
                        )
                    {
                        commands.Add(new ChangeProgressComment(progressIdentifier, newScore.Comment));
                    }
                }
            }
        }

        private bool HasNonEmptyChanges(Guid userId)
            => Categories.Values.Any(c => c.Scores[userId].Any(s => !string.IsNullOrWhiteSpace(s.Value) || !string.IsNullOrWhiteSpace(s.Comment)));

        private void CreateChangeCommand(ScoreItem scoreItem, QProgress progress, Guid progressIdentifier, Score newScore, List<ICommand> commands)
        {
            switch (scoreItem.Format)
            {
                case GradeItemFormat.Percent:
                    CreateChangeCommandPercent(progress, progressIdentifier, newScore, commands);
                    break;
                case GradeItemFormat.Point:
                    CreateChangeCommandPoint(progress, progressIdentifier, newScore, commands);
                    break;
                case GradeItemFormat.Boolean:
                    CreateChangeCommandBoolean(progress, progressIdentifier, newScore, commands);
                    break;
                case GradeItemFormat.Number:
                    CreateChangeCommandNumber(progress, progressIdentifier, newScore, commands);
                    break;
                case GradeItemFormat.Text:
                    CreateChangeCommandText(progress, progressIdentifier, newScore, commands);
                    break;
                default:
                    throw new ArgumentException($"Unsupported format: {scoreItem.Format}");
            }
        }

        private void CreateChangeCommandBoolean(QProgress progress, Guid progressIdentifier, Score newScore, List<ICommand> commands)
        {
            if (string.Equals(progress?.ProgressStatus ?? "", newScore.Value ?? "", StringComparison.OrdinalIgnoreCase))
                return;

            if (newScore.Value == "Started")
            {
                if (string.Equals(progress?.ProgressStatus, "Completed", StringComparison.OrdinalIgnoreCase))
                {
                    commands.Add(new DeleteProgress(progressIdentifier));
                    commands.Add(new AddProgress(progressIdentifier, progress.GradebookIdentifier, progress.GradeItemIdentifier, progress.UserIdentifier));
                }

                commands.Add(new StartProgress(progressIdentifier, DateTimeOffset.UtcNow));
                return;
            }

            if (newScore.Value == "Completed")
            {
                commands.Add(new CompleteProgress(progressIdentifier, DateTimeOffset.UtcNow, null, null, null));
                return;
            }

            if (newScore.Value == "Incomplete")
            {
                commands.Add(new IncompleteProgress(progressIdentifier));
                return;
            }

            commands.Add(new DeleteProgress(progressIdentifier));
        }

        private void CreateChangeCommandPercent(QProgress progress, Guid progressIdentifier, Score newScore, List<ICommand> commands)
        {
            var oldValue = progress?.ProgressPercent;
            var newValue = decimal.TryParse(newScore.Value, out var percentValue) ? percentValue / 100m : (decimal?)null;

            if (oldValue == newValue)
                return;

            if (newValue > 1.9999m)
                newValue = 1.9999m;

            var graded = progress?.ProgressGraded != null || newValue == null
                ? progress?.ProgressGraded
                : DateTimeOffset.UtcNow;

            commands.Add(new ChangeProgressPercent(progressIdentifier, newValue, graded));
        }

        private void CreateChangeCommandPoint(QProgress progress, Guid progressIdentifier, Score newScore, List<ICommand> commands)
        {
            var oldValue = progress?.ProgressPoints;
            var newValue = decimal.TryParse(newScore.Value, out var pointValue) ? pointValue : (decimal?)null;

            if (oldValue == newValue)
                return;

            var graded = progress?.ProgressGraded != null || newValue == null
                ? progress?.ProgressGraded
                : DateTimeOffset.UtcNow;

            commands.Add(new ChangeProgressPoints(progressIdentifier, newValue, null, graded));
        }

        private void CreateChangeCommandNumber(QProgress progress, Guid progressIdentifier, Score newScore, List<ICommand> commands)
        {
            var oldValue = progress?.ProgressNumber;
            var newValue = decimal.TryParse(newScore.Value, out var numberValue) ? numberValue : (decimal?)null;

            if (oldValue == newValue)
                return;

            var graded = progress?.ProgressGraded != null || newValue == null
                ? progress?.ProgressGraded
                : DateTimeOffset.UtcNow;

            commands.Add(new ChangeProgressNumber(progressIdentifier, newValue, graded));
        }

        private void CreateChangeCommandText(QProgress progress, Guid progressIdentifier, Score newScore, List<ICommand> commands)
        {
            var oldValue = progress?.ProgressText;

            if (string.IsNullOrEmpty(oldValue) && string.IsNullOrEmpty(newScore.Value) || string.Equals(newScore.Value, oldValue))
                return;

            var graded = progress?.ProgressGraded != null || string.IsNullOrEmpty(newScore.Value)
                ? progress?.ProgressGraded
                : DateTimeOffset.UtcNow;

            commands.Add(new ChangeProgressText(progressIdentifier, newScore.Value, graded));
        }

        #endregion
    }
}