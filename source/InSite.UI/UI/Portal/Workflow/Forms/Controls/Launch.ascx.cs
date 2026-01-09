using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using Humanizer;

using InSite.Application.Responses.Write;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Domain.Surveys.Forms;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Portal.Workflow.Forms.Models;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class Launch : SubmissionSessionControl
    {
        static ConcurrentDictionary<Guid, object> _mutexes = new ConcurrentDictionary<Guid, object>();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StartButton.Click += (s, a) => Continue();
            DebugButton.Click += (s, a) => Continue();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!Current.IsValid)
                HttpResponseHelper.Redirect(RelativeUrl.PortalHomeUrl, true);

            SurveyFormTitle.InnerText = Current.Survey.Content?.Title?.Text[Current.Language];

            if (!IsSurveyOpen())
                return;

            // If the form requires identification and the user is not identified, then display an error.
            if (Current.Survey.RequireUserIdentification && (Current.Respondent == null || Current.Respondent.UserIdentifier == Shift.Constant.UserIdentifiers.Someone))
            {
                ErrorAlert.AddMessage(AlertType.Error, $"<strong>{Translate("Access Denied")}:</strong> {Translate("This form requires identification and does not permit anonymous submissions")}.");
                return;
            }

            var isGradebookLocked = CheckGradebookLocked();

            // If the form does require identification and does not limit submissions per respondent then auto-start,
            // regardless of the user's identity, and regardless of the number of existing submissions.

            if (!isGradebookLocked && !Current.Survey.RequireUserIdentification && !Current.Survey.ResponseLimitPerUser.HasValue)
            {
                Continue();
                return;
            }

            // If the form has no starting instructions and no existing submissions, then then auto-start.

            DebugButton.Visible = Current.Debug && AutoStart(Current);

            if (!isGradebookLocked && AutoStart(Current) && !Current.Debug)
            {
                Continue();
                return;
            }

            SessionPanel.Visible = true;

            var instructions = Current.Survey.Content != null
                ? Current.Survey.Content.GetHtml(ContentLabel.StartingInstructions, Current.Language)
                : null;
            SurveyFormInstructions.InnerHtml = instructions;
            SurveyFormInstructions.Visible = instructions.HasValue();

            // Hide the start button if the user has already submitted the maximum number of submissions permitted.

            var limit = Current.Survey.ResponseLimitPerUser ?? 0;
            StartButton.Visible = !isGradebookLocked && (limit == 0 || limit > Current.Sessions.Length);

            // Display existing submissions, if any.

            ResponsePanel.Visible = Current.Sessions.Length > 0;
            SubmissionRepeater.Bind(Current.Respondent.TimeZone, Current.Sessions);

            var responseCountText = Translate($"{Translate("You have")} {"submission".ToQuantity(Current.Sessions.Length)} {Translate("to this form")}.");
            ResponseCount.Text = responseCountText;
        }

        private bool IsSurveyOpen()
        {
            if (Current.Survey.Status == SurveyFormStatus.Opened
                || Current.Respondent == null
                || Current.Survey.Status == SurveyFormStatus.Drafted
                    && CurrentSessionState.Identity?.Person != null
                    && CurrentSessionState.Identity.Person.IsAdministrator
                )
            {
                return true;
            }

            if (Current.Survey.Status == SurveyFormStatus.Closed)
            {
                var instruction = Current.Survey.Content?.GetHtml(ContentLabel.ClosedInstructions);
                if (instruction != null)
                {
                    ErrorAlert.AddMessage(AlertType.Error, $"{Translate(instruction).Trim()}");
                    return false;
                }
            }

            ErrorAlert.AddMessage(AlertType.Error, $"{Translate("This form is currently not accepting submissions.")}");

            return false;
        }

        public static bool AutoStart(SubmissionSessionState current)
        {
            var instructions = current.Survey.Content?.GetHtml(ContentLabel.StartingInstructions);
            if (!instructions.HasValue() && current.Sessions.Length == 0)
                return true;

            if (current.Sessions.Length == 1 && current.Survey.ResponseLimitPerUser == 1)
                if (!current.Sessions[0].ResponseSessionCompleted.HasValue)
                    return true;

            return false;
        }

        private void Continue()
        {
            var action = ResponseVerb.Answer;
            Guid? mostRecent = null;

            try
            {
                var mutex = _mutexes.GetOrAdd(Current.UserIdentifier, key => new object());

                lock (mutex)
                {
                    if (Current.Survey.ResponseLimitPerUser == 1)
                        mostRecent = GetMostRecentSession();

                    if (mostRecent.HasValue)
                        action = ResponseVerb.Resume;
                    else
                        mostRecent = CreateNewSession();
                }
            }
            finally
            {
                _mutexes.TryRemove(Current.UserIdentifier, out _);
            }

            if (action == ResponseVerb.Answer)
                SubmissionSessionNavigator.RedirectToStart(mostRecent.Value);
            else
                SubmissionSessionNavigator.RedirectTo(action, mostRecent.Value);
        }

        private Guid? GetMostRecentSession()
        {
            var filter = new QResponseSessionFilter
            {
                RespondentUserIdentifier = Current.UserIdentifier,
                SurveyFormIdentifier = Current.FormIdentifier
            };
            var sessions = ServiceLocator.SurveySearch.GetResponseSessions(filter);
            if (sessions.Count > 0)
                return sessions.First().ResponseSessionIdentifier;
            return null;
        }

        private Guid CreateNewSession()
        {
            var session = UniqueIdentifier.Create();
            var commands = BuildCommandScript("Launched", session, Current.Survey, Current.UserIdentifier);
            foreach (var command in commands)
                ServiceLocator.SendCommand(command);
            return session;
        }

        public static ICommand[] BuildCommandScript(string source, Guid session, SurveyForm form, Guid user)
        {
            ProgramHelper.EnrollLearnerByObjectId(form.Tenant, user, form.Identifier);

            var script = new List<ICommand>
            {
                new CreateResponseSession(session, source, form.Tenant, form.Identifier, user)
            };

            // Notice we add every question and every option to the submission.

            foreach (var question in form.Questions)
            {
                if (!question.HasInput)
                    continue;

                script.Add(new AddResponseAnswer(session, question.Identifier));

                var options = question.FlattenOptionItems();
                if (options.Length == 0)
                    continue;

                var items = options.Select(x => x.Identifier).ToArray();
                if (question.IsList && question.ListEnableRandomization)
                    items = Randomize(items, question.ListEnableOtherText);
                script.Add(new AddResponseOptions(session, question.Identifier, items));
            }

            return script.ToArray();
        }

        private static Guid[] Randomize(Guid[] input, bool isLastItemFixed)
        {
            var itemCount = input.Length;
            var shuffleCount = isLastItemFixed ? itemCount - 1 : itemCount;

            if (shuffleCount <= 1)
                return input;

            var list = input.ToList();
            list.Shuffle(0, shuffleCount);
            return list.ToArray();
        }

        private bool CheckGradebookLocked()
        {
            if (!LockedGradebookHelper.HasLockedGradebook(Current.Survey.Identifier, Current.Survey.Hook))
                return false;

            ErrorAlert.AddMessage(AlertType.Error, Translate("The gradebook is locked, please contact the administrator for details."));

            return true;
        }
    }

}