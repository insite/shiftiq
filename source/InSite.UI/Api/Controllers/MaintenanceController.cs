using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application;
using InSite.Application.Events.Write;
using InSite.Application.Users.Write;
using InSite.Persistence;
using InSite.Persistence.Integration.DirectAccess;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Api
{
    [DisplayName("Maintenance")]
    [ApiAuthenticationRequirement(ApiAuthenticationType.Jwt)]
    public class MaintenanceController : ApiBaseController
    {
        private static readonly Mutex _mutex = new Mutex();

        [HttpPost]
        [Route("api/maintenance/expire-default-passwords")]
        public HttpResponseMessage ExpireDefaultPasswords()
        {
            return ExecuteMaintenanceRoutine(() =>
            {
                var users = UserSearch.Bind(u => u.UserIdentifier, new UserFilter
                {
                    DefaultPasswordExpired = new DateTimeOffsetRange(null, DateTimeOffset.Now)
                });

                foreach (var user in users)
                    ServiceLocator.SendCommand(new ModifyUserDefaultPassword(user, null, null));

                return JsonSuccess("OK");
            });
        }

        [HttpPost]
        [Route("api/maintenance/send-class-notifications")]
        public HttpResponseMessage ClassMessages()
        {
            return ExecuteMaintenanceRoutine(() =>
            {
                var classReminder = new ClassReminder(
                    ServiceLocator.EventSearch,
                    ServiceLocator.RegistrationSearch,
                    ServiceLocator.RecordSearch,
                    ServiceLocator.GroupSearch,
                    new Commander(),
                    ServiceLocator.AlertMailer,
                    (Guid organizationId) => OrganizationSearch.Select(organizationId),
                    ServiceLocator.AppSettings
                );

                var count = classReminder.CreateNotifications(null, false);

                return JsonSuccess(count);
            });
        }

        [HttpPost]
        [Route("api/maintenance/cmds/run-daily-jobs")]
        public HttpResponseMessage RunDailyJobs()
        {
            var response = ExecuteMaintenanceRoutine(() =>
            {
                var partition = ServiceLocator.Partition;

                if (!partition.IsE03())
                    return JsonSuccess($"Skipped. CMDS daily jobs do not run on {partition.Slug.ToUpper()}.");

                var serviceIdentity = new ServiceIdentity
                {
                    Organization = OrganizationIdentifiers.CMDS,
                    User = UserIdentifiers.Maintenance,
                    Name = "Maintenance"
                };

                ServiceLocator.IdentityService.SetCurrentService(serviceIdentity);

                var runner = new RunDailyJobs();

                runner.Execute(ServiceLocator.SendCommand, ServiceLocator.AchievementSearch);

                return JsonSuccess("OK");
            });

            // It might take 5+ minutes to refresh the snapshots, and we don't need to wait for the refresh to complete
            // here, so invoke the Refresh method inside a fire-and-forget thread. Catch and report any unhandled
            // exceptions, in case anything goes haywire.

            Task.Run(() =>
            {
                try
                {
                    TUserStatusStore.Refresh();
                }
                catch (Exception ex)
                {
                    AppSentry.SentryError(ex);
                }
            });

            return response;
        }

        [HttpPost]
        [Route("api/maintenance/stbc/send-email-notifications")]
        public HttpResponseMessage SubmitEmailNotifications(int deliveryLimit)
        {
            return ExecuteMaintenanceRoutine(() =>
            {
                var partition = ServiceLocator.Partition;

                if (!partition.IsE04())
                    return JsonSuccess($"Skipped. STBC email notifications do not run on {partition.Slug.ToUpper()}.");

                var serviceIdentity = new ServiceIdentity
                {
                    Organization = OrganizationIdentifiers.SkilledTradesBC,
                    User = UserIdentifiers.Maintenance,
                    Name = "Maintenance"
                };

                ServiceLocator.IdentityService.SetCurrentService(serviceIdentity);

                var m2 = new DirectAccessMailer(
                    ServiceLocator.Logger,
                    ServiceLocator.SendCommand,
                    ServiceLocator.DirectAccessServer,
                    ServiceLocator.IdentityService);

                var s2 = new EmailScheduler(
                    ServiceLocator.AppSettings.Environment.Name,
                    StringHelper.Split(ServiceLocator.Partition.WhitelistDomains),
                    StringHelper.Split(ServiceLocator.Partition.WhitelistEmails),
                    ServiceLocator.Logger,
                    ServiceLocator.MessageSearch,
                    m2);

                s2.Execute(deliveryLimit);

                return JsonSuccess("OK");
            });
        }

        [HttpPost]
        [Route("api/maintenance/expire-registration-invitations")]
        public HttpResponseMessage ExpireRegistrationInvitations()
        {
            return ExecuteMaintenanceRoutine(() =>
            {
                RegistrationInvitationHelper.DeleteExpiredInvitations(
                    ServiceLocator.AppSettings.Security.Domain,
                    ServiceLocator.AppSettings.Environment,
                    ServiceLocator.SendCommand,
                    ServiceLocator.RegistrationSearch,
                    ServiceLocator.AlertMailer
                );

                return JsonSuccess(new { Status = "ok" });
            });
        }

        private HttpResponseMessage ExecuteMaintenanceRoutine(Func<HttpResponseMessage> routine)
        {
            var root = Global.GetRootSentinel();

            var identity = HttpContext.Current.GetIdentity();

            if (identity.User.Identifier != root.Identifier)
                return JsonError("Only the root account is permitted to invoke this API method.");

            try
            {
                _mutex.WaitOne(); // Wait until it is safe to enter.

                var serviceIdentity = new ServiceIdentity
                {
                    Organization = identity.Organization.Identifier,
                    User = identity.User.Identifier,
                    Name = $"API Developer {identity.User.Name}"
                };

                ServiceLocator.IdentityService.SetCurrentService(serviceIdentity);

                return routine();
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);

                return JsonError(GetDescription(ex));
            }
            finally
            {
                _mutex.ReleaseMutex(); // Release the mutual-exclusion lock.
            }
        }

        private static string GetDescription(Exception ex)
            => ex.GetType().FullName + ": " + ex.Message;
    }
}