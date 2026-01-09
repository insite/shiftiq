using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.UI.Layout.Admin;

using Shift.Constant;

using ProgressCompletedChange = InSite.Domain.Records.ProgressCompleted2;
using ProgressIncompletedChange = InSite.Domain.Records.ProgressIncompleted;
using ProgressStartedChange = InSite.Domain.Records.ProgressStarted;

namespace InSite.Admin.Records.Scores.Forms
{
    public partial class Change : AdminBasePage
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        private Guid ItemKey => Guid.TryParse(Request["item"], out var value) ? value : Guid.Empty;

        private Guid StudentIdentifier => Guid.TryParse(Request["student"], out var value) ? value : Guid.Empty;

        private bool IsBooleanFormat
        {
            get => (bool)ViewState[nameof(IsBooleanFormat)];
            set => ViewState[nameof(IsBooleanFormat)] = value;
        }

        private string ReturnUrl => Request["backToContact"] == "1"
            ? $"/ui/admin/contacts/people/edit?contact={StudentIdentifier}&panel=grades"
            : "/ui/admin/records/scores/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProgressStatus.AutoPostBack = true;
            ProgressStatus.SelectedIndexChanged += ProgressStatus_SelectedIndexChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
                if (gradebook == null)
                    HttpResponseHelper.Redirect(ReturnUrl);

                var progress = ServiceLocator.RecordSearch.GetProgress(GradebookIdentifier, ItemKey, StudentIdentifier);
                if (progress == null)
                    HttpResponseHelper.Redirect(ReturnUrl);

                var student = ServiceLocator.ContactSearch.GetUser(StudentIdentifier);

                PageHelper.AutoBindHeader(this, null, $"{gradebook.Name} <span class='form-text'>{student.UserFullName}</span>");

                var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

                SetInputValues(data, progress, student);

                CancelButton.NavigateUrl = ReturnUrl;
            }
        }

        private void ProgressStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBooleanFieldsVisibility();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var progress = ServiceLocator.RecordSearch.GetProgress(GradebookIdentifier, ItemKey, StudentIdentifier);
            var commands = new List<Command>();

            if (IsBooleanFormat)
            {
                if (ProgressStatus.SelectedValue == "Started")
                    GetStartedCommands(progress, commands);
                else if (ProgressStatus.SelectedValue == "Completed")
                    GetCompletedCommands(progress, commands);
                else if (ProgressStatus.SelectedValue == "Incomplete")
                    GetIncompletedCommands(progress, commands);
            }
            else
                GetNonBooleanCommands(progress, commands);

            if (progress.ProgressIsIgnored != IsIgnoredYes.Checked)
                commands.Add(new IgnoreProgress(progress.ProgressIdentifier, IsIgnoredYes.Checked));

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect(ReturnUrl);
        }

        private void GetStartedCommands(QProgress progress, List<Command> commands)
        {
            var id = progress.ProgressIdentifier;
            var started = ProgressStarted.Value ?? DateTimeOffset.UtcNow;

            if (!string.Equals(progress.ProgressStatus, ProgressStartedChange.Status, StringComparison.OrdinalIgnoreCase)
                || progress.ProgressStarted != started
                )
            {
                commands.Add(new StartProgress(id, started));
            }
        }

        private void GetCompletedCommands(QProgress progress, List<Command> commands)
        {
            var id = progress.ProgressIdentifier;
            var completed = ProgressCompleted.Value;
            var percent = CompletedProgressPercent.ValueAsDecimal;

            if (percent.HasValue)
                percent /= 100;

            bool? pass;
            if (string.IsNullOrEmpty(ProgressFailOrPass.SelectedValue))
                pass = null;
            else
                pass = ProgressFailOrPass.SelectedValue == "Pass";

            bool? oldPass;
            if (string.IsNullOrEmpty(progress.ProgressPassOrFail))
                oldPass = null;
            else
                oldPass = string.Equals(progress.ProgressPassOrFail, "Pass", StringComparison.OrdinalIgnoreCase);

            int? elapsedSeconds = ElapsedSeconds.ValueAsInt;

            if (progress.ProgressStatus != ProgressCompletedChange.Status
                || progress.ProgressCompleted != completed
                || progress.ProgressPercent != percent
                || oldPass != pass
                || progress.ProgressElapsedSeconds != elapsedSeconds
                )
            {
                commands.Add(new CompleteProgress(id, completed, percent, pass, null, elapsedSeconds));
            }
        }

        private void GetIncompletedCommands(QProgress progress, List<Command> commands)
        {
            if (!string.Equals(progress.ProgressStatus, ProgressIncompletedChange.Status, StringComparison.OrdinalIgnoreCase))
                commands.Add(new IncompleteProgress(progress.ProgressIdentifier));
        }

        private void GetNonBooleanCommands(QProgress progress, List<Command> commands)
        {
            var id = progress.ProgressIdentifier;

            var percent = ScorePercent.ValueAsDecimal / 100m;
            var text = ScoreText.Text;
            var number = ScoreNumber.ValueAsDecimal;
            var points = ScorePoint.ValueAsDecimal;
            var graded = Graded.Value;
            var comment = Comment.Text;

            commands.Add(new ChangeProgressPercent(id, percent, graded));
            commands.Add(new ChangeProgressText(id, text, graded));
            commands.Add(new ChangeProgressNumber(id, number, graded));
            commands.Add(new ChangeProgressPoints(id, points, progress.ProgressMaxPoints, graded));
            commands.Add(new ChangeProgressComment(id, comment));
        }

        private void SetInputValues(GradebookState data, QProgress score, VUser student)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Event, x => x.Achievement, x => x.Framework);
            GradebookDetails.BindGradebook(gradebook, User.TimeZone);

            var item = data.FindItem(ItemKey);

            ItemName.Text = item.Name;
            ScoreType.Text = item.Type.GetDescription();

            if (item.Format != GradeItemFormat.None)
            {
                OptionName.Text = "Format";
                OptionValue.Text = item.Format.GetDescription();
            }
            else if (item.Weighting != GradeItemWeighting.None)
            {
                OptionName.Text = "Score Calculation";
                OptionValue.Text = item.Weighting.GetDescription();
            }
            else
            {
                OptionField.Visible = false;
            }

            StudentFullName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={student.UserIdentifier}\">{student.UserFullName}</a>";

            IsIgnoredYes.Checked = score.ProgressIsIgnored;
            IsIgnoredNo.Checked = !score.ProgressIsIgnored;

            IsBooleanFormat = item.Format == GradeItemFormat.Boolean;

            if (IsBooleanFormat)
                BindScoreBoolean(data, score);
            else
                BindScoreNonBoolean(data, score, item);

            Comment.Text = score.ProgressComment;
            Comment.Enabled = !data.IsLocked;

            SaveButton.Visible = !data.IsLocked;
        }

        private void BindScoreNonBoolean(GradebookState data, QProgress score, GradeItem item)
        {
            GradeField.Visible = true;
            DateGradedField.Visible = true;
            ProgressStatusField.Visible = false;
            ProgressCompletedPanel.Visible = false;
            ProgressStartedField.Visible = false;

            var isText = item.Type == GradeItemType.Score && item.Format == GradeItemFormat.Text || item.Type != GradeItemType.Score && !string.IsNullOrEmpty(score.ProgressText);
            var isPercent = !isText && (item.Type == GradeItemType.Score && item.Format == GradeItemFormat.Percent || item.Type != GradeItemType.Score && score.ProgressPercent.HasValue);
            var isPoint = !isText && !isPercent && (item.Type == GradeItemType.Score && item.Format == GradeItemFormat.Point || item.Type != GradeItemType.Score && score.ProgressPoints.HasValue);
            var isNumber = !isText && !isPercent && !isPoint && (item.Type == GradeItemType.Score && item.Format == GradeItemFormat.Number || item.Type != GradeItemType.Score && score.ProgressNumber.HasValue);

            ScorePercentField.Visible = isPercent;
            ScoreTextField.Visible = isText;
            ScoreNumberField.Visible = isNumber;
            ScorePointField.Visible = isPoint;

            if (isPercent)
            {
                ScorePercent.ValueAsDecimal = 100m * score.ProgressPercent;
                ScorePercent.Enabled = item.Type == GradeItemType.Score && !data.IsLocked;
            }
            else if (isText)
            {
                ScoreText.Text = score.ProgressText;
                ScoreText.Enabled = item.Type == GradeItemType.Score && !data.IsLocked;
            }
            else if (isNumber)
            {
                ScoreNumber.ValueAsDecimal = score.ProgressNumber;
                ScoreNumber.Enabled = item.Type == GradeItemType.Score && !data.IsLocked;
            }
            else if (isPoint)
            {
                ScorePoint.ValueAsDecimal = score.ProgressPoints;
                ScorePoint.Enabled = item.Type == GradeItemType.Score && !data.IsLocked;

                if (item.MaxPoints.HasValue)
                    OutOfLiteral.Text = $"out of {item.MaxPoints:n2}";
            }

            Graded.Value = score.ProgressGraded;
            Graded.Enabled = !data.IsLocked;
        }

        private void BindScoreBoolean(GradebookState data, QProgress score)
        {
            GradeField.Visible = false;
            DateGradedField.Visible = false;
            ProgressStatusField.Visible = true;

            if (!string.IsNullOrEmpty(score.ProgressStatus))
                ProgressStatus.SelectedValue = score.ProgressStatus;

            ProgressStatus.Enabled = !data.IsLocked
                && !string.Equals(score.ProgressStatus, ProgressCompletedChange.Status) 
                && !string.Equals(score.ProgressStatus, ProgressIncompletedChange.Status);

            ProgressCompleted.Value = score.ProgressCompleted;
            ProgressCompleted.Enabled = !data.IsLocked;

            ElapsedSeconds.ValueAsInt = score.ProgressElapsedSeconds;
            ElapsedSeconds.Enabled = !data.IsLocked;

            if (string.IsNullOrEmpty(score.ProgressPassOrFail))
                ProgressFailOrPass.SelectedIndex = 0;
            else
                ProgressFailOrPass.SelectedValue = score.ProgressPassOrFail;

            ProgressFailOrPass.Enabled = !data.IsLocked;

            CompletedProgressPercent.ValueAsDecimal = 100m * score.ProgressPercent;
            CompletedProgressPercent.Enabled = !data.IsLocked;

            ProgressStarted.Value = score.ProgressStarted;

            SetBooleanFieldsVisibility();
        }

        private void SetBooleanFieldsVisibility()
        {
            ProgressCompletedPanel.Visible = ProgressStatus.SelectedValue == "Completed";
            ProgressStartedField.Visible = ProgressStatus.SelectedValue == "Started";
        }
    }
}
