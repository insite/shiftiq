using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Records
{
    public class CredentialAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new CredentialState();

        public CredentialState Data => (CredentialState)State;

        public void AssignCredential(Guid organization, Guid achievement, Guid user, DateTimeOffset? assigned)
        {
            if (AllowChanges())
                Apply(new CredentialCreated(organization, achievement, user, assigned));
        }

        public void AssignAndGrantCredential(
            Guid organization,
            Guid achievement,
            Guid user,
            DateTimeOffset granted,
            string description,
            decimal? score,
            Expiration expiration,
            Guid? employerGroup,
            string employerGroupStatus
            )
        {
            if (!AllowChanges())
                return;

            var isNew = Data == null || Data.Status == CredentialStatus.Undefined;

            if (isNew || Data?.Assigned == null)
                Apply(new CredentialCreated(organization, achievement, user, granted));

            if (isNew || Data?.Granted == null || Data.Granted < granted || Data.Status == CredentialStatus.Expired)
            {
                Apply(new CredentialGranted3(granted, description, score));
                if (Data.EmployerGroup != employerGroup || !StringHelper.Equals(Data.EmployerGroupStatus, employerGroupStatus))
                    Apply(new CredentialEmployerChanged(employerGroup, employerGroupStatus));
            }

            if (expiration != null && Data?.Expiration == null
                || expiration == null && Data?.Expiration != null
                || expiration != null && Data?.Expiration != null && !expiration.Equals(Data.Expiration)
                || Data?.Granted != null && Data.Granted < granted
                )
            {
                Apply(new CredentialExpirationChanged(expiration));
            }
        }

        public void DescribeCredential(string description)
        {
            if (AllowChanges())
                Apply(new CredentialDescribed2(description));
        }

        public void TagCredential(string necessity, string priority)
        {
            if (AllowChanges())
                Apply(new CredentialTagged(necessity, priority));
        }

        public void ChangeCredentialAuthority(Guid? authorityIdentifier = null, string authorityName = null, string authorityType = null,
            string location = null, string reference = null,
            decimal? hours = null)
        {
            if (AllowChanges())
                Apply(new CredentialAuthorityChanged(authorityIdentifier, authorityName, authorityType, location, reference, hours));
        }

        public void ChangeCredentialExpiration(Expiration expiration)
        {
            if (AllowChanges())
                Apply(new CredentialExpirationChanged(expiration));
        }

        public void ChangeCredentialEmployer(Guid? employerGroup, string employerGroupStatus)
        {
            if (AllowChanges())
                Apply(new CredentialEmployerChanged(employerGroup, employerGroupStatus));
        }

        public void GrantCredential(
            DateTimeOffset granted,
            string description,
            decimal? score,
            Guid? employerGroup,
            string employerGroupStatus
            )
        {
            if (!AllowChanges())
                return;

            var expiration = Data.Expiration;
            if (expiration != null)
            {
                // If the credential has a fixed expiration date, which occurs before the date granted, then it
                // no longer has an expiration after it is granted.
                if (expiration.Type == ExpirationType.Fixed && expiration.Date.HasValue && expiration.Date <= granted)
                {
                    expiration.Type = ExpirationType.None;
                    expiration.Date = null;
                    Apply(new CredentialExpirationChanged(expiration));
                }
            }

            Apply(new CredentialGranted3(granted, description, score));
            if (Data.EmployerGroup != employerGroup || !StringHelper.Equals(Data.EmployerGroupStatus, employerGroupStatus))
                Apply(new CredentialEmployerChanged(employerGroup, employerGroupStatus));
        }

        public void RevokeCredential(DateTimeOffset revoked, string reason, decimal? score)
        {
            if (AllowChanges())
                Apply(new CredentialRevoked2(revoked, reason, score));
        }

        public void RequestExpirationReminder(ReminderType type, DateTimeOffset requested)
        {
            if (AllowChanges())
                Apply(new ExpirationReminderRequested2(type, requested));
        }

        public void DeliverExpirationReminder(ReminderType type, DateTimeOffset? delivered)
        {
            if (AllowChanges())
                Apply(new ExpirationReminderDelivered2(type, delivered));
        }

        public void ExpireCredential(DateTimeOffset expired)
        {
            if (AllowChanges())
                Apply(new CredentialExpired2(expired));
        }

        public void DeleteCredential()
        {
            if (AllowChanges())
                Apply(new CredentialDeleted2());
        }

        private bool AllowChanges()
        {
            return true;
        }
    }
}
