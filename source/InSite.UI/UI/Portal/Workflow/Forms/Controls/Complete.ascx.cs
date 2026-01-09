using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class Complete : SubmissionSessionControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindTitle();
            BindInstructions();
            BindButtons();
        }

        private void BindTitle()
        {
            SurveyFormTitle.InnerText = GetTitle();
        }

        private void BindInstructions()
        {
            var instructions = GetInstructions();

            SurveyFormInstructions.Text = instructions.HasValue()
                ? instructions
                : Translate("Your submission to this form is complete!");

            SurveyFormInstructionsAlert.Visible = Current.IsValid;
        }

        private void BindButtons()
        {
            ReviewButton.Visible = Current.Survey != null && Current.Survey.UserFeedback != UserFeedbackType.Disabled;
            ReviewButton.NavigateUrl = SubmissionSessionNavigator.GetReviewPageUrl(Current.SessionIdentifier).ToString();

            var courseUrl = GetCourseUrl();

            BackToCourseButton.Visible = !string.IsNullOrEmpty(courseUrl);
            BackToCourseButton.NavigateUrl = courseUrl;
        }

        private string GetInstructions()
            => Current.Survey?.Content?.GetHtml(ContentLabel.CompletedInstructions, Current.Language);

        private string GetTitle()
            => Current.Survey?.Content?.Title?.Text[Current.Language];
    }
}