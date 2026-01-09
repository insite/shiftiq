using System;
using System.Collections.Concurrent;
using System.Web;
using System.Web.UI;

using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Instrumentation;

using Shift.Common;

namespace InSite.Admin.Events.Candidates.Forms
{
    public partial class Verify : AdminBasePage
    {
        #region Classes

        private static class VerifyError
        {
            private static readonly ConcurrentDictionary<MultiKey<Guid, Guid>, string> _errors = new ConcurrentDictionary<MultiKey<Guid, Guid>, string>();

            public static void Set(Guid userId, Guid eventId, string message)
            {
                _errors.AddOrUpdate(new MultiKey<Guid, Guid>(userId, eventId), message, (k, v) => message);
            }

            public static string Get(Guid userId, Guid eventId)
            {
                _errors.TryRemove(new MultiKey<Guid, Guid>(userId, eventId), out var message);

                return message;
            }
        }

        private class VerifyTaskData
        {
            public HttpApplicationState Application { get; set; }
            public ProgressPanel.IProgressExternalContext ProgressContext { get; set; }
            public Guid UserId { get; set; }
            public Guid EventId { get; set; }
        }

        #endregion

        private const string SearchUrl = "/ui/admin/events/exams/search";

        private Guid EventIdentifier => Guid.TryParse(Request["event"], out var value) ? value : Guid.Empty;

        private string OutlineUrl => $"/ui/admin/events/exams/outline?event={EventIdentifier}&panel=candidates";

        private string VerifyTaskID => "Events.Candidates.Verify." + EventIdentifier;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            VerifyCompleted.Click += VerifyCompleted_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier);
            if (@event == null || @event.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect(SearchUrl);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsPostBack)
                return;

            LongRunningTask.Create(
                VerifyTaskID,
                CreateVerifyTaskData,
                VerifyCandidates);

            if (!LongRunningTask.TryGetData(VerifyTaskID, out VerifyTaskData data))
                return;

            if (data.UserId == User.UserIdentifier)
            {
                VerifyProgress.SetExternalContext(data.ProgressContext);

                StartPolling();
            }
            else
            {
                ShowMessageAndGoToOutline("The process has already been initiated by another user.");
            }
        }

        private void VerifyCompleted_Click(object sender, EventArgs e)
        {
            var error = VerifyError.Get(User.UserIdentifier, EventIdentifier);
            if (error.IsEmpty())
                HttpResponseHelper.Redirect(OutlineUrl);
            else
                ShowMessageAndGoToOutline(error);
        }

        private VerifyTaskData CreateVerifyTaskData() => new VerifyTaskData
        {
            Application = HttpContext.Current.Application,
            ProgressContext = VerifyProgress.GetExternalContext(),
            UserId = User.UserIdentifier,
            EventId = EventIdentifier,
        };

        private static void VerifyCandidates(VerifyTaskData data)
        {
            var startedOnDate = DateTime.UtcNow;
            var startedOnUnix = Clock.ToUnixMilliseconds(startedOnDate);

            try
            {
                var registrations = ServiceLocator.RegistrationSearch.GetRegistrationsByEvent(data.EventId);

                for (var i = 0; i < registrations.Count; i++)
                {
                    var id = registrations[i].RegistrationIdentifier;
                    ServiceLocator.SendCommand(new ChangeEligibility(id, "Check Eligibility in DA", null));

                    ProgressPanel.UpdateExternalContext(data.Application, data.ProgressContext, context =>
                    {
                        var timeElapsed = DateTime.UtcNow - startedOnDate;

                        var progress = (ProgressIndicator.ContextData)context.Items["Progress"];
                        progress.Total = registrations.Count;
                        progress.Value = i;

                        if (timeElapsed.TotalSeconds > 1)
                        {
                            context.Variables["time_remaining"] = string.Format(
                                "{0:hh}:{0:mm}:{0:ss}s",
                                Clock.TimeRemaining(registrations.Count, i, startedOnDate)
                            );

                            context.Variables["time_estimated"] = string.Format(
                                "{0:hh}:{0:mm}:{0:ss}s",
                                Clock.TimeEstimated(registrations.Count, i, startedOnDate)
                            );
                        }
                        else
                        {
                            context.Variables["time_remaining"] = "N/A";
                            context.Variables["time_estimated"] = "N/A";
                        }

                        context.Variables["started_on"] = startedOnUnix.ToString();
                    });
                }
            }
            catch
            {
                VerifyError.Set(data.UserId, data.EventId, "An error occurred during operation.");

                throw;
            }
            finally
            {
                ProgressPanel.UpdateExternalContext(data.Application, data.ProgressContext, context =>
                {
                    context.IsComplete = true;
                });
            }
        }

        private void StartPolling()
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Verify),
                "start_polling",
                $"$(document).ready(function () {{ document.getElementById('{VerifyProgress.ClientID}').start(); }});",
                true);
        }

        private void ShowMessageAndGoToOutline(string message)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Verify),
                "message",
                $"$(document).ready(function () {{ " +
                $"alert({HttpUtility.JavaScriptStringEncode(message, true)}); " +
                $"window.location.replace({HttpUtility.JavaScriptStringEncode(OutlineUrl, true)}); " +
                $"}});",
                true);
        }
    }
}