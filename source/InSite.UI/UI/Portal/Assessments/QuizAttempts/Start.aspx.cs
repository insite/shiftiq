using System;

using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Assessments.QuizAttempts.Controls;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.QuizAttempts
{
    public partial class Start : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            View.ControlAdded += View_ControlAdded;
        }

        private void View_ControlAdded(object sender, EventArgs e)
        {
            var repeater = (ViewQuiz)((DynamicControl)sender).GetControl();
            repeater.Alert += (s, a) => ScreenStatus.AddMessage(a.Type, a.Text);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            ConfigureLayout();
            LoadQuiz();
        }

        private void ConfigureLayout()
        {
            PortalMaster.SidebarVisible(false);
            PortalMaster.HideBreadcrumbsAndTitle();
        }

        private void LoadQuiz()
        {
            TQuiz quiz = null;
            TQuizAttempt attempt = null;
            ViewQuiz view = null;

            if (Guid.TryParse(Request.QueryString["attempt"], out var attemptId))
            {
                (quiz, attempt, view) = LoadAttempt(attemptId);
            }
            else if (Guid.TryParse(Request.QueryString["quiz"], out var quizId))
            {
                (quiz, view) = LoadQuiz(quizId);
            }
            else if (Guid.TryParse(Request.QueryString["activity"], out var activityId))
            {
                var activityQuizId = CourseSearch.BindActivityFirst(x => x.QuizIdentifier, x => x.ActivityIdentifier == activityId);
                if (activityQuizId.HasValue)
                    (quiz, view) = LoadQuiz(activityQuizId.Value);
            }

            if (quiz == null || quiz.OrganizationIdentifier != Organization.Identifier || view == null)
            {
                HttpResponseHelper.Redirect(RelativeUrl.PortalHomeUrl);
                return;
            }

            QuizTitle.InnerText = quiz.QuizName;

            PageHelper.AutoBindHeader(this);

            view.LoadData(quiz, attempt);
        }

        private (TQuiz, ViewQuiz) LoadQuiz(Guid quizId)
        {
            var quiz = ServiceLocator.QuizSearch.Select(quizId);
            var view = (ViewWelcome)View.LoadControl("~/UI/Portal/Assessments/QuizAttempts/Controls/ViewWelcome.ascx");
            view.NeedRestart = Request.QueryString["restart"] == "1";

            return (quiz, view);
        }

        private (TQuiz, TQuizAttempt, ViewQuiz) LoadAttempt(Guid attemptId)
        {
            var attempt = ServiceLocator.QuizAttemptSearch.Select(attemptId);
            if (attempt == null)
                return (null, null, null);

            var quiz = ServiceLocator.QuizSearch.Select(attempt.QuizIdentifier);
            if (quiz == null)
                return (null, null, null);

            ViewQuiz view;

            if (attempt.AttemptStarted.HasValue)
                view = (ViewQuiz)View.LoadControl("~/UI/Portal/Assessments/QuizAttempts/Controls/ViewResult.ascx");
            else if (quiz.QuizType == QuizType.TypingSpeed)
                view = (ViewQuiz)View.LoadControl("~/UI/Portal/Assessments/QuizAttempts/Controls/ViewTypingSpeed.ascx");
            else if (quiz.QuizType == QuizType.TypingAccuracy)
                view = (ViewQuiz)View.LoadControl("~/UI/Portal/Assessments/QuizAttempts/Controls/ViewTypingAccuracy.ascx");
            else
                throw ApplicationError.Create("Unexpected quiz type: " + quiz.QuizType);

            return (quiz, attempt, view);
        }
    }
}