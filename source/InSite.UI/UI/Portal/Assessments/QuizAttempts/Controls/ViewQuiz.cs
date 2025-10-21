using System;

using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Constant;
using Shift.Common.Events;

namespace InSite.UI.Portal.Assessments.QuizAttempts.Controls
{
    public abstract class ViewQuiz : BaseUserControl
    {
        public event AlertHandler Alert;

        protected void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        public abstract void LoadData(TQuiz quiz, TQuizAttempt attempt);

        protected static void RedirectQuizStart(Guid quizId, bool restart)
        {
            var url = HttpRequestHelper.GetCurrentWebUrl();
            url.QueryString["quiz"] = quizId.ToString();
            url.QueryString.Remove("attempt");
            url.QueryString.Remove("restart");

            if (restart)
                url.QueryString["restart"] = "1";

            HttpResponseHelper.Redirect(url);
        }

        protected static void RedirectQuizAttempt(Guid attemptId)
        {
            var url = HttpRequestHelper.GetCurrentWebUrl();
            url.QueryString["attempt"] = attemptId.ToString();
            url.QueryString.Remove("quiz");
            url.QueryString.Remove("restart");

            HttpResponseHelper.Redirect(url);
        }

        protected static void RedirectQuizResult(Guid attemptId)
        {
            var url = HttpRequestHelper.GetCurrentWebUrl();
            url.QueryString["attempt"] = attemptId.ToString();
            url.QueryString.Remove("quiz");
            url.QueryString.Remove("restart");

            HttpResponseHelper.Redirect(url);
        }

        protected static int GetAttemptCount(Guid quizId, Guid learnerId)
        {
            return ServiceLocator.QuizAttemptSearch.Count(new TQuizAttemptFilter
            {
                QuizIdentifier = quizId,
                LearnerIdentifier = learnerId
            });
        }
    }
}