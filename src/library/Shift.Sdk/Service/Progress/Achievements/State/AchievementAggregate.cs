using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new Achievement();

        public Achievement Data => (Achievement)State;

        public void CreateAchievement(Guid organization, string label, string title, string description, Expiration expiration, string source)
        {
            Apply(new AchievementCreated(organization, label, title, description, expiration, source));
        }

        public void DescribeAchievement(string label, string title, string description, bool allowSelfDeclared)
        {
            if (AllowChanges())
                Apply(new AchievementDescribed(label, title, description, allowSelfDeclared));
        }

        public void ChangeAchievementExpiry(Expiration expiration)
        {
            if (AllowChanges())
                Apply(new AchievementExpiryChanged(expiration));
        }

        public void ChangeAchievementBadgeImageUrl(string badgeImageUrl)
        {
            if (AllowChanges())
                Apply(new AchievementBadgeImageChanged(badgeImageUrl));
        }

        public void EnableAchievementCustomBadgeImage()
            => Apply(new AchievementBadgeImageEnabled());

        public void DisableAchievementCustomBadgeImage()
            => Apply(new AchievementBadgeImageDisabled());

        public void ChangeAchievementOrganization(Guid organization)
        {
            if (AllowChanges())
                Apply(new AchievementTenantChanged(organization));
        }

        public void ChangeAchievementType(string type)
        {
            if (AllowChanges())
                Apply(new AchievementTypeChanged(type));
        }

        public void ChangeCertificateLayout(string code)
        {
            if (AllowChanges())
                Apply(new CertificateLayoutChanged(code));
        }

        public void LockAchievement()
        {
            Apply(new AchievementLocked());
        }

        public void UnlockAchievement()
        {
            Apply(new AchievementUnlocked());
        }

        public void DeleteAchievement(bool cascade)
        {
            Apply(new AchievementDeleted(cascade));
        }

        private bool AllowChanges(bool throwWhenDisabled = true)
        {
            if (throwWhenDisabled && !Data.Enabled)
                throw new ChangeDisabledAchievementException();
            return Data.Enabled;
        }

        public void AddAchievementPrerequisite(Guid identifier, Guid[] conditions)
        {
            Apply(new AchievementPrerequisiteAdded(identifier, conditions));
        }

        public void DeleteAchievementPrerequisite(Guid identifier)
        {
            Apply(new AchievementPrerequisiteDeleted(identifier));
        }

        public void EnableAchievementReporting()
            => Apply(new AchievementReportingEnabled());

        public void DisableAchievementReporting()
            => Apply(new AchievementReportingDisabled());
    }
}
