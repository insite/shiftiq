using System;
using System.Collections.Generic;

using InSite.Application.Achievements.Write;
using InSite.Application.Credentials.Write;
using InSite.Cmds.Infrastructure;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.Common.Web.Cmds
{
    public static class CompanyAchievementHelper
    {
        #region Public methods

        public static void Delete(Guid uploadId, Guid organizationId, UserModel user)
        {
            var removeFiles = new List<Upload>();

            var relatedAchievements = UploadRepository.SelectAchievements(uploadId);

            UploadRepository.DeleteRelations(uploadId);

            foreach (var achievementId in relatedAchievements)
            {
                if (!AchievementHasReferences(organizationId, achievementId, user.FullName))
                    DeleteAchievement(achievementId, removeFiles);
            }

            using (var fileStorage = CmdsUploadProvider.Current.CreateFileStorage())
            {
                foreach (var uploadInfo in removeFiles)
                    CmdsUploadProvider.Delete(uploadInfo.ContainerIdentifier, uploadInfo.Name, fileStorage);

                CmdsUploadProvider.Delete(uploadId, fileStorage);

                fileStorage.Commit();
            }
        }

        #endregion

        #region Helper methods

        private static bool AchievementHasReferences(Guid organizationId, Guid achievement, string userEmail)
        {
            // Achievement
            if (VCmdsAchievementSearch.Select(achievement).OrganizationIdentifier != organizationId)
                return true;

            // Employee Achievements
            if (VCmdsCredentialSearch.Exists(achievement))
                return true;

            // Upload Relationships
            if (UploadRepository.IsAchievementHasRealtionships(achievement))
                return true;

            // Achievement Folders
            if (UploadRepository.IsAchievementHasUploads(achievement))
                return true;

            return false;
        }

        private static void DeleteAchievement(Guid achievementIdentifier, ICollection<Upload> removeFiles)
        {
            var achievement = VCmdsAchievementSearch.Select(achievementIdentifier);
            var credentials = VCmdsCredentialSearch.Select(x => x.AchievementIdentifier == achievementIdentifier);
            var departments = TAchievementDepartmentSearch.Select(x => x.AchievementIdentifier == achievementIdentifier);
            var categories = TAchievementClassificationSearch.Select(x => x.AchievementIdentifier == achievementIdentifier);

            TAchievementDepartmentStore.Delete(departments);
            TAchievementClassificationStore.Delete(categories);

            foreach (var credential in credentials)
                ServiceLocator.SendCommand(new DeleteCredential(credential.CredentialIdentifier));

            ServiceLocator.SendCommand(new DeleteAchievement(achievementIdentifier, true));

            foreach (var uploadInfo in UploadRepository.SelectAchievementUploads(achievementIdentifier))
            {
                UploadRepository.DeleteRelations(uploadInfo.UploadIdentifier);

                if (uploadInfo.UploadType == UploadType.CmdsFile)
                    removeFiles.Add(uploadInfo);
                else
                    UploadStore.DeleteLink(uploadInfo.UploadIdentifier);
            }
        }

        #endregion
    }
}