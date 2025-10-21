using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Portal.Surveys.Responses
{
    public partial class Complete : ResponseSessionControl
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
                : Translate("Your response to this survey is complete!");

            SurveyFormInstructionsAlert.Visible = Current.IsValid;
        }

        private void BindButtons()
        {
            ReviewButton.Visible = Current.Survey != null && Current.Survey.UserFeedback != UserFeedbackType.Disabled;
            ReviewButton.NavigateUrl = ResponseSessionNavigator.GetReviewPageUrl(Current.SessionIdentifier).ToString();

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