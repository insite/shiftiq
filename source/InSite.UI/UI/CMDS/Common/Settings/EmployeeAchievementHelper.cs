using System;

using InSite.Application.Credentials.Write;
using InSite.Cmds.Infrastructure;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Common.Web.Cmds
{
    public static class EmployeeAchievementHelper
    {
        #region Public methods

        public static VCmdsCredential SignOff(Guid user, Guid achievement)
        {
            var credential = VCmdsCredentialSearch.SelectFirst(x => x.UserIdentifier == user && x.AchievementIdentifier == achievement);
            if (credential == null)
            {
                AppSentry.SentryError($"credential not found userId: {user} achievementId: {achievement}");
                return null;
            }

            var person = ServiceLocator.ContactSearch.GetPerson(user, credential.OrganizationIdentifier);

            ServiceLocator.SendCommand(new GrantCredential(
                credential.CredentialIdentifier,
                DateTimeOffset.UtcNow,
                null,
                null,
                person?.EmployerGroupIdentifier,
                person?.EmployerGroupStatus));

            ServiceLocator.SendCommand(new ChangeCredentialAuthority(
                credential.CredentialIdentifier,
                credential.AuthorityIdentifier,
                credential.AuthorityName,
                credential.AuthorityType,
                credential.AuthorityLocation,
                credential.AuthorityReference,
                credential.CredentialHours));

            credential = VCmdsCredentialSearch.SelectFirst(x => x.UserIdentifier == user && x.AchievementIdentifier == achievement);

            return credential;
        }

        public static bool CanSignOff(VCmdsCredential credential, Guid user)
        {
            return CanSignOff(credential.AchievementIdentifier, credential.UserIdentifier, credential.AuthorityType == "Self", credential.CredentialGranted, user);
        }

        public static bool CanSignOff(Guid? achievement, Guid? employee, bool allowSelfGranting, DateTimeOffset? dateCompleted, Guid user)
        {
            var _achievement = achievement.HasValue ? VCmdsAchievementSearch.Select(achievement.Value) : null;

            return dateCompleted == null
                   && employee == user
                   && (_achievement == null
                       || StringHelper.Equals(_achievement.AchievementLabel, AchievementTypes.AdditionalComplianceRequirement)
                       || StringHelper.Equals(_achievement.AchievementLabel, AchievementTypes.CodeOfPractice)
                       || StringHelper.Equals(_achievement.AchievementLabel, AchievementTypes.HumanResourcesDocument)
                       || StringHelper.Equals(_achievement.AchievementLabel, AchievementTypes.SafeOperatingPractice)
                       || StringHelper.Equals(_achievement.AchievementLabel, AchievementTypes.SiteSpecificOperatingProcedure)
                       || StringHelper.Equals(_achievement.AchievementLabel, AchievementTypes.TrainingGuide)
                       || (allowSelfGranting && TypeAllowsSignOff(_achievement.AchievementLabel)));
        }

        public static bool TypeAllowsSignOff(string subtype)
        {
            return StringHelper.EqualsAny(subtype, new[] {
                AchievementTypes.AdditionalComplianceRequirement,
                AchievementTypes.CodeOfPractice,
                AchievementTypes.HumanResourcesDocument,
                AchievementTypes.SafeOperatingPractice,
                AchievementTypes.SiteSpecificOperatingProcedure,
                AchievementTypes.TrainingGuide
            });
        }

        public static bool IsSignedOff(VCmdsCredential credential)
        {
            return IsSignedOff(credential.CredentialGranted);
        }

        public static bool IsSignedOff(DateTimeOffset? dateCompleted)
        {
            return dateCompleted.HasValue;
        }

        public static void DeleteUserAchievement(Guid userIdentifier, Guid achievement)
        {
            var credential = VCmdsCredentialSearch.SelectFirst(x => x.UserIdentifier == userIdentifier && x.AchievementIdentifier == achievement);

            if (credential != null)
            {
                ServiceLocator.SendCommand(new DeleteCredential(credential.CredentialIdentifier));

                DeleteFiles(credential.CredentialIdentifier);
            }
        }

        public static void DeleteContactExperience(Guid experience)
        {
            var entity = VCmdsCredentialAndExperienceSearch.Select(experience);

            DeleteContactExperience(entity);
        }

        public static void DeleteContactExperience(ContactExperience employeeAchievement)
        {
            ContactExperienceStore.Delete(employeeAchievement);

            DeleteFiles(employeeAchievement.ExperienceIdentifier);
        }

        private static void DeleteFiles(Guid containerId)
        {
            if (containerId == Guid.Empty)
                return;

            var files = UploadSearch.Bind(x => x.Name, x => x.ContainerIdentifier == containerId);

            using (var storage = CmdsUploadProvider.Current.CreateFileStorage())
            {
                foreach (var fileName in files)
                    CmdsUploadProvider.Delete(containerId, fileName, storage);

                storage.Commit();
            }
        }

        #endregion
    }
}