using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.QuizAttempts.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.QuizAttempts
{
    public partial class View : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/assessment/quizzes-attempts/view";

        public static string GetNavigateUrl(Guid attemptId) => NavigateUrl + "?attempt=" + attemptId;

        public static void Redirect(Guid attemptId) => HttpResponseHelper.Redirect(GetNavigateUrl(attemptId));

        private Guid? AttemptIdentifier => Guid.TryParse(Request["attempt"], out Guid value) ? value : (Guid?)null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open();
        }

        private void Open()
        {
            var attempt = AttemptIdentifier.HasValue
                ? ServiceLocator.QuizAttemptSearch.Select(AttemptIdentifier.Value)
                : null;
            if (attempt == null)
                HttpResponseHelper.Redirect(Search.NavigateUrl, true);

            var learner = ServiceLocator.PersonSearch.GetPerson(attempt.LearnerIdentifier, attempt.OrganizationIdentifier, x => x.User);
            PersonDetail.BindPerson(learner, User.TimeZone);

            PageHelper.AutoBindHeader(this, null, learner.FullName);

            QuizType.Text = attempt.QuizType;
            QuizName.Text = attempt.QuizName;
            QuizName.NavigateUrl = Quizzes.Edit.GetNavigateUrl(attempt.QuizIdentifier);

            AttemptStartedOutput.Text = attempt.AttemptStarted.Format(User.TimeZone, nullValue: "N/A");
            AttemptCompletedOutput.Text = attempt.AttemptCompleted.Format(User.TimeZone, nullValue: "N/A");
            AttemptTimeTaken.Text = attempt.AttemptDuration.HasValue
                ? ((double)attempt.AttemptDuration).Seconds().Humanize(2, minUnit: TimeUnit.Second)
                : "N/A";
            AttemptIsPassing.Text = !attempt.AttemptIsPassing.HasValue
                ? "N/A"
                : attempt.AttemptIsPassing.Value
                    ? "<span class='badge bg-success'>Pass</span>"
                    : "<span class='badge bg-danger'>Fail</span>";
            AttemptMistakes.Text = attempt.AttemptMistakes?.ToString("n0") ?? "N/A";
            AttemptKphField.Visible = attempt.QuizType == Shift.Constant.QuizType.TypingAccuracy;
            AttemptKph.Text = attempt.AttemptKeystrokesPerHour?.ToString("n0") ?? "N/A";
            AttemptWpm.Text = attempt.AttemptWordsPerMin?.ToString("n0") ?? "N/A";
            AttemptCpm.Text = attempt.AttemptCharsPerMin?.ToString("n0") ?? "N/A";
            AttemptAccuracy.Text = attempt.AttemptAccuracy?.ToString("p0") ?? "N/A";

            AnswerPanel.Visible = BindAnswers(attempt);

            CloseButton.NavigateUrl = Search.NavigateUrl;
        }

        private bool BindAnswers(TQuizAttempt attempt)
        {
            if (attempt.QuizData.IsEmpty())
                return false;

            if (attempt.QuizType == Shift.Constant.QuizType.TypingSpeed)
                BindTypingSpeed(attempt);
            else if (attempt.QuizType == Shift.Constant.QuizType.TypingAccuracy)
                BindTypingAccuracy(attempt);
            else
                return false;

            return true;
        }

        private void BindTypingSpeed(TQuizAttempt attempt)
        {
            AnswerMultiView.SetActiveView(TypingSpeedView);
            TypingSpeedQuestion.InnerText = attempt.QuizData;
            TypingSpeedAnswer.InnerText = attempt.AttemptData;
        }

        private void BindTypingAccuracy(TQuizAttempt attempt)
        {
            AnswerMultiView.SetActiveView(TypingAccuracyView);

            var quizData = JsonConvert.DeserializeObject<TQuizAttemptTypingAccuracyQuestion[]>(attempt.QuizData);
            var attemptData = attempt.AttemptData.IsNotEmpty()
                ? JsonConvert.DeserializeObject<string[]>(attempt.AttemptData)
                : null;

            var questionNumber = 1;
            var answerIndex = 0;

            TypingAccuracyQuestionRepeater.ItemDataBound += TypingAccuracyQuestionRepeater_ItemDataBound;
            TypingAccuracyQuestionRepeater.DataSource = quizData.Select(q => new
            {
                Number = questionNumber++,
                Columns = q.Columns.Select(c => new
                {
                    Rows = c.Rows.Select(r =>
                    {
                        var index = answerIndex++;
                        return new
                        {
                            Index = index,
                            Label = r.Label,
                            Value = r.Value,
                            Answer = attemptData?.TryGetItem(index)
                        };
                    }).ToArray()
                }).ToArray()
            }).ToArray();
            TypingAccuracyQuestionRepeater.DataBind();
        }

        private void TypingAccuracyQuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var questionColumnRepeater = (Repeater)e.Item.FindControl("QuestionColumnRepeater");
            questionColumnRepeater.ItemDataBound += ColumnRepeater_ItemDataBound;
            questionColumnRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Columns");
            questionColumnRepeater.DataBind();

            var answerColumnRepeater = (Repeater)e.Item.FindControl("AnswerColumnRepeater");
            answerColumnRepeater.ItemDataBound += ColumnRepeater_ItemDataBound;
            answerColumnRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Columns");
            answerColumnRepeater.DataBind();
        }

        private void ColumnRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var rowRepeater = (Repeater)e.Item.FindControl("RowRepeater");
            rowRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Rows");
            rowRepeater.DataBind();
        }
    }
}