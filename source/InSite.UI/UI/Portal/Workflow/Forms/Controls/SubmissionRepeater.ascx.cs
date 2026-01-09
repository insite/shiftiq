using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Responses.Write;
using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

using uxButton = InSite.Common.Web.UI.Button;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class SubmissionRepeater : BaseUserControl
    {
        private class DataItem
        {
            public string FirstAnswerText { get; set; }

            public Guid ResponseSessionIdentifier { get; set; }
            public DateTimeOffset? ResponseSessionStarted { get; set; }
            public DateTimeOffset? ResponseSessionCompleted { get; set; }
            public bool ResponseIsLocked { get; set; }
            public string ResponseSessionStatus { get; set; }
        }

        protected SubmitPage Bootstrap5Page => Page as SubmitPage;

        private string _timeZone;
        private bool _allowDelete;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SessionRepeater.ItemCreated += SessionRepeater_ItemCreated;
            SessionRepeater.ItemDataBound += SessionRepeater_ItemDataBound;
        }

        public void Bind(string timeZone, ISurveyResponse[] sessions)
        {
            _timeZone = (timeZone ?? CurrentSessionState.Identity.Organization.TimeZone.ToString());
            _allowDelete = CurrentSessionState.Identity
                .IsGranted(PermissionIdentifiers.Admin_Surveys_Responses, PermissionOperation.Delete);

            SessionRepeater.DataSource = sessions.Select(x => new DataItem
            {
                FirstAnswerText = ServiceLocator.SurveySearch.FirstCommentAnswer(x.ResponseSessionIdentifier),
                ResponseSessionIdentifier = x.ResponseSessionIdentifier,
                ResponseSessionStarted = x.ResponseSessionStarted,
                ResponseSessionCompleted = x.ResponseSessionCompleted,
                ResponseSessionStatus = x.ResponseSessionStatus,
                ResponseIsLocked = x.ResponseIsLocked
            });
            SessionRepeater.DataBind();
        }

        private void SessionRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var restartButton = (uxButton)e.Item.FindControl("RestartButton");
            restartButton.Click += RestartButton_Click;
        }

        private void SessionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (DataItem)e.Item.DataItem;

            var startButton = (uxButton)e.Item.FindControl("StartButton");
            if (dataItem.ResponseSessionCompleted.HasValue && dataItem.ResponseIsLocked)
                BindStartButton("Review", "far fa-search", ButtonStyle.Info, SubmissionSessionNavigator.GetReviewPageUrl);
            else if (dataItem.ResponseSessionStarted.HasValue)
                BindStartButton("Resume", "far fa-play", ButtonStyle.Primary, SubmissionSessionNavigator.GetResumePageUrl);
            else
                BindStartButton("Start", "far fa-rocket", ButtonStyle.Success, SubmissionSessionNavigator.GetStartPageUrl);

            var restartButton = (uxButton)e.Item.FindControl("RestartButton");
            restartButton.CommandArgument = dataItem.ResponseSessionIdentifier.ToString();
            restartButton.Visible = _allowDelete;

            var deleteButton = (uxButton)e.Item.FindControl("DeleteButton");
            deleteButton.NavigateUrl = SubmissionSessionNavigator.GetDeletePageUrl(dataItem.ResponseSessionIdentifier).ToString();
            deleteButton.Visible = _allowDelete || !dataItem.ResponseIsLocked;

            void BindStartButton(string text, string icon, ButtonStyle style, Func<Guid, WebUrl> getUrl)
            {
                startButton.Text = Translate(text);
                startButton.Icon = icon;
                startButton.ButtonStyle = style;
                startButton.NavigateUrl = getUrl(dataItem.ResponseSessionIdentifier).ToString();
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
            SubmissionSessionNavigator.RedirectToStart(session);
        }

        protected string LocalizeTime(object o)
        {
            if (o == null)
                return "-";

            var time = o as DateTimeOffset?;
            if (o == null)
                return "-";

            return TimeZones.Format(time.Value,
                (_timeZone ?? CurrentSessionState.Identity.Organization.TimeZone.ToString()),
                CurrentSessionState.Identity.Language);
        }

        protected new string Translate(string text) => Bootstrap5Page.Translate(text);
    }
}