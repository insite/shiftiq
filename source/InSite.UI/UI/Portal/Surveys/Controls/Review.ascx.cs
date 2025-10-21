using System;

using InSite.Application.Responses.Write;

using Shift.Common;

using Shift.Constant;

namespace InSite.Portal.Surveys.Responses
{
    public partial class Review : ResponseSessionControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack || !Current.IsValid)
                return;

            BindAnswers();
            Continue();

            if (Current.Survey.UserFeedback != UserFeedbackType.Disabled)
            {
                DownloadSummaryButton.NavigateUrl = $"/ui/survey/respond/review-print?session={Current.SessionIdentifier}&print=summary";
                DownloadSummaryButton.Visible = Current.Survey.UserFeedback == UserFeedbackType.Summary;
                DownloadDetailsButton.NavigateUrl = $"/ui/survey/respond/review-print?session={Current.SessionIdentifier}&print=details";
                DownloadDetailsButton.Visible = Current.Survey.UserFeedback == UserFeedbackType.Detailed;
                DownloadAnsweredButton.NavigateUrl = $"/ui/survey/respond/review-print?session={Current.SessionIdentifier}&print=answered";
                DownloadAnsweredButton.Visible = Current.Survey.UserFeedback == UserFeedbackType.Answered;
            }

            SetupBackButton();
        }

        private void SetupBackButton()
        {
            var courseUrl = GetCourseUrl();
            if (courseUrl.IsNotEmpty())
            {
                ReturnPanel.Visible = true;
                BackToCourseButton.Visible = true;
                BackToCourseButton.NavigateUrl = courseUrl;
                return;
            }

            var returnUrl = new ReturnUrl().GetReturnUrl();
            if (returnUrl.IsEmpty())
                return;

            if (returnUrl.StartsWith("/ui/portal/events/classes/register"))
            {
                ReturnPanel.Visible = true;
                BackToRegistrationButton.Visible = true;
                BackToRegistrationButton.NavigateUrl = returnUrl;
            }
        }

        private void BindAnswers()
        {
            var questions = Current.Survey.Questions;
            if (questions.Count == 0)
                Navigator.RedirectToNextPage(Current.SessionIdentifier, Current.PageNumber);

            Details.LoadData(Current, Navigator, Current.Survey.UserFeedback, false);
        }

        private void Continue()
        {
            ServiceLocator.SendCommand(new ReviewResponseSession(Current.SessionIdentifier));
        }
    }
}