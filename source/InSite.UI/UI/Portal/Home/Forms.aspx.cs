using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Responses.Write;
using InSite.Application.Surveys.Read;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Workflow.Forms.Controls;

using Shift.Common;
using Shift.Constant;

using navigator = InSite.UI.Portal.Workflow.Forms.Controls.SubmissionSessionNavigator;
using uxButton = InSite.Common.Web.UI.Button;

namespace InSite.UI.Portal.Home
{
    public partial class Forms : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ResponseRepeater.ItemCreated += ResponseRepeater_ItemCreated;
            ResponseRepeater.ItemDataBound += ResponseRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BindModelToControls(User.Identifier);
        }

        private void BindModelToControls(Guid user)
        {
            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);

            var filter = new QResponseSessionFilter
            {
                RespondentUserIdentifier = user,
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };

            var responses = ServiceLocator.SurveySearch
                .GetResponseSessions(filter)
                .OrderByDescending(x => x.ResponseSessionCreated)
                .ToArray();

            ResponseRepeater.DataSource = responses.Select(x => new ResponseRepeaterItem
            {
                SurveyFormName = x.SurveyName,
                FirstAnswerText = ServiceLocator.SurveySearch.FirstCommentAnswer(x.ResponseSessionIdentifier),
                ResponseSessionIdentifier = x.ResponseSessionIdentifier,
                ResponseSessionStarted = x.ResponseSessionStarted,
                ResponseSessionCompleted = x.ResponseSessionCompleted,
                ResponseSessionStatus = x.ResponseSessionStatus,
                ResponseIsLocked = x.ResponseIsLocked
            });
            ResponseRepeater.DataBind();

            if (responses.Length == 0)
                StatusAlert.AddMessage(AlertType.Information, GetDisplayText("There are no Forms to display related to your learner profile."));
        }

        private void ResponseRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                BindResponseButtons((uxButton)e.Item.FindControl("RestartButton"));
            }
        }

        private void ResponseRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                BindResponseButtons(
                    (ResponseRepeaterItem)e.Item.DataItem,
                    (uxButton)e.Item.FindControl("RestartButton"),
                    (uxButton)e.Item.FindControl("StartButton"),
                    (uxButton)e.Item.FindControl("DeleteButton"));
            }
        }

        private void BindResponseButtons(uxButton restartButton)
        {
            var allowDelete = CurrentSessionState.Identity
                .IsGranted(PermissionIdentifiers.Admin_Surveys_Responses, PermissionOperation.Delete);

            {
                var button = restartButton;
                button.Click += RestartButton_Click;
            }
        }

        private void BindResponseButtons(ResponseRepeaterItem session, uxButton restartButton, uxButton startButton, uxButton voidButton)
        {
            {
                var button = startButton;
                if (session.ResponseSessionCompleted.HasValue && session.ResponseIsLocked)
                {
                    button.Text = Translate("Review");
                    button.Icon = "far fa-search";
                    button.ButtonStyle = ButtonStyle.Info;
                    button.NavigateUrl = navigator.GetReviewPageUrl(session.ResponseSessionIdentifier).ToString();
                }
                else if (session.ResponseSessionStarted.HasValue)
                {
                    button.Text = Translate("Resume");
                    button.Icon = "far fa-play";
                    button.ButtonStyle = ButtonStyle.Primary;
                    button.NavigateUrl = navigator.GetResumePageUrl(session.ResponseSessionIdentifier).ToString();
                }
                else
                {
                    button.Text = Translate("Start");
                    button.Icon = "far fa-rocket";
                    button.ButtonStyle = ButtonStyle.Success;
                    button.NavigateUrl = navigator.GetStartPageUrl(session.ResponseSessionIdentifier).ToString();
                }
            }

            var allowDelete = CurrentSessionState.Identity
                .IsGranted(PermissionIdentifiers.Admin_Surveys_Responses, PermissionOperation.Delete);

            {
                var button = restartButton;
                button.CommandArgument = session.ResponseSessionIdentifier.ToString();
                button.Click += RestartButton_Click;
                button.Visible = allowDelete;
            }

            {
                var button = voidButton;
                button.NavigateUrl = navigator.GetDeletePageUrl(session.ResponseSessionIdentifier).ToString();
                button.Visible = allowDelete || !session.ResponseIsLocked;
            }
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            // Delete the existing submission session
            var id = Guid.Parse(((uxButton)sender).CommandArgument);
            var response = ServiceLocator.SurveySearch.GetResponseSession(id);
            ServiceLocator.SendCommand(new DeleteResponseSession(id));

            // Create a new submission session.
            var survey = ServiceLocator.SurveySearch.GetSurveyState(response.SurveyFormIdentifier).Form;
            var session = UniqueIdentifier.Create();
            var commands = Launch.BuildCommandScript("Restarted", session, survey, CurrentSessionState.Identity.User.UserIdentifier);
            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            // Start the submission.
            navigator.RedirectToStart(session);
        }
    }

    public class ResponseRepeaterItem
    {
        public string SurveyFormName { get; set; }
        public string FirstAnswerText { get; set; }

        public Guid ResponseSessionIdentifier { get; set; }
        public DateTimeOffset? ResponseSessionStarted { get; set; }
        public DateTimeOffset? ResponseSessionCompleted { get; set; }
        public bool ResponseIsLocked { get; set; }
        public string ResponseSessionStatus { get; set; }
    }
}