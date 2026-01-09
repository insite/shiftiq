using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Records.Gradebooks.Reports;
using InSite.Admin.Records.Reports.Gradebooks.Controls;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Records;
using InSite.Application.Records.Read;
using InSite.Application.Records.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

using Label = System.Web.UI.WebControls.Label;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class OutlineProgressList : BaseUserControl
    {
        #region Constants

        private const int ItemsPerPage = 11;
        protected const int StudentColumnWidth = 210;
        protected const int ScoreColumnWidth = 83;

        #endregion

        #region Events

        public event EventHandler Calculated;
        private void OnCalculated() =>
            Calculated?.Invoke(this, new EventArgs());

        public event EventHandler Released;
        private void OnReleased() =>
            Released?.Invoke(this, new EventArgs());

        public event IntValueHandler ScoresCreated;
        private void OnScoresCreated(int count) =>
            ScoresCreated?.Invoke(this, new IntValueArgs(count));

        public event AlertHandler Alert;
        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

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
        }

        [Serializable]
        private class Score
        {
            public string Value { get; set; }
            public string Graded { get; set; }
            public string Comment { get; set; }
            public string Hint { get; set; }
            public bool HasEmptyScore { get; set; }
            public int? Restart { get; set; }
            public bool IsIgnored { get; set; }
        }

        [Serializable]
        private class StudentItem
        {
            public Guid GradebookIdentifier { get; set; }
            public Guid Identifier { get; set; }
            public string FullName { get; set; }
            public string Notes { get; set; }
            public DateTimeOffset? Added { get; set; }
            public string AddedHtml { get; set; }
            public Guid? PeriodIdentifier { get; set; }
            public string PeriodName { get; set; }

            public int EnrollmentRestart { get; set; }
            public string EnrollmentRestartHtml
            {
                get
                {
                    if (EnrollmentRestart > 0)
                        return Shift.Common.Humanizer.ToQuantity(EnrollmentRestart, "Restart");
                    return string.Empty;
                }
            }
        }

        #endregion

        #region Properties

        protected Guid GradebookIdentifier
        {
            get => (Guid)ViewState[nameof(GradebookIdentifier)];
            set => ViewState[nameof(GradebookIdentifier)] = value;
        }

        public Guid[] Learners
        {
            get => ViewState[nameof(Learners)] as Guid[];
            set => ViewState[nameof(Learners)] = value;
        }

        private List<ScoreItem> ScoreItems
        {
            get => (List<ScoreItem>)ViewState[nameof(ScoreItems)];
            set => ViewState[nameof(ScoreItems)] = value;
        }

        private Dictionary<Guid, List<Score>> Scores
        {
            get => (Dictionary<Guid, List<Score>>)ViewState[nameof(Scores)];
            set => ViewState[nameof(Scores)] = value;
        }

        private List<StudentItem> Students
        {
            get => (List<StudentItem>)ViewState[nameof(Students)];
            set => ViewState[nameof(Students)] = value;
        }

        private int ScorePageIndex
        {
            get => ViewState[nameof(ScorePageIndex)] as int? ?? 0;
            set => ViewState[nameof(ScorePageIndex)] = value;
        }

        protected int ScorePageCount
        {
            get
            {
                var itemCount = ScoreItems?.Count ?? 0;
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
                if (ScorePageCount == 0 || ScorePageIndex < ScorePageCount - 1)
                    return ItemsPerPage;

                var remained = ScoreItems.Count % ItemsPerPage;

                return remained == 0 ? ItemsPerPage : remained;
            }
        }

        protected bool IsLocked { get; set; }

        protected bool HideComments => HideCommentsCheckBox.Checked;

        protected bool HideDates => HideDatesCheckBox.Checked;

        public UserMode UserMode
        {
            get => ViewState[nameof(UserMode)] as UserMode? ?? UserMode.Portal;
            set => ViewState[nameof(UserMode)] = value;
        }

        private bool HideIgnoreScoreCheckbox
        {
            get => Organization.Toolkits?.Gradebooks?.HideIgnoreScoreCheckbox == true && UserMode != UserMode.Admin;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchButton.Click += (x, y) => LoadStudents();

            ReleaseButton.Click += ReleaseButton_Click;
            CalculateButton.Click += CalculateButton_Click;

            GradeItemIdentifier.AutoPostBack = true;
            GradeItemIdentifier.ValueChanged += GradebookItem_ValueChanged;

            PeriodIdentifier.AutoPostBack = true;
            PeriodIdentifier.ValueChanged += (x, y) => LoadStudents();

            ScoreItemHeaderRepeater.DataBinding += ScoreItemHeaderRepeater_DataBinding;

            StudentRepeater.DataBinding += StudentRepeater_DataBinding;
            StudentRepeater.ItemDataBound += StudentRepeater_ItemDataBound;
            StudentRepeater.ItemCommand += StudentRepeater_ItemCommand;

            NextButton.Click += NextButton_Click;
            PrevButton.Click += PrevButton_Click;

            SaveCommentButton.Click += SaveCommentButton_Click;
            SaveNotesButton.Click += SaveNotesButton_Click;

            HideCommentsCheckBox.AutoPostBack = true;
            HideCommentsCheckBox.CheckedChanged += HideCommentsCheckBox_CheckedChanged;

            HideDatesCheckBox.AutoPostBack = true;
            HideDatesCheckBox.CheckedChanged += HideDatesCheckBox_CheckedChanged;

            HideIgnoreCheckBox.AutoPostBack = true;
            HideIgnoreCheckBox.CheckedChanged += HideIgnoreCheckBox_CheckedChanged;

            StudentPagination.PageChanged += StudentPagination_PageChanged;

            PeriodUpdatePanel.Request += PeriodUpdatePanel_Request;

            SummaryReportButton.Click += SummaryReportButton_Click;

            CreateScoresButton.Click += CreateScoresButton_Click;

            PeriodSelector.AutoPostBack = true;
            PeriodSelector.ValueChanged += PeriodSelector_ValueChanged;

            SaveIsIgnored.Click += SaveIsIgnored_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            PageNumberLiteral.Text = (ScorePageIndex + 1).ToString();
            PageCountLiteral.Text = ScorePageCount.ToString();

            NextButton.Enabled = ScorePageIndex < ScorePageCount - 1;
            PrevButton.Enabled = ScorePageIndex > 0;
        }

        #endregion

        #region Event handlers

        private void ReleaseButton_Click(object sender, EventArgs e)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            if (gradebook.IsLocked)
            {
                OnAlert(AlertType.Error, $"Can't grant credentials: the gradebook is locked.");
                return;
            }

            if (Calculate(gradebook, true))
            {
                ServiceLocator.SendCommand(new LockGradebook(GradebookIdentifier));

                OnReleased();
            }
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            if (gradebook.IsLocked)
            {
                OnAlert(AlertType.Error, $"Can't calculate scores: the gradebook is locked.");
                return;
            }

            if (Calculate(gradebook, false))
                OnCalculated();
        }

        private void GradebookItem_ValueChanged(object sender, EventArgs e)
        {
            SetScoresLink();

            BindScores();

            StudentRepeater.DataBind();
        }

        private void ScoreItemHeaderRepeater_DataBinding(object sender, EventArgs e)
        {
            var pageScoreItems = ScoreItems.Skip(ScorePageIndex * ItemsPerPage).Take(ItemsPerPage).ToList();

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

            var voidStudent = (IconLink)e.Item.FindControl("VoidStudent");
            voidStudent.Visible = !IsLocked && UserMode == UserMode.Admin;

            var repeater = (Repeater)e.Item.FindControl("ProgressRepeater");
            repeater.Visible = Scores.IsNotEmpty();

            if (Scores.IsEmpty())
                return;

            var student = (StudentItem)e.Item.DataItem;
            var contactScores = Scores[student.Identifier];

            var pageScores = contactScores
                .Skip(ScorePageIndex * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(x => new LearnerRepeaterItem
                {
                    Key = $"{Students.IndexOf(student)}:{contactScores.IndexOf(x)}",
                    Value = x.Value,
                    Graded = x.Graded,
                    Comment = x.Comment,
                    Hint = x.Hint,
                    HasEmptyScore = x.HasEmptyScore,
                    Restart = x.Restart,
                    IsIgnored = x.IsIgnored
                })
                .ToList();

            foreach (var i in pageScores)
            {
                if ((i.Restart ?? 0) > 0)
                    i.Value += $"&nbsp;<span class='form-text' title='{Shift.Common.Humanizer.ToQuantity(i.Restart.Value, "Restart")}'>({i.Restart})</span>";
            }

            repeater.ItemDataBound += ProgressRepeater_ItemDataBound;
            repeater.DataSource = pageScores;
            repeater.DataBind();
        }

        private void StudentRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Print")
            {
                var report = (StudentMarksReport)LoadControl("~/UI/Admin/Records/Reports/Gradebooks/Controls/StudentMarksReport.ascx");
                var studentIdentifier = Guid.Parse((string)e.CommandArgument);

                var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier,
                    x => x.Event,
                    x => x.Achievement);

                var user = UserSearch.Select(studentIdentifier);

                var person = PersonSearch.Select(CurrentSessionState.Identity.Organization.Identifier, studentIdentifier,
                    x => x.EmployerGroup,
                    x => x.HomeAddress,
                    x => x.ShippingAddress);

                var gradebookData = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

                report.LoadReport(user, person, gradebook, gradebookData);

                var siteContent = new StringBuilder();
                using (var stringWriter = new StringWriter(siteContent))
                {
                    using (var htmlWriter = new HtmlTextWriter(stringWriter))
                        report.RenderControl(htmlWriter);
                }

                var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                    MarginTop = 5,
                    HeaderUrl = "",
                    HeaderSpacing = 7,
                };

                var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

                Response.SendFile($"StudentScoreReport-{user.FullName}-{gradebook.Achievement?.AchievementTitle}", "pdf", data);
            }
        }

        private void ProgressRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var value = (string)DataBinder.Eval(e.Item.DataItem, "Value");
            var hint = (string)DataBinder.Eval(e.Item.DataItem, "Hint");

            var valueLabel = (Label)e.Item.FindControl("Value");
            valueLabel.Text = value;
            valueLabel.ToolTip = hint;

            var comment = (string)DataBinder.Eval(e.Item.DataItem, "Comment");
            var color = !string.IsNullOrEmpty(comment) ? "#337ab7" : "gray";

            var commentButton = (IconButton)e.Item.FindControl("CommentButton");
            commentButton.ToolTip = "Edit Comment" + (!string.IsNullOrEmpty(comment) ? ":\n" + comment : "");
            commentButton.Name = "comment";
            commentButton.Style["color"] = color;

            var ignorePanel = e.Item.FindControl("IgnorePanel");
            ignorePanel.Visible = !HideIgnoreScoreCheckbox;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            ScorePageIndex++;

            ScoreItemHeaderRepeater.DataBind();
            StudentRepeater.DataBind();
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            ScorePageIndex--;

            ScoreItemHeaderRepeater.DataBind();
            StudentRepeater.DataBind();
        }

        private void SaveCommentButton_Click(object sender, EventArgs e)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            if (gradebook.IsLocked)
            {
                OnAlert(AlertType.Error, $"Can't save comment: the gradebook is locked.");
                return;
            }

            var keyParts = CommentKey.Value.Split(new[] { ':' });
            var studentIndex = int.Parse(keyParts[0]);
            var scoreIndex = int.Parse(keyParts[1]);
            var comment = CommentTextBox.Text.Trim();

            if (string.IsNullOrEmpty(comment))
                comment = null;

            var student = Students[studentIndex];
            var cachedScore = Scores[student.Identifier][scoreIndex];

            if (string.Equals(cachedScore.Comment, comment))
                return;

            cachedScore.Comment = comment;

            var scoreItem = ScoreItems[scoreIndex];
            var progressId = ServiceLocator.RecordSearch.GetProgressIdentifier(GradebookIdentifier, scoreItem.ItemKey, student.Identifier);

            if (progressId == null)
            {
                var command = ServiceLocator.RecordSearch.CreateCommandToAddProgress(null, GradebookIdentifier, scoreItem.ItemKey, student.Identifier);

                ServiceLocator.SendCommand(command);

                progressId = command.AggregateIdentifier;
            }

            ServiceLocator.SendCommand(new ChangeProgressComment(progressId.Value, comment));
        }

        private void SaveNotesButton_Click(object sender, EventArgs e)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            if (gradebook.IsLocked)
            {
                OnAlert(AlertType.Error, $"Can't save notes: the gradebook is locked.");
                return;
            }

            var studentIndex = int.Parse(StudentIndex.Value);
            var notes = NotesTextBox.Text.Trim();

            if (string.IsNullOrEmpty(notes))
                notes = null;

            var student = Students[studentIndex];

            if (string.Equals(student.Notes, notes))
                return;

            student.Notes = notes;

            ServiceLocator.SendCommand(new NoteGradebookUser(GradebookIdentifier, student.Identifier, student.Notes, null));
        }

        private void SaveIsIgnored_Click(object sender, EventArgs e)
        {
            if (HideIgnoreScoreCheckbox)
                return;

            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            if (gradebook.IsLocked)
                return;

            var isIgnored = bool.Parse(IsIgnored.Value);
            var keyParts = IgnoredScoreKey.Value.Split(new[] { ':' });
            var studentIndex = int.Parse(keyParts[0]);
            var scoreIndex = int.Parse(keyParts[1]);
            var student = Students[studentIndex];
            var cachedScore = Scores[student.Identifier][scoreIndex];

            if (cachedScore.IsIgnored == isIgnored)
                return;

            cachedScore.IsIgnored = isIgnored;

            var scoreItem = ScoreItems[scoreIndex];
            var progressId = ServiceLocator.RecordSearch.GetProgressIdentifier(GradebookIdentifier, scoreItem.ItemKey, student.Identifier);
            var commands = new List<Command>();

            if (progressId == null)
            {
                var command = ServiceLocator.RecordSearch.CreateCommandToAddProgress(null, GradebookIdentifier, scoreItem.ItemKey, student.Identifier);
                commands.Add(command);
                commands.Add(new ChangeProgressPercent(command.AggregateIdentifier, null, DateTimeOffset.UtcNow));

                progressId = command.AggregateIdentifier;
            }

            commands.Add(new IgnoreProgress(progressId.Value, isIgnored));

            ServiceLocator.SendCommands(commands);
        }

        private void PeriodSelector_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            if (gradebook.IsLocked)
            {
                OnAlert(AlertType.Error, $"Can't save period: the gradebook is locked.");
                return;
            }

            var studentIndex = int.Parse(PeriodStudentIndex.Value);
            var periodIdentifier = PeriodSelector.Value;
            var student = Students[studentIndex];

            student.PeriodIdentifier = periodIdentifier;
            student.PeriodName = PeriodSelector.Item?.Text;

            ServiceLocator.SendCommand(new ChangeGradebookUserPeriod(GradebookIdentifier, student.Identifier, periodIdentifier));

            PeriodSelector.Value = null;
        }

        private void HideCommentsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ScoreControl.HideCommentsGlobal = HideCommentsCheckBox.Checked;
        }

        private void HideDatesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ScoreControl.HideDatesGlobal = HideDatesCheckBox.Checked;
        }

        private void HideIgnoreCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ScoreControl.HideIgnoreGlobal = HideIgnoreCheckBox.Checked;
        }

        private void StudentPagination_PageChanged(object sender, Pagination.PageChangedEventArgs e)
        {
            BindStudents();
        }

        private void PeriodUpdatePanel_Request(object sender, StringValueArgs e)
        {
            PeriodSelector.Value = e.Value.IsNotEmpty() ? Guid.Parse(e.Value) : (Guid?)null;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(OutlineProgressList),
                "show_period",
                $"scoreList.periodWindow.show();",
                true);
        }

        private void SummaryReportButton_Click(object sender, EventArgs e)
        {
            var data = SummaryListReport.GetXlsx(GradebookIdentifier);
            Response.SendFile("summary-list", "xlsx", data);
        }

        private void CreateScoresButton_Click(object sender, EventArgs e)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            if (gradebook.IsLocked)
            {
                OnAlert(AlertType.Error, $"Can't create missed scores: the gradebook is locked.");
                return;
            }

            var count = GradebookHelper.CreateMissingScores(GradebookIdentifier);

            OnScoresCreated(count);
        }

        #endregion

        #region Public methods

        public int LoadData(GradebookState gradebook, Guid? selectedScoreItem, Guid[] learners)
        {
            GradebookIdentifier = gradebook.Identifier;
            IsLocked = gradebook.IsLocked;

            NoScoresAlert.Visible = false;
            PagingPanel.Visible = false;

            HideCommentsCheckBox.Checked = ScoreControl.HideCommentsGlobal;
            HideDatesCheckBox.Checked = ScoreControl.HideDatesGlobal;
            HideIgnoreCheckBox.Checked = ScoreControl.HideIgnoreGlobal;
            HideIgnoreCheckBox.Visible = !HideIgnoreScoreCheckbox;

            Learners = learners;

            var studentCount = LoadStudents(gradebook);

            PeriodIdentifier.Enabled = studentCount > 0;

            var oldItemKey = selectedScoreItem ?? GradeItemIdentifier.ValueAsGuid;
            GradeItemIdentifier.GradebookIdentifier = GradebookIdentifier;
            GradeItemIdentifier.RefreshData();
            GradeItemIdentifier.ValueAsGuid = oldItemKey;

            ReleaseButton.Visible = !IsLocked;
            CalculateButton.Visible = !IsLocked;

            ScoresLink.Visible = !IsLocked && studentCount <= ScoreControl.MaxStudents;

            AddStudentsLink.Visible = AssignPeriodButton.Visible = !IsLocked && UserMode == UserMode.Admin;

            AddStudentsLink.NavigateUrl = $"/ui/admin/records/gradebooks/add-learner?gradebook={GradebookIdentifier}";

            AssignPeriodButton.NavigateUrl = $"/ui/admin/records/gradebooks/assign-period?gradebook={GradebookIdentifier}";

            CommentWindow.Title = IsLocked ? "View Comment" : "Edit Comment";

            SaveCommentButton.Visible = !IsLocked;
            CancelCommentButton.Visible = !IsLocked;
            CloseCommentButton.Visible = IsLocked;

            SaveNotesButton.Visible = !IsLocked;
            CancelNotesButton.Visible = !IsLocked;
            CloseNotesButton.Visible = IsLocked;

            CreateScoresButton.Visible = !IsLocked && CurrentSessionState.Identity.IsGranted(ActionName.Admin_Records_Gradebooks_Outline_CreateScores);

            SetScoresLink();

            return studentCount;
        }

        private int LoadStudents()
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

            return LoadStudents(gradebook);
        }

        private int LoadStudents(GradebookState gradebook)
        {
            var studentCount = ServiceLocator.RecordSearch.CountEnrollments(new QEnrollmentFilter
            {
                GradebookIdentifier = GradebookIdentifier,
                PeriodIdentifier = PeriodIdentifier.Value,
                LearnerFullName = LearnerName.Text
            });

            StudentPagination.PageIndex = 0;
            StudentPagination.ItemsCount = studentCount;
            StudentPagination.Visible = StudentPagination.PageCount > 1;

            NoStudentsAlert.Visible = studentCount == 0;

            GradeItemIdentifier.Enabled = studentCount > 0;

            ScorePanel.Visible = studentCount > 0;

            BindStudents(gradebook);

            return studentCount;
        }

        #endregion

        #region Helper methods

        private void SetScoresLink()
        {
            string url;
            if (UserMode == UserMode.Admin)
                url = $"/ui/admin/records/gradebooks/scores?gradebook={GradebookIdentifier}";
            else
                url = $"/ui/admin/records/gradebooks/instructors/gradebook-change?gradebook={GradebookIdentifier}";

            if (GradeItemIdentifier.ValueAsGuid.HasValue)
                url += $"&item={GradeItemIdentifier.ValueAsGuid}";

            ScoresLink.NavigateUrl = url;
        }

        private void BindStudents()
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

            BindStudents(gradebook);
        }

        private void BindStudents(GradebookState gradebook)
        {
            if (Learners != null && Learners.Count() == 0)
            {
                Students = new List<StudentItem>();
                StudentRepeater.DataBind();
                return;
            }

            Students = ServiceLocator.RecordSearch
                .GetEnrollments(
                    new QEnrollmentFilter
                    {
                        GradebookIdentifier = GradebookIdentifier,
                        PeriodIdentifier = PeriodIdentifier.Value,
                        LearnerFullName = LearnerName.Text,
                        Paging = Paging.SetSkipTake(StudentPagination.ItemsSkip, StudentPagination.ItemsTake)
                    },
                    x => x.Period
                )
                .Select(x => new StudentItem
                {
                    GradebookIdentifier = x.GradebookIdentifier,
                    Identifier = x.LearnerIdentifier,
                    FullName = x.Learner.UserFullName,
                    Notes = x.EnrollmentComment,
                    Added = x.EnrollmentStarted,
                    EnrollmentRestart = x.EnrollmentRestart,
                    AddedHtml = x.EnrollmentStarted.HasValue ? TimeZones.FormatDateOnly(x.EnrollmentStarted.Value, CurrentSessionState.Identity.User.TimeZone) : null,
                    PeriodIdentifier = x.PeriodIdentifier,
                    PeriodName = x.Period?.PeriodName
                })
                .ToList();

            if (Learners != null)
            {
                var exclusions = Students.Where(x => !Learners.Any(learner => learner == x.Identifier)).ToList();
                foreach (var exclusion in exclusions)
                    Students.Remove(exclusion);
            }

            if (Students.Count > 0)
                BindScores(gradebook);

            StudentRepeater.DataBind();
        }

        private void BindScores()
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

            BindScores(gradebook);
        }

        private void BindScores(GradebookState gradebook)
        {
            LoadScores(gradebook);

            var hasScoreItems = ScoreItems.IsNotEmpty();

            NoScoresAlert.Visible = !hasScoreItems;
            PagingPanel.Visible = ScorePageCount > 1;
            ScoreItemHeaderRepeater.Visible = hasScoreItems;

            ScorePageIndex = 0;

            if (hasScoreItems)
                ScoreItemHeaderRepeater.DataBind();
        }

        private void LoadScores(GradebookState gradebook)
        {
            var categoryItem = GradeItemIdentifier.ValueAsGuid.HasValue ? gradebook.FindItem(GradeItemIdentifier.ValueAsGuid.Value) : null;

            ScoreItems = new List<ScoreItem>();

            if (categoryItem != null)
            {
                ScoreItems.Add(new ScoreItem
                {
                    ItemKey = categoryItem.Identifier,
                    Name = categoryItem.Name,
                    ShortName = !string.IsNullOrEmpty(categoryItem.ShortName) ? categoryItem.ShortName : null,
                    Format = categoryItem.Format,
                    Abbreviation = categoryItem.Abbreviation,
                    MaxPoint = categoryItem.MaxPoints
                });
            }

            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter { GradebookIdentifier = GradebookIdentifier });
            var emptyScores = CreateEmptyScores(gradebook, progresses);

            LoadStudentScores(categoryItem, progresses, emptyScores);
            LoadScoreItems(gradebook, categoryItem, progresses, emptyScores);
        }

        private Dictionary<Guid, Dictionary<Guid, bool>> CreateEmptyScores(GradebookState data, List<QProgress> progresses)
        {
            var emptyScores = new Dictionary<Guid, Dictionary<Guid, bool>>();
            foreach (var student in Students)
            {
                var studentEmptyScores = new Dictionary<Guid, bool>();
                emptyScores.Add(student.Identifier, studentEmptyScores);

                CreateEmptyScores(student.Identifier, data.RootItems, progresses, studentEmptyScores);
            }

            return emptyScores;
        }

        private bool CreateEmptyScores(Guid student, List<GradeItem> items, List<QProgress> progresses, Dictionary<Guid, bool> studentEmptyScores)
        {
            if (items.IsEmpty())
                return false;

            bool hasEmpty = false;

            foreach (var item in items)
            {
                var progress = progresses.Find(x => x.GradeItemIdentifier == item.Identifier && x.UserIdentifier == student);
                var areChildrenEmpty = CreateEmptyScores(student, item.Children, progresses, studentEmptyScores);

                bool isEmpty;
                if (progress != null && progress.ProgressIsIgnored)
                    isEmpty = false;
                else
                {
                    isEmpty = item.Type == GradeItemType.Score
                        ? (item.Format == GradeItemFormat.Point || item.Format == GradeItemFormat.Percent)
                            && (progress == null || progress.NoScore)
                        : areChildrenEmpty || progress == null || progress.NoScore;
                }

                studentEmptyScores.Add(item.Identifier, isEmpty);

                if (isEmpty && !hasEmpty)
                    hasEmpty = true;
            }

            return hasEmpty;
        }

        private void LoadStudentScores(GradeItem categoryItem, List<QProgress> progresses, Dictionary<Guid, Dictionary<Guid, bool>> emptyScores)
        {
            Scores = new Dictionary<Guid, List<Score>>();

            foreach (var student in Students)
            {
                var scores = new List<Score>();
                Scores.Add(student.Identifier, scores);

                if (categoryItem != null)
                {
                    var score = progresses.Find(y => y.GradeItemIdentifier == categoryItem.Identifier && y.UserIdentifier == student.Identifier);
                    var value = GetScoreValue(categoryItem, score);
                    var hint = GetScoreHint(score);

                    scores.Add(new Score
                    {
                        Value = value,
                        Graded = score?.ProgressGraded != null ? TimeZones.FormatDateOnly(score.ProgressGraded.Value, CurrentSessionState.Identity.User.TimeZone) : null,
                        Comment = score?.ProgressComment,
                        Hint = hint,
                        HasEmptyScore = emptyScores[student.Identifier][categoryItem.Identifier],
                        Restart = score?.ProgressRestartCount,
                        IsIgnored = score != null && score.ProgressIsIgnored
                    });
                }
            }
        }

        private void LoadScoreItems(GradebookState data, GradeItem categoryItem, List<QProgress> progresses, Dictionary<Guid, Dictionary<Guid, bool>> emptyScores)
        {
            var scoreItems = categoryItem != null ? categoryItem.Children : data.RootItems;
            if (scoreItems.IsEmpty())
                return;

            foreach (var student in Students)
            {
                var studentEmptyScores = emptyScores[student.Identifier];

                var contactScores = scoreItems.Select(x =>
                {
                    var score = progresses.Find(y => y.GradeItemIdentifier == x.Identifier && y.UserIdentifier == student.Identifier);
                    var value = GetScoreValue(x, score);
                    var hint = GetScoreHint(score);

                    return new Score
                    {
                        Value = value,
                        Graded = score?.ProgressGraded != null ? TimeZones.FormatDateOnly(score.ProgressGraded.Value, CurrentSessionState.Identity.User.TimeZone) : null,
                        Comment = score?.ProgressComment,
                        Hint = hint,
                        HasEmptyScore = studentEmptyScores[x.Identifier],
                        Restart = score?.ProgressRestartCount,
                        IsIgnored = score != null && score.ProgressIsIgnored
                    };
                }).ToList();

                Scores[student.Identifier].AddRange(contactScores);
            }

            ScoreItems.AddRange(scoreItems.Select(x => new ScoreItem
            {
                ItemKey = x.Identifier,
                Name = x.Name,
                ShortName = !string.IsNullOrEmpty(x.ShortName) ? x.ShortName : null,
                Format = x.Format,
                Abbreviation = x.Abbreviation,
                MaxPoint = x.MaxPoints
            }));
        }

        private static string GetScoreHint(QProgress score)
        {
            if (score == null)
                return null;

            if (score.ProgressPercent == null || score.ProgressPoints == null || score.ProgressMaxPoints == null)
                return null;

            var point = $"{score.ProgressPoints:n2}";
            var maxPoint = $"{score.ProgressMaxPoints:n2}";

            return $"{point} / {maxPoint}";
        }

        private static string GetScoreValue(GradeItem item, QProgress score)
        {
            if (score == null)
                return null;

            if (score.ProgressIsIgnored && score.ProgressGraded == null && score.NoScore)
                return "N/A";

            if (item.Format == GradeItemFormat.Boolean)
                return $"{score.ProgressStatus}";

            if (score.ProgressNumber.HasValue)
                return $"{score.ProgressNumber:n2}";

            if (!string.IsNullOrEmpty(score.ProgressText))
                return score.ProgressText;

            if (score.ProgressPercent.HasValue)
                return $"{score.ProgressPercent.Value * 100:n2}%";

            if (item.Type != GradeItemType.Calculation && item.Type != GradeItemType.Category && score.ProgressPoints.HasValue)
                return $"{score.ProgressPoints:n2}";

            return null;
        }

        private bool Calculate(GradebookState gradebook, bool publish)
        {
            if (gradebook.IsLocked)
                return false;

            ICommand[] commands;

            try
            {
                commands = GradebookCalculator.Calculate(GradebookIdentifier, null, publish, ServiceLocator.RecordSearch);
            }
            catch (CalculateScoreException ex)
            {
                OnAlert(AlertType.Error, ex.Message);
                return false;
            }

            ServiceLocator.SendCommands(commands);

            return true;
        }

        #endregion

        #region Databinding helpers

        protected static string GetProgressCellStyle(object o)
        {
            var hasEmptyScore = (bool)DataBinder.Eval(o, "HasEmptyScore");

            return hasEmptyScore
                ? "text-align:center;background-color:#fef1f4;"
                : "text-align:center";
        }

        protected static string GetProgressCellTooltip(object o)
        {
            var hasEmptyScore = (bool)DataBinder.Eval(o, "HasEmptyScore");

            return hasEmptyScore
                ? "One or more scores does not have specified value"
                : string.Empty;
        }

        #endregion
    }

    internal class LearnerRepeaterItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Graded { get; set; }
        public string Comment { get; set; }
        public string Hint { get; set; }
        public bool HasEmptyScore { get; set; }
        public int? Restart { get; set; }
        public bool IsIgnored { get; set; }
    }
}