using System;
using System.Data;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class VCmdsAchievementHelper
    {
        #region Public methods

        public static bool IsCompleted(VCmdsCredentialFilter2 p)
        {
            if (p.AchievementLabel == AchievementTypes.TimeSensitiveSafetyCertificate || p.AchievementLabel == AchievementTypes.AdditionalComplianceRequirement)
                return p.CredentialStatus == "Valid";

            if (p.AchievementLabel == AchievementTypes.Module)
                return p.CredentialStatus == "Valid" && p.IsSuccess;

            return p.CredentialStatus == "Valid" && p.CredentialGranted.HasValue;
        }

        public static VCmdsAchievementDependencyList BuildReferencesText(Guid achievementIdentifier)
        {
            var list = new VCmdsAchievementDependencyList();
            DataTable groups = VCmdsAchievementSearch.SelectRelatedGroups(achievementIdentifier);
            DataTable credentials = VCmdsAchievementSearch.SelectRelatedCredentials(achievementIdentifier);
            DataTable gradebooks = VCmdsAchievementSearch.SelectRelatedGradebooks(achievementIdentifier);
            DataTable gradeitems = VCmdsAchievementSearch.SelectRelatedGradeItems(achievementIdentifier);

            if (groups.Rows.Count + credentials.Rows.Count + gradebooks.Rows.Count + gradeitems.Rows.Count == 0)
                return list;

            if (groups.Rows.Count > 0)
                AddRelatedGroups(list, groups);

            if (credentials.Rows.Count > 0)
                AddRelatedCredentials(list, credentials);

            if (credentials.Rows.Count > 0)
                AddRelatedGradebooks(list, gradebooks);

            if (credentials.Rows.Count > 0)
                AddRelatedGradeItems(list, gradeitems);

            return list;
        }

        #endregion

        #region Helper methods

        private static void AddRelatedGroups(VCmdsAchievementDependencyList list, DataTable groups)
        {
            foreach (DataRow row in groups.Rows)
            {
                string type = string.Empty;
                string name = string.Empty;

                String companyName = (String)row["CompanyName"];
                String departmentName = row["DepartmentName"] as String;

                if (departmentName != null)
                {
                    type = "Department";
                    name = $"{companyName}: {departmentName}";
                }
                else
                {
                    type = "Company";
                    name = $"{companyName}";
                }

                list.Add(type, name);
            }
        }

        private static void AddRelatedCredentials(VCmdsAchievementDependencyList list, DataTable credentials)
        {
            foreach (DataRow row in credentials.Rows)
                list.Add("Credential", (string)row["FullName"]);
        }

        private static void AddRelatedGradebooks(VCmdsAchievementDependencyList list, DataTable gradebooks)
        {
            foreach (DataRow row in gradebooks.Rows)
                list.Add("Gradebook", (string)row["GradebookTitle"]);
        }

        private static void AddRelatedGradeItems(VCmdsAchievementDependencyList list, DataTable gradeitems)
        {
            foreach (DataRow row in gradeitems.Rows)
                list.Add("Grade Item", (string)row["GradeItemName"]);
        }

        #endregion
    }
}