using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class VCmdsCredentialSearch
    {
        #region Classes

        public class PolicySignedGroup
        {
            public string AchievementLabel { get; set; }
            public int Total { get; set; }
            public int Signed { get; set; }
        }

        private class ReadHelper : ReadHelper<VCmdsCredential>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VCmdsCredential>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Database.CommandTimeout = 2 * 60;

                    var query = context.VCmdsCredentials.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Binding

        public static IReadOnlyList<VCmdsCredential> Select(
            Expression<Func<VCmdsCredential, bool>> filter,
            params Expression<Func<VCmdsCredential, object>>[] includes) =>
            ReadHelper.Instance.Select(filter, includes);

        public static VCmdsCredential SelectFirst(
            Expression<Func<VCmdsCredential, bool>> filter,
            params Expression<Func<VCmdsCredential, object>>[] includes) =>
            ReadHelper.Instance.SelectFirst(filter, includes);

        public static T[] Bind<T>(
            Expression<Func<VCmdsCredential, T>> binder,
            Expression<Func<VCmdsCredential, bool>> filter,
            string modelSort = null,
            string entitySort = null) => ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<VCmdsCredential, T>> binder,
            Expression<Func<VCmdsCredential, bool>> filter,
            string modelSort = null,
            string entitySort = null) => ReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<VCmdsCredential, bool>> filter) =>
            ReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<VCmdsCredential, bool>> filter) =>
            ReadHelper.Instance.Exists(filter);

        #endregion

        #region SELECT

        public static bool Exists(Guid achievement)
        {
            using (var db = new InternalDbContext())
                return db.VCmdsCredentials.FirstOrDefault(x => x.AchievementIdentifier == achievement) != null;
        }

        public static VCmdsCredential Select(Guid user, Guid achievement)
        {
            using (var db = new InternalDbContext())
                return db.VCmdsCredentials.FirstOrDefault(x => x.UserIdentifier == user && x.AchievementIdentifier == achievement);
        }

        public static List<VCmdsCredential> SelectCredentials(Guid department, Guid achievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCmdsCredentials.Where(x => x.AchievementIdentifier == achievementIdentifier)
                    .Join(db.Users.Where(x => x.UtcArchived == null && x.Memberships.Any(z => z.GroupIdentifier == department && z.MembershipType == "Department")),
                        a => a.UserIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => a
                    )
                    .OrderBy(x => x.UserFirstName)
                    .ThenBy(x => x.UserLastName)
                    .ToList();
            }
        }

        public static List<VCmdsCredential> SelectEmployeeAchievementsByDepartment(Guid department, String[] achievementCategories, Boolean? isRequired)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.Memberships
                    .Where(x => x.GroupIdentifier == department && x.MembershipType == "Department" && x.User.UtcArchived == null)
                    .Join(db.VCmdsAchievementDepartments,
                        a => a.GroupIdentifier,
                        b => b.DepartmentIdentifier,
                        (a, b) => new { b.AchievementIdentifier, a.UserIdentifier }
                    )
                    .Join(db.VCmdsCredentials,
                        a => new { a.AchievementIdentifier, a.UserIdentifier },
                        b => new { b.AchievementIdentifier, b.UserIdentifier },
                        (a, b) => b
                    );

                if (achievementCategories.IsNotEmpty())
                    query = query.Where(x => achievementCategories.Contains(x.Achievement.AchievementLabel));

                if (isRequired.HasValue)
                    query = query.Where(x => x.CredentialIsMandatory == isRequired);

                return query.ToList();
            }
        }

        public static List<VCmdsCredential> SelectEmployeeAchievementsByCompany(Guid organizationId, String[] achievementCategories, Boolean? isRequired)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.VCmdsAchievementOrganizations
                    .Where(x => x.OrganizationIdentifier == organizationId)
                    .Join(
                        db.VCmdsCredentials.Where(x => x.User.AccessGrantedToCmds),
                        a => a.AchievementIdentifier,
                        b => b.AchievementIdentifier,
                        (a, b) => b
                    )
                    .Where(
                        x => db.Memberships.Any(
                            y => y.UserIdentifier == x.UserIdentifier
                              && y.Group.GroupType == GroupTypes.Department
                              && y.Group.OrganizationIdentifier == organizationId
                              && y.MembershipType == "Department"
                              && y.User.UtcArchived == null
                        )
                    );

                if (achievementCategories.IsNotEmpty())
                    query = query.Where(x => achievementCategories.Contains(x.AchievementLabel));

                if (isRequired.HasValue)
                    query = query.Where(x => x.CredentialIsMandatory == isRequired);

                return query.ToList();
            }
        }

        public static List<VCmdsAchievement> SelectAchievementsByDepartment(Guid[] departments, String[] achievementTypes, Boolean? isRequired)
        {
            if (departments.IsEmpty())
                departments = new Guid[] { Guid.Empty };

            using (var db = new InternalDbContext())
            {
                var credentials = db.Memberships
                    .Where(
                        x => departments.Contains(x.GroupIdentifier)
                          && x.MembershipType == "Department")
                    .Join(
                        db.VCmdsAchievementDepartments,
                        a => a.GroupIdentifier,
                        b => b.DepartmentIdentifier,
                        (a, b) => new { b.AchievementIdentifier, a.UserIdentifier }
                    )
                    .Join(
                        db.VCmdsCredentials,
                        a => new { a.AchievementIdentifier, a.UserIdentifier },
                        b => new { b.AchievementIdentifier, b.UserIdentifier },
                        (a, b) => b
                    );

                if (isRequired.HasValue)
                    credentials = credentials.Where(x => x.CredentialIsMandatory == isRequired);

                var query = db.VCmdsAchievements
                    .Where(x => credentials.Any(y => y.AchievementIdentifier == x.AchievementIdentifier));

                if (achievementTypes.IsNotEmpty())
                    query = query.Where(x => achievementTypes.Contains(x.AchievementLabel));

                return query
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<VCmdsAchievement> SelectAchievementsByCompany(Guid organizationId, String[] achievementCategories, Boolean? isRequired)
        {
            using (var db = new InternalDbContext())
            {
                var credentials = db.Memberships
                    .Where(
                        x => x.Group.GroupType == GroupTypes.Department
                          && x.Group.OrganizationIdentifier == organizationId)
                    .Join(
                        db.VCmdsCredentials,
                        a => a.UserIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => b
                    );

                if (isRequired.HasValue)
                    credentials = credentials.Where(x => x.CredentialIsMandatory == isRequired);

                var query = db.VCmdsAchievementOrganizations.Where(x => x.OrganizationIdentifier == organizationId)
                    .Select(x => x.Achievement)
                    .Where(x => credentials.Any(y => y.AchievementIdentifier == x.AchievementIdentifier));

                if (achievementCategories.IsNotEmpty())
                    query = query.Where(x => achievementCategories.Contains(x.AchievementLabel));

                return query
                    .OrderBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static VCmdsCredentialAndExperience SelectForTrainingPlan(Guid credential, Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCmdsCredentialAndExperiences
                    .Where(x =>
                        x.CredentialIdentifier == credential
                        && (
                            x.AchievementOrganizationIdentifier == organization
                            || x.AchievementOrganizationIdentifier == OrganizationIdentifiers.CMDS
                        )
                        && x.IsInTrainingPlan == true
                        && x.ExperienceIdentifier == Guid.Empty
                    )
                    .FirstOrDefault();
            }
        }

        public static List<VCmdsCredentialAndExperience> SelectForTrainingPlan(Guid employeeId, Guid organizationIdentifier, string achievementType)
        {
            return SelectForTrainingPlan(employeeId, organizationIdentifier, achievementType, null);
        }

        private static List<VCmdsCredentialAndExperience> SelectForTrainingPlan(Guid employeeId, Guid organizationIdentifier, string achievementType, Guid? achievement)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.VCmdsCredentialAndExperiences
                    .Where(x =>
                        x.UserIdentifier == employeeId
                        && (
                            x.AchievementOrganizationIdentifier == organizationIdentifier
                         || x.AchievementOrganizationIdentifier == OrganizationIdentifiers.CMDS
                        )
                        && x.IsInTrainingPlan == true
                        && x.ExperienceIdentifier == Guid.Empty
                        && x.AchievementDescription != "Hidden"
                    );

                if (achievement.HasValue)
                    query = query.Where(x => x.AchievementIdentifier == achievement);

                if (!string.IsNullOrEmpty(achievementType))
                    query = query.Where(x => x.AchievementLabel == achievementType);

                return query
                    .OrderBy(x => x.AchievementLabel == "Other Achievement" ? 1 : 0)
                    .ThenBy(x => x.AchievementLabel)
                    .ThenBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        public static List<PolicySignedGroup> SelectPolicySignedGroups(Guid user, Guid organization, bool onlyPlanned, bool onlyMandatory, string[] categories)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.VCmdsCredentials
                    .Where(x => x.UserIdentifier == user
                        && (
                                x.OrganizationIdentifier == organization
                             || x.OrganizationIdentifier == OrganizationIdentifiers.CMDS
                            )
                        && categories.Contains(x.AchievementLabel));

                if (onlyPlanned)
                    query = query.Where(x => x.IsInTrainingPlan);

                if (onlyMandatory)
                    query = query.Where(x => x.CredentialIsMandatory);

                return query
                    .GroupBy(x => x.AchievementLabel)
                    .Select(x => new PolicySignedGroup
                    {
                        AchievementLabel = x.Key,
                        Total = x.Count(),
                        Signed = x.Count(y => y.CredentialStatus == "Valid")
                    })
                    .OrderBy(x => x.AchievementLabel)
                    .ToList();
            }
        }

        public static void CountOrientations(Guid user, Guid organization, bool mandatory, out int valid, out int total)
        {
            using (var db = new InternalDbContext())
            {
                valid = db
                    .VCmdsCredentials
                    .Count(x => x.UserIdentifier == user && x.OrganizationIdentifier == organization
                        && x.AchievementLabel == "Orientation"
                        && x.CredentialIsMandatory == mandatory
                        && x.CredentialStatus == "Valid");

                total = db
                    .VCmdsCredentials
                    .Count(x => x.UserIdentifier == user && x.OrganizationIdentifier == organization
                        && x.AchievementLabel == "Orientation"
                        && x.CredentialIsMandatory == mandatory);
            }
        }

        #endregion

        #region COUNT

        public static void CountCompletedByScore(Guid user, Guid organizationId, String resourceType, out Int32 countCompleted, out Int32 countAll)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.VCmdsCredentials
                    .Where(x =>
                        x.UserIdentifier == user
                        && x.IsInTrainingPlan == true
                        && (x.OrganizationIdentifier == organizationId || x.OrganizationIdentifier == OrganizationIdentifiers.CMDS)
                        && x.AchievementLabel == resourceType
                    );

                countCompleted = query.Where(x => x.CredentialStatus == "Valid").Count();
                countAll = query.Count();
            }
        }

        public static void CountCompletedByStatus(Guid user, Guid organization, string resourceType, InclusionType required, out int countCompleted, out int countAll)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.VCmdsCredentials
                    .Where(x =>
                        x.UserIdentifier == user
                        && (x.OrganizationIdentifier == organization || x.OrganizationIdentifier == OrganizationIdentifiers.CMDS)
                        && x.IsInTrainingPlan == true
                        && x.AchievementLabel == resourceType
                    );

                if (required == InclusionType.Only)
                {
                    var isMandatory = required == InclusionType.Only;
                    query = query.Where(x => x.CredentialIsMandatory == isMandatory);
                }

                countCompleted = query.Where(x => x.CredentialStatus == "Valid").Count();
                countAll = query.Count();
            }
        }

        #endregion

        #region Filtering

        public static List<VCmdsCredentialAndExperience> SelectSearchResults(VCmdsCredentialFilter filter, Guid organizationIdentifier, bool includeGlobal)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, organizationIdentifier, includeGlobal, db)
                    .OrderBy(x => x.AchievementLabel)
                    .ThenBy(x => x.AchievementTitle)
                    .ToList();
            }
        }

        private static IQueryable<VCmdsCredentialAndExperience> CreateQuery(VCmdsCredentialFilter filter, Guid organizationIdentifier, bool includeGlobal, InternalDbContext db)
        {
            var query = includeGlobal
                ? db.VCmdsCredentialAndExperiences.Where(x => x.AchievementVisibility == null || x.AchievementVisibility == AccountScopes.Enterprise || x.AchievementOrganizationIdentifier == organizationIdentifier)
                : db.VCmdsCredentialAndExperiences.Where(x => x.AchievementOrganizationIdentifier == organizationIdentifier);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier);

            if (filter.AchievementIdentifier.HasValue)
                query = query.Where(x => x.AchievementIdentifier == filter.AchievementIdentifier);

            if (!string.IsNullOrEmpty(filter.AchievementType))
                query = query.Where(x => x.AchievementLabel == filter.AchievementType);

            if (!string.IsNullOrEmpty(filter.NotAchievementType))
                query = query.Where(x => x.AchievementLabel != filter.NotAchievementType);

            if (!string.IsNullOrEmpty(filter.AchievementTitle))
                query = query.Where(x => x.AchievementTitle.Contains(filter.AchievementTitle));

            if (string.Equals(filter.ProgressionStatus, "Completed", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => x.CredentialStatus == "Valid");
            else if (string.Equals(filter.ProgressionStatus, "Not Completed", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => x.CredentialStatus == "Pending");
            else if (string.Equals(filter.ProgressionStatus, "Expired", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => x.CredentialStatus == "Expired");

            if (filter.CompletionDate.Since.HasValue)
                query = query.Where(x => x.CredentialGranted >= filter.CompletionDate.Since.Value);

            if (filter.CompletionDate.Before.HasValue)
                query = query.Where(x => x.CredentialGranted < filter.CompletionDate.Before.Value);

            if (filter.IsCompetencyTraining.HasValue)
                query = filter.IsCompetencyTraining.Value
                    ? query.Where(x => x.ExperienceIdentifier == Guid.Empty)
                    : query.Where(x => x.ExperienceIdentifier != Guid.Empty);

            if (filter.IsReportingDisabled.HasValue)
                query = query.Where(x => x.AchievementReportingDisabled == filter.IsReportingDisabled.Value);

            if (filter.ExpirationDate.Since.HasValue)
                query = query.Where(x => x.CredentialExpirationExpected >= filter.ExpirationDate.Since.Value);

            if (filter.ExpirationDate.Before.HasValue)
                query = query.Where(x => x.CredentialExpirationExpected < filter.ExpirationDate.Before.Value);

            return query;
        }

        #endregion
    }
}
