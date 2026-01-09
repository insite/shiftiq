using System;

using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Scores.Forms
{
    public partial class Delete : AdminBasePage
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        private Guid ItemKey => Guid.TryParse(Request["item"], out var value) ? value : Guid.Empty;

        private Guid StudentIdentifier => Guid.TryParse(Request["student"], out var value) ? value : Guid.Empty;

        private string ReturnUrl => Request["backToContact"] == "1"
            ? $"/ui/admin/contacts/people/edit?contact={StudentIdentifier}&panel=grades"
            : "/ui/admin/records/scores/search";

        private bool IsBooleanFormat
        {
            get => (bool)ViewState[nameof(IsBooleanFormat)];
            set => ViewState[nameof(IsBooleanFormat)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
                if (gradebook == null || gradebook.Tenant != Organization.OrganizationIdentifier || gradebook.IsLocked)
                    HttpResponseHelper.Redirect(ReturnUrl);

                var progress = ServiceLocator.RecordSearch.GetProgress(GradebookIdentifier, ItemKey, StudentIdentifier);
                if (progress == null)
                    HttpResponseHelper.Redirect(ReturnUrl);

                var student = PersonSearch.Select(Organization.Identifier, StudentIdentifier, x => x.User);

                PageHelper.AutoBindHeader(this, null, $"{student.User.FullName} <span class='form-text'>{gradebook.Name}</span>");

                var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

                SetValues(data, progress, student);

                CancelButton.NavigateUrl = ReturnUrl;
            }
        }
        private string GetLocalTime(DateTimeOffset? date)
        {
            return date != null ? date.Format(User.TimeZone) : "N/A";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var progressId = ServiceLocator.RecordSearch.GetProgressIdentifier(GradebookIdentifier, ItemKey, StudentIdentifier);
            if (progressId.HasValue)
                ServiceLocator.SendCommand(new DeleteProgress(progressId.Value));

            HttpResponseHelper.Redirect(ReturnUrl);
        }

        private void SetValues(GradebookState data, QProgress score, Person student)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Event, x => x.Achievement, x => x.Framework);

            UserName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={student.UserIdentifier}\">{student.User.FullName}</a>";
            GradebookTitle.Text = $"<a href=\"/ui/admin/records/gradebooks/outline?id={gradebook.GradebookIdentifier}\">{gradebook.GradebookTitle}</a>";

            var item = data.FindItem(ItemKey);

            ItemName.Text = $"<a href=\"/ui/admin/records/scores/change?gradebook={GradebookIdentifier}&item={ItemKey}&student={StudentIdentifier}\">{item.Name}</a>" ;
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


            IsBooleanFormat = item.Format == GradeItemFormat.Boolean;

            if (IsBooleanFormat)
                BindScoreBoolean(data, score);
            else
                BindScoreNonBoolean(data, score, item);

            Comment.Text = !string.IsNullOrEmpty(score.ProgressComment) ? score.ProgressComment : "None";
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
                ScorePercent.Text = (100m * score.ProgressPercent).ToString();
            else if (isText)
                ScoreText.Text = score.ProgressText;
            else if (isNumber)
                ScoreNumber.Text = score.ProgressNumber.ToString();
            else if (isPoint)
            {
                ScorePoint.Text = score.ProgressPoints.ToString();

                if (item.MaxPoints.HasValue)
                    OutOfLiteral.Text = $"out of {item.MaxPoints:n2}";
            }

            Graded.Text = GetLocalTime(score.ProgressGraded);
        }

        private void BindScoreBoolean(GradebookState data, QProgress score)
        {
            GradeField.Visible = false;
            DateGradedField.Visible = false;
            ProgressStatusField.Visible = true;

            if (!string.IsNullOrEmpty(score.ProgressStatus))
                ProgressStatus.Text = score.ProgressStatus;

            ProgressCompleted.Text = GetLocalTime(score.ProgressCompleted);

            ElapsedSeconds.Text = score.ProgressElapsedSeconds.ToString();

            if (string.IsNullOrEmpty(score.ProgressPassOrFail))
                ProgressFailOrPass.Text = "Unknown";
            else
                ProgressFailOrPass.Text = score.ProgressPassOrFail;

            CompletedProgressPercent.Text = (100m * score.ProgressPercent).ToString();

            ProgressStarted.Text = GetLocalTime(score.ProgressStarted);

            SetBooleanFieldsVisibility();
        }

        private void SetBooleanFieldsVisibility()
        {
            ProgressCompletedPanel.Visible = ProgressStatus.Text == "Completed";
            ProgressStartedField.Visible = ProgressStatus.Text == "Started";
        }
    }
}