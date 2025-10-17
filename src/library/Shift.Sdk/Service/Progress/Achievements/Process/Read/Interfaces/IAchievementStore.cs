using System;

using InSite.Domain.Records;

using Shift.Constant;

namespace InSite.Application.Records.Read
{
    public interface IAchievementStore
    {
        void InsertAchievement(AchievementCreated e);
        void UpdateAchievement(AchievementDescribed e);
        void UpdateAchievement(AchievementLocked e);
        void UpdateAchievement(AchievementUnlocked e);
        void UpdateAchievement(AchievementExpiryChanged e);
        void UpdateAchievement(AchievementTenantChanged e);
        void UpdateAchievement(AchievementTypeChanged e);
        void DeleteAchievement(AchievementDeleted e);
        void UpdateAchievement(CertificateLayoutChanged e);
        void UpdateAchievement(AchievementPrerequisiteAdded e);
        void UpdateAchievement(AchievementPrerequisiteDeleted e);
        void UpdateAchievement(AchievementBadgeImageChanged e);
        void UpdateAchievement(AchievementBadgeImageDisabled e);
        void UpdateAchievement(AchievementBadgeImageEnabled e);

        void UpdateAchievement(AchievementReportingDisabled e);
        void UpdateAchievement(AchievementReportingEnabled e);

        void InsertCredential(CredentialCreated e, CredentialStatus status);
        void UpdateCredential(CredentialAuthorityChanged e);
        void UpdateCredential(CredentialDescribed2 e);
        void UpdateCredential(CredentialExpirationChanged e);
        void UpdateCredential(CredentialExpired2 e, CredentialStatus status);
        void UpdateCredential(CredentialGranted3 e, CredentialStatus status);
        void UpdateCredential(CredentialRevoked2 e, CredentialStatus status);
        void UpdateCredential(CredentialEmployerChanged e);
        void UpdateCredential(CredentialTagged e);
        void DeleteCredential(CredentialDeleted2 e);
        void DeleteCredential(Guid credential);
        void UpdateCredential(ExpirationReminderRequested2 e);
        void UpdateCredential(ExpirationReminderDelivered2 e);

        void DeleteAll();
        void Delete(Guid aggregate);
    }
}