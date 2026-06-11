using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Write;

using Shift.Constant;

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

        public static IEnumerable<CompanyEmployee> SelectActiveUsers(Guid organization, IEnumerable<string> employmentTypes, string excludeGroup)
        {
            var departmentEmploymentType = MembershipType.Department;
            var companyEmploymentType = MembershipType.Organization;
            var administrationEmploymentType = MembershipType.Administration;

            using (var db = new InternalDbContext())
            {
                var users = db.Persons
                    .Where(x => x.OrganizationIdentifier == organization && x.User.AccessGrantedToCmds && x.User.UtcArchived == null)
                    .Select(x => new CompanyEmployee
                    {
                        Identifier = x.UserIdentifier,
                        Name = x.User.FullName,
                        Email = x.User.Email,
                        Profiles = x.User.DepartmentProfiles.Count(),
                        OrganizationCount = x.User.Memberships.Where(m => m.Group.GroupType == "Department").Select(m => m.Group.OrganizationIdentifier).Distinct().Count(),
                        Status = x.User.Persons.Any(r => r.UserAccessGranted.HasValue) ? "Approved" : "Pending Approval",
                        Employments = x.User.Memberships
                            .Where(
                                y => y.Group.OrganizationIdentifier == organization
                                  && y.Group.GroupType == GroupTypes.Department
                                  && (excludeGroup == null || !y.Group.GroupName.StartsWith(excludeGroup))
                                  && employmentTypes.Contains(y.MembershipType)
                                  && (
                                    y.MembershipType == departmentEmploymentType
                                    || y.MembershipType == companyEmploymentType
                                    || y.MembershipType == administrationEmploymentType))
                            .Select(y => new Employment
                            {
                                EmploymentType = y.MembershipType,
                                OrganizationIdentifier = y.Group.Organization.OrganizationIdentifier,
                                Company = y.Group.Organization.CompanyName,
                                Department = y.Group.GroupName,
                                Profiles = x.User.DepartmentProfiles.Count(z => z.DepartmentIdentifier == y.GroupIdentifier)
                            })
                            .ToList(),
                        Roles = x.User.Memberships
                            .Where(y => y.Group.GroupType == GroupTypes.Role && (excludeGroup == null || !y.Group.GroupName.StartsWith(excludeGroup)))
                            .Select(y => y.Group.GroupName)
                            .ToList(),
                        LastAuthenticated = x.LastAuthenticated
                    })
                    .ToList();

                return users;
            }
        }

        public static void DeleteDepartmentReferences(Guid department)
        {
            const string query = @"
DELETE FROM achievements.TAchievementDepartment WHERE DepartmentIdentifier = @DepartmentIdentifier;
DELETE FROM contacts.Membership WHERE GroupIdentifier = @DepartmentIdentifier;
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