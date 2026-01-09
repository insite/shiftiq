using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Contacts.Read;
using InSite.Application.Records.Read;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The Competency Management and Development System (CMDS) is a toolkit designed specifically for designing, delivering, and administrating 
    /// competency-based training and education plans. It is a purpose-built for technical trades, where occupational profiles are defined in terms 
    /// of customizable competency frameworks, and microcredentials are granted to learners (who may be students, employees, contractors, members, 
    /// etc.) for completed achievements.
    /// </remarks>
    [DisplayName("Variant: CMDS")]
    [RoutePrefix("api/cmds/contacts")]
    public class CmdsContactsController : ApiBaseController
    {
        /// <remarks>
        /// Use this request to query for a listing of the departments in your organization, and the learners assigned to each department.
        /// </remarks>
        [HttpGet]
        [Route("departments")]
        public HttpResponseMessage Departments(string include = null)
        {
            var includeUsers = include == "users";
            var list = GetDepartmentList(includeUsers);
            return JsonSuccess(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Use this request to query for a listing of the users in your organization, and the departments to which each learner is assigned.
        /// </remarks>
        [HttpGet]
        [Route("users")]
        public HttpResponseMessage Users()
        {
            var list = GetUserList();
            return JsonSuccess(list);
        }

        private Models.Cmds.Department[] GetDepartmentList(bool includeUsers)
        {
            var departments = new List<Models.Cmds.Department>();

            if (includeUsers)
            {
                var users = GetUserList();
                foreach (var learner in users)
                {
                    foreach (var department in learner.Departments)
                    {
                        var item = departments.FirstOrDefault(x => x.Identifier == department.Identifier);
                        if (item == null)
                        {
                            item = new Models.Cmds.Department
                            {
                                Identifier = department.Identifier,
                                Name = department.Name,
                                Users = new List<Models.Cmds.DepartmentUser>()
                            };
                            departments.Add(item);
                        }

                        item.Users.Add(new Models.Cmds.DepartmentUser
                        {
                            Identifier = learner.Identifier,
                            FirstName = learner.FirstName,
                            LastName = learner.LastName,
                            Email = learner.Email
                        });
                    }
                }
            }
            else
            {
                departments = GetDepartments();
            }

            return departments.ToArray();
        }

        private Models.Cmds.User[] GetUserList()
        {
            var organization = GetOrganization();

            return PersonCriteria.Bind(
                x => new Models.Cmds.User
                {
                    Identifier = x.UserIdentifier,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    Departments = x.User.Memberships
                        .Where(y => y.Group.GroupType == GroupTypes.Department)
                        .Select(y => new Models.Cmds.UserDepartment
                        {
                            Identifier = y.Group.GroupIdentifier,
                            Name = y.Group.GroupName,
                        }).ToList()
                },
                new PersonFilter
                {
                    OrganizationIdentifier = organization.Identifier,
                    EmailNotEndsWith = "@" + ServiceLocator.AppSettings.Security.Domain,
                    IsApproved = true,
                    IsArchived = false,
                    IsCmds = true
                }
            );
        }

        private List<Models.Cmds.Department> GetDepartments()
        {
            var organization = GetOrganization();

            var filter = new QGroupFilter
            {
                OrganizationIdentifier = organization.Identifier,
                GroupType = GroupTypes.Department
            };

            return ServiceLocator.GroupSearch
                .GetGroups(filter)
                .Select(x => new Models.Cmds.Department
                {
                    Identifier = x.GroupIdentifier,
                    Name = x.GroupName,
                })
                .ToList();
        }
    }

    /// <summary>
    /// This assignment of a specific profile, competency, or achievement to a specific learner is a document. Use the requests in this folder to 
    /// query for things like assigned profiles, self-assessed competencies, validated competencies, granted achievements, and expired achievements.
    /// </summary>
    [DisplayName("Variant: CMDS")]
    [RoutePrefix("api/cmds/documents")]
    public class CmdsDocumentsController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Use this request to query for a listing of the achievements assigned to learners in your organization.
        /// </remarks>
        [HttpGet]
        [Route("achievements")]
        public HttpResponseMessage Achievements(string achievement = null, string learner = null)
        {
            Guid? achievementId = Guid.TryParse(achievement, out Guid a) ? a : (Guid?)null;
            Guid? learnerId = Guid.TryParse(learner, out Guid u) ? u : (Guid?)null;

            var list = GetAchievementList(achievementId, learnerId);
            return JsonSuccess(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Use this request to query for a listing of the competencies assigned to learners in your organization.
        /// </remarks>
        [HttpGet]
        [Route("competencies")]
        public HttpResponseMessage Competencies(string competency = null, string learner = null)
        {
            Guid? competencyId = Guid.TryParse(competency, out Guid c) ? c : (Guid?)null;
            Guid? learnerId = Guid.TryParse(learner, out Guid u) ? u : (Guid?)null;

            var list = GetCompetencyList(competencyId, learnerId);
            return JsonSuccess(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Use this request to query for a listing of the profiles assigned to learners in your organization.
        /// </remarks>
        [HttpGet]
        [Route("profiles")]
        public HttpResponseMessage Profiles(string profile = null, string learner = null)
        {
            Guid? profileId = Guid.TryParse(profile, out Guid p) ? p : (Guid?)null;
            Guid? learnerId = Guid.TryParse(learner, out Guid u) ? u : (Guid?)null;

            var list = GetProfileList(profileId, learnerId);
            return JsonSuccess(list);
        }

        private Models.Cmds.Documents.Achievement[] GetAchievementList(Guid? achievement, Guid? learner)
        {
            var organization = GetOrganization();

            var filter = new VCredentialFilter { OrganizationIdentifier = organization.Identifier };
            if (achievement.HasValue)
                filter.AchievementIdentifier = achievement;
            if (learner.HasValue)
                filter.UserIdentifier = learner;

            var credentials = ServiceLocator.AchievementSearch.GetCredentials(filter);
            return credentials.Select(
                x => new Models.Cmds.Documents.Achievement
                {
                    AchievementIdentifier = x.AchievementIdentifier,
                    Learner = x.UserIdentifier,
                    Status = x.CredentialStatus,
                    Granted = x.CredentialGranted,
                    Score = (int?)(x.CredentialGrantedScore * 100),
                    Expiry = x.CredentialExpirationExpected
                }).ToArray();
        }

        private Models.Cmds.Documents.Competency[] GetCompetencyList(Guid? competency, Guid? learner)
        {
            var organization = GetOrganization();
            var competencies = StandardValidationSearch.Bind(
                x => new Models.Cmds.Documents.Competency
                {
                    CompetencyIdentifier = x.Standard.StandardIdentifier,
                    Learner = x.User.UserIdentifier,
                    SelfAssessed = x.SelfAssessmentDate,
                    SelfAssessedStatus = x.SelfAssessmentStatus,
                    Validated = x.ValidationDate,
                    ValidatedStatus = x.ValidationStatus
                },
                x => x.User.Persons.Any(y => y.OrganizationIdentifier == organization.OrganizationIdentifier && y.IsLearner)
                  && (competency == null || competency == x.StandardIdentifier)
                  && (learner == null || learner == x.UserIdentifier)
                );

            return competencies.ToArray();
        }

        private Models.Cmds.Documents.Profile[] GetProfileList(Guid? profile, Guid? learner)
        {
            var organization = GetOrganization();
            return DepartmentProfileUserSearch.Bind(
                x => new Models.Cmds.Documents.Profile
                {
                    ProfileIdentifier = x.ProfileStandardIdentifier,
                    Learner = x.User.UserIdentifier,
                    IsPrimary = x.IsPrimary,
                    IsRequired = x.IsRequired
                },
                x => x.User.Persons.Select(y => y.OrganizationIdentifier).Contains(organization.OrganizationIdentifier)
                  && (profile == null || profile == x.ProfileStandardIdentifier)
                  && (learner == null || learner == x.UserIdentifier)
                );
        }
    }

    [DisplayName("Variant: CMDS")]
    [RoutePrefix("api/cmds/reports")]
    public class CmdsReportsController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Use this request to query for a listing of the monthly statistics for mandatory critical primary competencies.
        /// </remarks>
        [HttpGet]
        [Route("statistics")]
        public HttpResponseMessage Statistics(int? year = null, int? month = null)
        {
            if (year == null)
                return JsonError(ProblemFactory.BadRequest("Missing parameter value: year"), HttpStatusCode.BadRequest);

            if (month == null)
                return JsonError(ProblemFactory.BadRequest("Missing parameter value: month"), HttpStatusCode.BadRequest);

            var list = GetStatisticList(year.Value, month.Value)
                .Where(x => StringHelper.EqualsAny(x.ItemName,
                    new[] { "Mandatory Critical Primary Competency", "Mandatory Critical Secondary Competency" }))
                .ToList();

            return JsonSuccess(list);
        }

        /// <summary>
        /// Gets a summary of billable user accounts
        /// </summary>
        [HttpGet]
        [Route("billable-user-summaries")]
        public HttpResponseMessage BillableUserSummaries(string classification)
        {
            var organization = GetOrganization();
            var search = new TUserStatusSearch();
            var list = search.GetBillableUserSummaries(organization.Identifier, classification);
            return JsonSuccess(list);
        }

        /// <summary>
        /// Return the array of month-end user/department statistics for mandatory critical primary competencies in 
        /// facility departments. Keyera's IT department needs this for an executive summary dashboard they are 
        /// developing internally.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        private TUserStatus[] GetStatisticList(int year, int month)
        {
            var organization = GetOrganization();

            var search = new TUserStatusSearch();

            var mostRecent = search.GetMostRecentSnapshotDate(organization.Identifier, year, month);

            if (mostRecent == null)
                return new TUserStatus[0];

            var filter = new TUserStatusFilter
            {
                OrganizationIdentifier = organization.OrganizationIdentifier,
                AsAt = mostRecent,
                DepartmentRole = "Department",
                DepartmentLabel = "Facility"
            };

            return search.Select(filter).ToArray();
        }
    }

    /// <summary>
    /// The setup and configuration of a profile, competency, and/or achievement is done in a template. Use the requests in this folder to query your 
    /// library of profiles, competencies, and achievements.
    /// </summary>
    [DisplayName("Variant: CMDS")]
    [RoutePrefix("api/cmds/templates")]
    public class CmdsTemplatesController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Use this request to query for a listing of all the achievements in your CMDS library. Achievements include e-learning modules, 
        /// time-sensitive safety certificates, orientations, codes of practice, safe operating practices, site-specific operating procedures, 
        /// training guides, and others.
        /// </remarks>
        [HttpGet]
        [Route("achievements")]
        public HttpResponseMessage Achievements()
        {
            var list = GetAchievementList();
            return JsonSuccess(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Use this request to query for a listing of all the competencies in your CMDS library, and the profiles that contain each competency.
        /// </remarks>
        [HttpGet]
        [Route("competencies")]
        public HttpResponseMessage Competencies()
        {
            var list = GetCompetencyList();
            return JsonSuccess(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Use this request to query for a listing of all the profiles in your CMDS library, and the competencies contained in each profile.
        /// </remarks>
        [HttpGet]
        [Route("profiles")]
        public HttpResponseMessage Profiles()
        {
            var list = GetProfileList();
            return JsonSuccess(list);
        }

        private Models.Cmds.Templates.Achievement[] GetAchievementList()
        {
            var organization = GetOrganization();
            var filter = new QAchievementFilter(organization.OrganizationIdentifier);
            var list = ServiceLocator.AchievementSearch.GetAchievements(filter)
                .Select(x => new Models.Cmds.Templates.Achievement
                {
                    Identifier = x.AchievementIdentifier,
                    Name = x.AchievementTitle,
                    Label = x.AchievementLabel,
                    Description = x.AchievementDescription,
                    Expiration = new Expiration(x.ExpirationType, x.ExpirationFixedDate, x.ExpirationLifetimeQuantity, x.ExpirationLifetimeUnit)
                });
            return list.ToArray();
        }

        private Models.Cmds.Templates.Profile[] GetProfileList()
        {
            var organization = GetOrganization();
            return StandardSearch
                .Bind(
                    x => new
                    {
                        x.StandardIdentifier,
                        x.Code,
                        Title = x.ContentTitle,
                        Competencies = x.ParentContainments
                            .Where(y => y.Child.StandardType == "Competency" && y.Parent.StandardType == "Profile")
                            .Select(y => new Models.Cmds.Templates.ProfileCompetency
                            {
                                Identifier = y.ChildStandardIdentifier,
                                Code = y.Child.Code,
                                Name = y.Child.ContentTitle
                            })
                    },
                    x => x.StandardType == "Profile" && x.DepartmentProfiles.Any(y => y.Department.OrganizationIdentifier == organization.OrganizationIdentifier),
                    "Code"
                )
                .Select(x => new Models.Cmds.Templates.Profile
                {
                    Identifier = x.StandardIdentifier,
                    Code = x.Code,
                    Name = x.Title,
                    Competencies = x.Competencies.ToArray()
                })
                .ToArray();
        }

        private Models.Cmds.Templates.Competency[] GetCompetencyList()
        {
            var competencies = new List<Models.Cmds.Templates.Competency>();

            var profiles = GetProfileList();
            foreach (var profile in profiles)
            {
                foreach (var competency in profile.Competencies)
                {
                    var item = competencies.FirstOrDefault(x => x.Identifier == competency.Identifier);
                    if (item == null)
                    {
                        item = new Models.Cmds.Templates.Competency
                        {
                            Identifier = competency.Identifier,
                            Code = competency.Code,
                            Name = competency.Name,
                            Profiles = new List<Models.Cmds.Templates.CompetencyProfile>()
                        };
                        competencies.Add(item);
                    }

                    item.Profiles.Add(new Models.Cmds.Templates.CompetencyProfile
                    {
                        Identifier = profile.Identifier,
                        Code = profile.Code,
                        Name = profile.Name
                    });
                }
            }

            return competencies.ToArray();
        }
    }
}

namespace InSite.Api.Models.Cmds
{
    namespace Templates
    {
        public class Achievement
        {
            public Guid Identifier { get; set; }
            public string Name { get; set; }
            public string Label { get; set; }
            public string Description { get; set; }
            public Expiration Expiration { get; set; }
        }

        public class Competency
        {
            public Guid Identifier { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public List<CompetencyProfile> Profiles { get; set; }
        }

        public class CompetencyProfile
        {
            public Guid Identifier { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class Profile
        {
            public Guid Identifier { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public ProfileCompetency[] Competencies { get; set; }
        }

        public class ProfileCompetency
        {
            public Guid Identifier { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }
    }

    namespace Documents
    {
        public class Achievement
        {
            [JsonProperty("Achievement")]
            public Guid AchievementIdentifier { get; set; }
            public Guid Learner { get; set; }
            public string Status { get; set; }
            public int? Score { get; set; }
            public DateTimeOffset? Granted { get; set; }
            public DateTimeOffset? Expiry { get; set; }
        }

        public class Competency
        {
            [JsonProperty("Competency")]
            public Guid CompetencyIdentifier { get; set; }
            public Guid Learner { get; set; }
            public DateTimeOffset? SelfAssessed { get; set; }
            public string SelfAssessedStatus { get; set; }
            public DateTimeOffset? Validated { get; set; }
            public string ValidatedStatus { get; set; }
        }

        public class Profile
        {
            [JsonProperty("Profile")]
            public Guid ProfileIdentifier { get; set; }
            public Guid Learner { get; set; }
            public bool IsPrimary { get; set; }
            public bool IsRequired { get; set; }
        }
    }

    public class Department
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public List<DepartmentUser> Users { get; set; }
    }

    public class DepartmentUser
    {
        public Guid Identifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class User
    {
        public Guid Identifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<UserDepartment> Departments { get; set; }
    }

    public class UserDepartment
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
    }
}