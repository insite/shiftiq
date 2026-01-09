using System;
using System.Linq;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class GoTo : SubmissionSessionControl
    {
        protected GoToType GoToType
        {
            get
            {
                return Page.Request.QueryString["to"].ToEnum(GoToType.Next);
            }
        }

        protected SubmissionSessionHelper.GoToInfo GoToInfo
        {
            get => (SubmissionSessionHelper.GoToInfo)ViewState[nameof(GoToInfo)];
            set => ViewState[nameof(GoToInfo)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DebugButton.Click += (sender, args) => Continue();

            Pagination.PageChanged += (s, a) => RedirectToPage(Pagination.PageIndex + 1);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack || !Current.IsValid)
                return;

            if (Current.PageNumber <= 0 || Current.PageNumber > Current.PageCount)
            {
                ErrorAlert.AddMessage(
                    AlertType.Error,
                    "fas fa-bomb",
                    $"<strong>{Translate("Invalid URL")}:</strong> {Translate("A valid form page number is required")}.");
                return;
            }

            // Check for termination.

            if (GoToType == GoToType.Next)
            {
                var questions = Current.Survey.GetPage(Current.PageNumber).Questions;
                if (questions.Any(q => q.Type == SurveyQuestionType.Terminate))
                {
                    Navigator.RedirectToConfirmPage(Current.SessionIdentifier);
                    return;
                }
            }

            CalculateGoToPageNumber();

            if (!Current.Debug)
            {
                Continue();
            }
            else
            {
                LoadPagination();
                InfoPanel.Visible = true;
            }
        }

        private void LoadPagination()
        {
            Pagination.PageSize = 1;
            Pagination.ItemsCount = Current.PageCount;
            Pagination.PageIndex = Current.PageNumber - 1;
        }

        private void RedirectToPage(int pageNumber)
        {
            Navigator.RedirectToAnswerPage(Current.SessionIdentifier, pageNumber, null);
        }

        private void CalculateGoToPageNumber()
        {
            // By default, the next page is simply the previous number in the sequence. Otherwise, if the user selected
            // an option with a branch then that determines the next page.

            if (GoToType == GoToType.Previous)
                GoToInfo = SubmissionSessionHelper.GoToPreviousPage(Current);

            // By default, the next page is simply the next number in the sequence. Otherwise, if the user selected
            // an option with a branch then that determines the next page.

            else
                GoToInfo = SubmissionSessionHelper.GoToNextPage(Current);
        }


        private void Continue()
        {
            var goTo = GoToInfo ?? new SubmissionSessionHelper.GoToInfo(1);

            Navigator.RedirectToAnswerPage(Current.SessionIdentifier, goTo.Page, goTo.Question);
        }
    }
}