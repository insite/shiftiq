using System;

using Shift.Common.Timeline.Commands;

using InSite.Application.Credentials.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    public class RunDailyJobs
    {
        public void Execute(Action<ICommand> sendCommand, IAchievementSearch achievementSearch)
        {
            ExpireCredentials(sendCommand, achievementSearch);

            RequestExpirationReminders(sendCommand, achievementSearch);

            EmployeeCompetencyChangeHelper.CreateChanges(OrganizationExpirationType.Interval, sendCommand);

            EmployeeCompetencyChangeHelper.CreateChanges(OrganizationExpirationType.Date, sendCommand);
        }

        private void ExpireCredentials(Action<ICommand> sendCommand, IAchievementSearch achievementSearch)
        {
            // Find all valid achievement credentials where the actual status is not Expired, but where the expected 
            // status is Expired. Send a command to expire all such credentials. This is done once per day so the
            // system does not send multiple expiration email notices to the same person on the same day.

            var filter = new VCredentialFilter()
            {
                CredentialStatus = CredentialStatus.Valid.ToString(),
                IsPendingExpiry = true
            };

            var list = achievementSearch.GetCredentials(filter);
            foreach (var item in list)
            {
                var credential = achievementSearch.GetCredential(item.CredentialIdentifier);

                var expiration = new Expiration(credential.CredentialExpirationType, credential.CredentialExpirationFixedDate, credential.CredentialExpirationLifetimeQuantity, credential.CredentialExpirationLifetimeUnit);

                var actual = credential.CredentialStatus;

                var expected = CredentialState.ExpectedStatus(credential.CredentialGranted, credential.CredentialRevoked, expiration, DateTimeOffset.UtcNow);

                if (actual == CredentialStatus.Valid.ToString() && expected == CredentialStatus.Expired)
                {
                    var expectedExpiry = CredentialState.CalculateExpectedExpiry(expiration, credential.CredentialGranted);
                    if (expected == CredentialStatus.Expired && expectedExpiry.HasValue)
                        sendCommand(new ExpireCredential(credential.CredentialIdentifier, expectedExpiry.Value));
                }
            }
        }

        private void RequestExpirationReminders(Action<ICommand> sendCommand, IAchievementSearch achievementSearch)
        {
            var now = DateTimeOffset.UtcNow;
            var oneWeekAgo = now.AddDays(-7);
            var twoWeeksFromNow = now.AddDays(14);
            var oneMonthFromNow = now.AddMonths(1);
            var twoMonthsFromNow = now.AddMonths(2);
            var threeMonthsFromNow = now.AddMonths(3);

            var filter = new VCredentialFilter()
            {
                CredentialStatus = CredentialStatus.Expired.ToString(),
                IsPendingReminderRequest = true
            };
            var list = achievementSearch.GetCredentials(filter);

            foreach (var item in list)
            {
                var alert0 = item.CredentialExpirationReminderRequested0;

                var actual = item.CredentialExpired;
                var expected = item.CredentialExpirationExpected;

                if (actual <= now)
                {
                    if (!alert0.HasValue)
                        sendCommand(new RequestExpirationReminder(item.CredentialIdentifier, ReminderType.Today, now));
                }
            }

            filter = new VCredentialFilter()
            {
                CredentialStatus = CredentialStatus.Valid.ToString(),
                IsPendingReminderRequest = true,
                CredentialExpirationExpectedBefore = threeMonthsFromNow
            };
            list = achievementSearch.GetCredentials(filter);

            foreach (var item in list)
            {
                var alert1 = item.CredentialExpirationReminderRequested1;
                var alert2 = item.CredentialExpirationReminderRequested2;
                var alert3 = item.CredentialExpirationReminderRequested3;

                var actual = item.CredentialExpired;
                var expected = item.CredentialExpirationExpected;

                if (expected > twoWeeksFromNow)
                {
                    if (expected <= oneMonthFromNow && !alert1.HasValue)
                        sendCommand(new RequestExpirationReminder(item.CredentialIdentifier, ReminderType.InOneMonth, now));
                    else if (expected <= twoMonthsFromNow && !alert2.HasValue && !alert1.HasValue)
                        sendCommand(new RequestExpirationReminder(item.CredentialIdentifier, ReminderType.InTwoMonths, now));
                    else if (expected <= threeMonthsFromNow && !alert3.HasValue && !alert2.HasValue && !alert1.HasValue)
                        sendCommand(new RequestExpirationReminder(item.CredentialIdentifier, ReminderType.InThreeMonths, now));
                }
            }
        }
    }
}