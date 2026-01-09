using System;
using System.Collections.Generic;
using System.ComponentModel;

using InSite.Application.Responses.Write;
using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using uxButton = InSite.Common.Web.UI.Button;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class SubmitSearchResults : SearchResultsGridViewController<QResponseSessionFilter>
    {
        private bool _allowDelete;
        private ReturnUrl _returnUrl;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
            Grid.RowCreated += Grid_RowCreated;
            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            _allowDelete = CurrentSessionState.Identity
                .IsGranted(PermissionIdentifiers.Admin_Surveys_Responses, PermissionOperation.Delete);
            _returnUrl = new ReturnUrl();
        }

        private void Grid_RowCreated(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var restartButton = (uxButton)e.Row.FindControl("RestartButton");
            restartButton.Click += RestartButton_Click;
        }

        private void Grid_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (VResponse)e.Row.DataItem;

            var startButton = (uxButton)e.Row.FindControl("StartButton");
            if (dataItem.ResponseSessionCompleted.HasValue && dataItem.ResponseIsLocked)
                BindStartButton("Review", "far fa-search", ButtonStyle.Info, SubmissionSessionNavigator.GetReviewPageUrl);
            else if (dataItem.ResponseSessionStarted.HasValue)
                BindStartButton("Resume", "far fa-play", ButtonStyle.Primary, SubmissionSessionNavigator.GetResumePageUrl);
            else
                BindStartButton("Start", "far fa-rocket", ButtonStyle.Success, SubmissionSessionNavigator.GetStartPageUrl);

            var restartButton = (uxButton)e.Row.FindControl("RestartButton");
            restartButton.CommandArgument = dataItem.ResponseSessionIdentifier.ToString();
            restartButton.Visible = _allowDelete;

            var deleteUrl = SubmissionSessionNavigator.GetDeletePageUrl(dataItem.ResponseSessionIdentifier).ToString();

            var deleteButton = (uxButton)e.Row.FindControl("DeleteButton");
            deleteButton.NavigateUrl = _returnUrl.GetRedirectUrl(deleteUrl);
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
            var id = Guid.Parse(((uxButton)sender).CommandArgument);
            var response = ServiceLocator.SurveySearch.GetResponseSession(id);
            ServiceLocator.SendCommand(new DeleteResponseSession(id));

            var survey = ServiceLocator.SurveySearch.GetSurveyState(response.SurveyFormIdentifier).Form;
            var session = UniqueIdentifier.Create();
            var commands = Launch.BuildCommandScript("Restarted", session, survey, CurrentSessionState.Identity.User.UserIdentifier);
            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            SubmissionSessionNavigator.RedirectToStart(session);
        }

        protected override int SelectCount(QResponseSessionFilter filter)
        {
            return ServiceLocator.SurveySearch.CountResponseSessions(filter);
        }

        protected override IListSource SelectData(QResponseSessionFilter filter)
        {
            filter.OrderBy = "ResponseSessionStarted DESC,ResponseSessionCompleted DESC";

            return ServiceLocator.SurveySearch
                .GetResponseSessions(filter)
                .ToSearchResult();
        }

        protected string FormatTime()
        {
            var session = (VResponse)Page.GetDataItem();

            return GetLine("Started", session.ResponseSessionStarted)
                + GetLine("Completed", session.ResponseSessionCompleted);

            string GetLine(string title, DateTimeOffset? date)
            {
                return date.HasValue
                    ? "<div>" + Translate(title) + " " + date.Value.Format(User.TimeZone, true) + "</div>"
                    : string.Empty;
            }
        }

        private Dictionary<Guid, string> _surveyTitles = new Dictionary<Guid, string>();

        protected string GetSurveyTitle()
        {
            var session = (VResponse)Page.GetDataItem();

            if (!_surveyTitles.TryGetValue(session.SurveyFormIdentifier, out var result))
            {
                result = ServiceLocator.ContentSearch
                    .GetTitleText(session.SurveyFormIdentifier, CurrentSessionState.Identity.Language)
                    .IfNullOrEmpty(session.SurveyName);

                _surveyTitles.Add(session.SurveyFormIdentifier, result);
            }

            return result;
        }

        protected string GetFirstCommentAnswer()
        {
            var session = (VResponse)Page.GetDataItem();

            return ServiceLocator.SurveySearch.FirstCommentAnswer(session.ResponseSessionIdentifier);
        }
    }
}