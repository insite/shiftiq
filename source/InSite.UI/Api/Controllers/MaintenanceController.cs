using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application;
using InSite.Application.Events.Write;
using InSite.Application.Records.Write;
using InSite.Application.Users.Write;
using InSite.Persistence;
using InSite.Persistence.Integration.DirectAccess;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.Service.Progress.Credentials;
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

                SetServiceIdentity(OrganizationIdentifiers.CMDS, UserIdentifiers.Maintenance, "Maintenance");

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

                SetServiceIdentity(OrganizationIdentifiers.SkilledTradesBC, UserIdentifiers.Maintenance, "Maintenance");

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
                    ServiceLocator.AppSettings.Partition.Domain,
                    ServiceLocator.AppSettings.Environment,
                    ServiceLocator.SendCommand,
                    ServiceLocator.RegistrationSearch,
                    ServiceLocator.AlertMailer
                );

                return JsonSuccess(new { Status = "ok" });
            });
        }

        [HttpPost]
        [Route("api/maintenance/send-achievement-notifications")]
        public HttpResponseMessage AchievementNotifications()
        {
            return ExecuteMaintenanceRoutine(() =>
            {
                var helper = new CredentialNotificationHelper(
                    ServiceLocator.AchievementSearch,
                    ServiceLocator.SendCommand,
                    ServiceLocator.AlertMailer);

                var count = helper.CreateNotifications();

                return JsonSuccess(count);
            });
        }

        [HttpPost]
        [Route("api/maintenance/validate-and-publish-exams")]
        public HttpResponseMessage ValidateAndPublishExams()
        {
            return ExecuteMaintenanceRoutine(() =>
            {
                var partition = ServiceLocator.Partition;
                if (!partition.IsE04())
                    return JsonSuccess($"Skipped. Automated upload of online exam results to DirectAccess run on {partition.Slug.ToUpper()}.");

                SetServiceIdentity(OrganizationIdentifiers.SkilledTradesBC, UserIdentifiers.Maintenance, "Maintenance");

                var classReminder = new ExamAutomation(
                    ServiceLocator.EventSearch,
                    ServiceLocator.RegistrationSearch,
                    ServiceLocator.OrganizationSearch,
                    ServiceLocator.IdentityService,
                    ServiceLocator.SendCommand
                );

                var count = classReminder.ValidateAndPublishGrades();

                return JsonSuccess(count);
            });
        }

        [HttpPost]
        [Route("api/maintenance/expire-credentials")]
        public HttpResponseMessage ExpireCredentials()
        {
            return ExecuteMaintenanceRoutine(() =>
            {
                var partition = ServiceLocator.Partition;

                if (partition.IsE03())
                    return JsonSuccess($"Skipped. Expiration for CMDS credentials done in a separate task.");

                var expirer = new CredentialExpirer(new Commander(), ServiceLocator.AchievementSearch);
                var expiredCredentials = expirer.GetExpiredCredentials();
                var organizationIds = expiredCredentials.Select(x => x.OrganizationIdentifier).Distinct().OrderBy(x => x).ToList();
                var expiredCount = 0;

                foreach (var organizationId in organizationIds)
                {
                    SetServiceIdentity(organizationId, UserIdentifiers.Maintenance, "Maintenance");

                    var organizationCredentials = expiredCredentials
                        .Where(x => x.OrganizationIdentifier == organizationId)
                        .OrderBy(x => x.CredentialIdentifier)
                        .ToList();

                    expiredCount += expirer.Expire(organizationCredentials);
                }

                return JsonSuccess(new { Status = "ok", ExpiredCount = expiredCount });
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

                SetServiceIdentity(identity);

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

        private static void SetServiceIdentity(Principal identity) =>
            SetServiceIdentity(identity.Organization.Identifier, identity.User.Identifier, $"API Developer {identity.User.Name}");

        private static void SetServiceIdentity(Guid organizationId, Guid userId, string userName)
        {
            ServiceLocator.IdentityService.SetCurrentService(new ServiceIdentity
            {
                Organization = organizationId,
                User = userId,
                Name = $"API Developer {userName}"
            });
        }

        private static string GetDescription(Exception ex)
            => ex.GetType().FullName + ": " + ex.Message;
    }
}