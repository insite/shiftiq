using System;
using System.Linq;

using InSite.Application.Responses.Write;
using InSite.Common.Web;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class Confirm : SubmissionSessionControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SubmitButton.Click += (s, a) => Continue();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Current.SessionIdentifier != Guid.Empty)
            {
                PreviousButton.NavigateUrl = SubmissionSessionNavigator.GetResumePageUrl(Current.SessionIdentifier).ToString();

                SurveyFormTitle.InnerText = Current.Survey.Content?.Title?.Text[Current.Language];
                SurveyFormInstructions.Text = GetInstructions();
            }
            else
            {
                SurveyFormTitle.Visible = false;
                InstructionPanel.Visible = false;
                ButtonPanel.Visible = false;
            }
        }

        private string GetInstructions()
        {
            var instructions = Current.Survey.Content?.GetHtml(ContentLabel.EndingInstructions, Current.Language);

            if (instructions.HasValue())
                return instructions;

            var buttonText = GetDisplayText("Submit My Submission");

            instructions = GetDisplayHtml("Form Submission Completed: Ending Instructions")
                .Replace("$ConfirmMyResponse", buttonText);

            return instructions;
        }

        private void Continue()
        {
            var session = ServiceLocator.SurveySearch.GetResponseSession(Current.SessionIdentifier);

            if(session != null && string.Equals(session.ResponseSessionStatus, ResponseSessionStatus.Completed.ToString(), StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.Redirect("/");

            var dateCompleted = DateTimeOffset.UtcNow;

            ServiceLocator.SendCommand(new CompleteResponseSession(Current.SessionIdentifier, DateTimeOffset.Now));
            ServiceLocator.SendCommand(new LockResponseSession(Current.SessionIdentifier));

            RefreshUserIdentity(dateCompleted);

            var question = Current.Survey.Questions.Where(x => x.Attribute != null && x.Attribute.Contains("Consent")).FirstOrDefault();
            if (question != null)
            {
                var options = Current.Session.QResponseOptions
                    .Where(x => x.SurveyQuestionIdentifier == question.Identifier)
                    .OrderBy(x => x.OptionSequence)
                    .ToArray();

                if (options.IsNotEmpty())
                {
                    if (options[0].ResponseOptionIsSelected)
                        ServiceLocator.SendCommand(new TermsConsentResponseSession(Current.SessionIdentifier,
                        question.Identifier,
                        Current.Survey.Tenant,
                        Current.UserIdentifier));
                }
            }

            var returnUrl = new ReturnUrl().GetReturnUrl();
            if (returnUrl.IsNotEmpty())
                HttpResponseHelper.Redirect(returnUrl, true);

            Navigator.RedirectToCompletePage(Current.SessionIdentifier);
        }

        private void RefreshUserIdentity(DateTimeOffset dateCompleted)
        {
            // Survey.AssignRespondentToGroups can assign the respondent to groups
            // therefore we need to make sure the respondent has the updated permissions

            if (Current.UserIdentifier != Current.Respondent?.UserIdentifier
                || !MembershipSearch.Exists(x => x.UserIdentifier == Current.UserIdentifier && x.Assigned >= dateCompleted)
                )
            {
                return;
            }

            CurrentSessionModule.Initialize(Context, CurrentSessionState.Identity, true);
        }
    }
}