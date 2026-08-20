using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Write;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class ContactRepository2
    {
        #region Classes

        public class Employment
        {
            public Guid OrganizationIdentifier { get; set; }
            public string EmploymentType { get; set; }
            public string Company { get; set; }
            public string Department { get; set; }
            public int Profiles { get; set; }
            public List<ValidationStatus> ValidationStatuses { get; set; }
        }

        public class ValidationStatus
        {
            public string Status { get; set; }
            public int Count { get; set; }
        }

        public class CompanyEmployee
        {
            public Guid Identifier { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int Profiles { get; set; }
            public int OrganizationCount { get; set; }
            public string Status { get; set; }
            public DateTimeOffset? LastAuthenticated { get; set; }
            public List<Employment> Employments { get; set; }
            public List<string> Roles { get; set; }
        }

        public class CompetencyStatusPerPerson
        {
            public string DepartmentName { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public string MembershipType { get; set; }
            public Guid UserIdentifier { get; set; }
            public string UserFullName { get; set; }
            public string ValidationStatus { get; set; }
            public int CompetencyCount { get; set; }
        }

        #endregion

        #region Initialization

        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        #endregion

        public static bool IsCmdsUserInCmdsRole(int userId, string roleName)
        {
            const string query = @"
SELECT TOP 1 1 
FROM custom_cmds.UserRole
WHERE (GroupName = @RoleName OR GroupName = @RoleName) AND UserIdentifier = @UserIdentifier
                ";

            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<int?>(query,
                    new SqlParameter("UserIdentifier", userId),
                    new SqlParameter("RoleName", roleName)
                    ).FirstOrDefault() != null;
            }
        }

        public static IEnumerable<CompanyEmployee> SelectCompetencyStatusPerPerson(Guid organizationId, Guid? department, Guid? personKey, Guid? profileStandardIdentifier, bool isPrimary)
        {
            const string query = "EXEC custom_cmds.SelectCompetencyStatusPerUser @OrganizationIdentifier, @DepartmentIdentifier, @PersonKey, @ProfileStandardIdentifier, @IsPrimary";

            using (var db = new InternalDbContext())
            {
                var data = db.Database.SqlQuery<CompetencyStatusPerPerson>(query, new SqlParameter[]
                {
                    new SqlParameter("@OrganizationIdentifier", organizationId),
                    new SqlParameter("@DepartmentIdentifier", department.HasValue ? (object)department : DBNull.Value),
                    new SqlParameter("@PersonKey", personKey.HasValue ? (object)personKey : DBNull.Value),
                    new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier.HasValue ? (object)profileStandardIdentifier : DBNull.Value),
                    new SqlParameter("@IsPrimary", isPrimary)
                }).ToList();

                var dict = new Dictionary<Guid, CompanyEmployee>();

                foreach (var item in data)
                {

                    if (!dict.TryGetValue(item.UserIdentifier, out CompanyEmployee employee))
                    {
                        dict.Add(item.UserIdentifier, employee = new CompanyEmployee
                        {
                            Name = item.UserFullName,
                            Employments = new List<Employment>()
                        });
                    }

                    var employment = employee.Employments.Find(x => x.Department.Equals(item.DepartmentName, StringComparison.OrdinalIgnoreCase));

                    if (employment == null)
                    {
                        employee.Employments.Add(employment = new Employment
                        {
                            EmploymentType = item.MembershipType,
                            Department = item.DepartmentName,
                            ValidationStatuses = new List<ValidationStatus>()
                        });
                    }

                    employment.ValidationStatuses.Add(new ValidationStatus { Status = item.ValidationStatus, Count = item.CompetencyCount });
                }

                return dict.Values;
            }
        }

        private class ActiveUserRow
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public DateTimeOffset? LastAuthenticated { get; set; }
            public int Profiles { get; set; }
            public int OrganizationCount { get; set; }
            public string Status { get; set; }
        }

        private class ActiveUserEmploymentRow
        {
            public Guid UserIdentifier { get; set; }
            public string MembershipType { get; set; }
            public Guid OrganizationIdentifier { get; set; }
            public string Company { get; set; }
            public string Department { get; set; }
            public int Profiles { get; set; }
        }

        private class ActiveUserRoleRow
        {
            public Guid UserIdentifier { get; set; }
            public string Role { get; set; }
        }

        public static IEnumerable<CompanyEmployee> SelectActiveUsers(Guid organization, IEnumerable<string> employmentTypes, bool includeNoMemberships, string excludeGroup, string nameFilter)
        {
            var employmentTypeList = employmentTypes?.ToList() ?? new List<string>();
            if (employmentTypeList.Count == 0 && !includeNoMemberships)
                return new List<CompanyEmployee>();

            var excludeGroupPattern = string.IsNullOrEmpty(excludeGroup)
                ? (object)DBNull.Value
                : EscapeLikePattern(excludeGroup) + "%";

            var trimmedName = nameFilter?.Trim();
            var nameFilterPattern = string.IsNullOrEmpty(trimmedName)
                ? (object)DBNull.Value
                : "%" + EscapeLikePattern(trimmedName) + "%";

            var employmentTypesCsv = string.Join(",", employmentTypeList);

            using (var db = new InternalDbContext())
            {
                // The three procedures read the whole organization. Two minutes matches the other
                // report searches; the default of thirty seconds is not enough for the largest tenants.
                db.Database.CommandTimeout = 2 * 60;

                var sw = Stopwatch.StartNew();
                var userRows = db.Database.SqlQuery<ActiveUserRow>(
                        "EXEC contacts.GetActiveUsers @Organization, @NameFilter, @EmploymentTypes, @IncludeNoMemberships",
                        new SqlParameter("@Organization", organization),
                        VarCharParameter("@NameFilter", 400, nameFilterPattern),
                        VarCharParameter("@EmploymentTypes", -1, employmentTypesCsv),
                        new SqlParameter("@IncludeNoMemberships", includeNoMemberships))
                    .ToList();
                sw.Stop();
                InSite.ServiceLocator.Logger?.Information(
                    "SelectActiveUsers Q1(users) org={Org} rows={Rows} elapsed={Ms}ms",
                    organization, userRows.Count, sw.ElapsedMilliseconds);

                var dict = userRows.ToDictionary(
                    r => r.UserIdentifier,
                    r => new CompanyEmployee
                    {
                        Identifier = r.UserIdentifier,
                        Name = r.FullName,
                        Email = r.Email,
                        LastAuthenticated = r.LastAuthenticated,
                        Profiles = r.Profiles,
                        OrganizationCount = r.OrganizationCount,
                        Status = r.Status,
                        Employments = new List<Employment>(),
                        Roles = new List<string>()
                    });

                if (dict.Count == 0)
                    return new List<CompanyEmployee>();

                sw.Restart();
                var employments = db.Database.SqlQuery<ActiveUserEmploymentRow>(
                    "EXEC contacts.GetActiveUserEmployments @Organization, @ExcludeGroupPattern, @EmploymentTypes",
                    new SqlParameter("@Organization", organization),
                    VarCharParameter("@ExcludeGroupPattern", 400, excludeGroupPattern),
                    VarCharParameter("@EmploymentTypes", -1, employmentTypesCsv)).ToList();
                sw.Stop();
                InSite.ServiceLocator.Logger?.Information(
                    "SelectActiveUsers Q2(employments) org={Org} rows={Rows} elapsed={Ms}ms",
                    organization, employments.Count, sw.ElapsedMilliseconds);

                foreach (var row in employments)
                {
                    if (!dict.TryGetValue(row.UserIdentifier, out var employee))
                        continue;

                    employee.Employments.Add(new Employment
                    {
                        EmploymentType = row.MembershipType,
                        OrganizationIdentifier = row.OrganizationIdentifier,
                        Company = row.Company,
                        Department = row.Department,
                        Profiles = row.Profiles
                    });
                }

                sw.Restart();
                var roles = db.Database.SqlQuery<ActiveUserRoleRow>(
                    "EXEC contacts.GetActiveUserRoles @Organization, @ExcludeGroupPattern",
                    new SqlParameter("@Organization", organization),
                    VarCharParameter("@ExcludeGroupPattern", 400, excludeGroupPattern)).ToList();
                sw.Stop();
                InSite.ServiceLocator.Logger?.Information(
                    "SelectActiveUsers Q3(roles) org={Org} rows={Rows} elapsed={Ms}ms",
                    organization, roles.Count, sw.ElapsedMilliseconds);

                foreach (var row in roles)
                {
                    if (!dict.TryGetValue(row.UserIdentifier, out var employee))
                        continue;

                    employee.Roles.Add(row.Role);
                }

                return dict.Values.ToList();
            }
        }

        /// <summary>
        /// The columns behind these procedures are varchar, so the parameters have to be varchar as
        /// well. A string handed to SqlParameter is inferred as nvarchar, and because nvarchar wins
        /// data type precedence SQL Server then converts the *column* on every comparison, which
        /// disables the indexes on QMembership and QUser. Pass -1 as the size for varchar(max).
        /// </summary>
        private static SqlParameter VarCharParameter(string name, int size, object value)
        {
            return new SqlParameter(name, SqlDbType.VarChar, size) { Value = value ?? DBNull.Value };
        }

        private static string EscapeLikePattern(string input)
        {
            return input
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_")
                .Replace("[", "\\[");
        }

        public static void DeleteDepartmentReferences(Guid department)
        {
            const string query = @"
DELETE FROM achievements.TAchievementDepartment WHERE DepartmentIdentifier = @DepartmentIdentifier;
DELETE FROM contacts.QMembership WHERE GroupIdentifier = @DepartmentIdentifier;
DELETE FROM standards.DepartmentProfileCompetency WHERE DepartmentIdentifier = @DepartmentIdentifier;
DELETE FROM standards.DepartmentProfileUser WHERE DepartmentIdentifier = @DepartmentIdentifier;
";
            var commands = new List<ICommand>();
            using (var db = new InternalDbContext())
            {
                var standardIds = db.TDepartmentStandards
                    .Where(x => x.DepartmentIdentifier == department)
                    .Select(x => x.StandardIdentifier).ToArray();
                foreach (var standardId in standardIds)
                    commands.Add(new RemoveStandardGroup(standardId, department));

                db.Database.ExecuteSqlCommand(query, new SqlParameter("DepartmentIdentifier", department));
            }

            _sendCommands(commands);
        }
    }
}
