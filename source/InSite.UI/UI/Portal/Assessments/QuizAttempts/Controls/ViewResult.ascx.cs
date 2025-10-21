using System;
using System.Linq;
using System.Web.UI;

using Humanizer;

using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.UI.Portal.Assessments.QuizAttempts.Controls
{
    public partial class ViewResult : ViewQuiz
    {
        private Guid QuizId
        {
            get => (Guid)ViewState[nameof(QuizId)];
            set => ViewState[nameof(QuizId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RestartButton.Click += RestartButton_Click;
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            RedirectQuizStart(QuizId, true);
        }

        public override void LoadData(TQuiz quiz, TQuizAttempt attempt)
        {
            QuizId = quiz.QuizIdentifier;

            LoadQuiz(quiz);
            LoadAttempt(quiz, attempt);
            LoadResult(attempt.QuizGradebookIdentifier, attempt.LearnerIdentifier);
            LoadLearner(attempt.LearnerIdentifier);


            var courseId = Guid.TryParse(Request.QueryString["activity"], out var activityId)
                ? CourseSearch.BindActivityFirst(
                    x => (Guid?)x.Module.Unit.Course.CourseIdentifier,
                    x => x.ActivityIdentifier == activityId && x.QuizIdentifier == quiz.QuizIdentifier)
                : null;

            BackToCourseButton.Visible = courseId.HasValue;
            BackToCourseButton.NavigateUrl = $"/ui/portal/learning/course/{courseId}?activity={activityId}";
        }

        private void LoadQuiz(TQuiz quiz)
        {
            QuizType.Text = quiz.QuizType;
        }

        private void LoadAttempt(TQuiz quiz, TQuizAttempt attempt)
        {
            var attemptCount = GetAttemptCount(QuizId, User.Identifier);
            var lastAttempt = ServiceLocator.QuizAttemptSearch.SelectLatest(new TQuizAttemptFilter
            {
                QuizIdentifier = quiz.QuizIdentifier,
                LearnerIdentifier = attempt.LearnerIdentifier,
                IsCompleted = true
            });

            QuizTimeElapsed.Text = "N/A";

            if (lastAttempt != null && lastAttempt.AttemptCompleted.HasValue && lastAttempt.AttemptStarted.HasValue)
            {
                var timeElapsed = lastAttempt.AttemptCompleted.Value - lastAttempt.AttemptStarted.Value;
                var secondsElapsed = Math.Round(timeElapsed.TotalSeconds, MidpointRounding.AwayFromZero);

                QuizTimeElapsed.Text = secondsElapsed.Seconds().Humanize();
            }

            QuizAttemptNumber.Text = attemptCount.ToString("n0");

            RestartButton.Visible = quiz.AttemptLimit <= 0 || attemptCount < quiz.AttemptLimit;
        }

        private void LoadResult(Guid gradebookId, Guid learnerId)
        {
            ResultAccuracy.Text = "N/A";
            ResultMistakes.Text = "N/A";
            ResultWpm.Text = "N/A";
            ResultCpm.Text = "N/A";

            var gradebook = ServiceLocator.RecordSearch.GetGradebook(gradebookId, x => x.Items);
            if (gradebook == null)
                return;

            var itemType = GradeItemType.Score.GetName();
            var numberFormat = GradeItemFormat.Number.GetName();
            var pointFormat = GradeItemFormat.Point.GetName();
            var percentFormat = GradeItemFormat.Percent.GetName();

            SetResult(QuizGradeItem.Accuracy, ResultAccuracy);
            SetResult(QuizGradeItem.Speed, ResultSpeed);
            SetResult(QuizGradeItem.Mistakes, ResultMistakes);
            SetResult(QuizGradeItem.CharsPerMin, ResultCpm);
            SetResult(QuizGradeItem.WordsPerMin, ResultWpm);
            FieldKph.Visible = SetResult(QuizGradeItem.KeystrokesPerHour, ResultKph);

            bool SetResult(QuizGradeItem item, ITextControl output)
            {
                var formatName = item.Format.GetName();
                var gradeItem = gradebook.Items.FirstOrDefault(x => x.GradeItemName == item.FullName
                    && x.GradeItemType == itemType && x.GradeItemFormat == formatName);
                if (gradeItem == null)
                    return false;

                var progress = ServiceLocator.RecordSearch.GetProgress(gradebookId, gradeItem.GradeItemIdentifier, learnerId);
                if (progress == null)
                    return false;

                if (item.Format == GradeItemFormat.Number)
                {
                    if (!progress.ProgressNumber.HasValue)
                        return false;

                    output.Text = progress.ProgressNumber.Value.ToString("n0");
                }
                else if (item.Format == GradeItemFormat.Percent)
                {
                    if (!progress.ProgressPercent.HasValue)
                        return false;

                    output.Text = progress.ProgressPercent.Value.ToString("p0");
                }
                else
                    return false;

                return true;
            }
        }

        private void LoadLearner(Guid learnerId)
        {
            var learner = ServiceLocator.ContactSearch.GetPerson(learnerId, Organization.Identifier);

            LearnerFirstName.Text = learner.UserFirstName;
            LearnerLastName.Text = learner.UserLastName;
            LearnerEmail.Text = learner.UserEmail;
        }
    }
}