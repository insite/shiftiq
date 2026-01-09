using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Events.Write;
using InSite.Application.Registrations.Write;

using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Timeline")]
    [ApiAuthenticationRequirement(ApiAuthenticationType.Jwt)]
    public class TimelinesController : ApiBaseController
    {
        // Create a new mutual-exclusion lock. The creating thread does not own it.
        private static readonly Mutex mutex = new Mutex();

        [HttpPost]
        [Route("api/timelines/commands-timers/elapse")]
        public HttpResponseMessage Elapse()
        {
            var root = Global.GetRootSentinel();
            var identity = HttpContext.Current.GetIdentity();
            if (identity.User.Identifier != root.Identifier)
                return JsonError("Only the root developer account is permitted to invoke this API method.");

            try
            {
                mutex.WaitOne(); // Wait until it is safe to enter.

                var exceptions = new List<Exception>();

                ElapseTimers(exceptions);

                SendCommands(exceptions);

                return exceptions.Count > 0
                    ? JsonError(exceptions.Select(x => GetDescription(x)))
                    : JsonSuccess("OK");

            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);

                return JsonError(GetDescription(ex));
            }
            finally
            {
                mutex.ReleaseMutex(); // Release the mutual-exclusion lock.
            }

            string GetDescription(Exception ex)
                => ex.GetType().FullName + ": " + ex.Message;
        }

        private void ElapseTimers(List<Exception> exceptions)
        {
            LogInfo($"ElapseTimers started...");

            try
            {
                var eventTimers = ServiceLocator.EventSearch.GetTimersThatShouldBeElapsed();
                LogInfo($"{eventTimers.Count} event timers will be processed");

                foreach (var timer in eventTimers)
                    SendCommand(new ElapseEventTimer(timer.EventIdentifier, timer.TriggerCommand));

                var registrationTimers = ServiceLocator.RegistrationSearch.GetTimersThatShouldBeElapsed();
                LogInfo($"{registrationTimers.Count} registration timers will be processed");

                foreach (var timer in registrationTimers)
                    SendCommand(new ElapseRegistrationTimer(timer.RegistrationIdentifier, timer.TriggerCommand));
            }
            catch (Exception ex)
            {
                HandleException(ex, nameof(ElapseTimers));
                exceptions.Add(ex);
            }

            LogInfo($"ElapseTimers completed.");
        }

        private void SendCommands(List<Exception> exceptions)
        {
            LogInfo("SendCommands started...");

            try
            {
                ServiceLocator.CommandQueue.Ping(commandCount =>
                {
                    LogInfo($"{commandCount} scheduled commands will be processed");
                });
            }
            catch (Exception ex)
            {
                HandleException(ex, nameof(SendCommands));
                exceptions.Add(ex);
            }

            LogInfo("SendCommands completed.");
        }

        private void HandleException(Exception ex, string source)
        {
            if (ex is OperationCanceledException)
                return;

            AppSentry.SentryError(new Exception($"An unexpected error occurred in the API Timelines controller ({source}).", ex));
        }

        private static void LogInfo(string text)
        {
            ServiceLocator.Logger.Information(text);
        }
    }
}