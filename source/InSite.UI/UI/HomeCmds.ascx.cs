using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.CMDS.Common.Models;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.UI
{
    public partial class HomeCmds : BaseUserControl
    {
        public Guid CustomCatalogId = Guid.Parse("8765cb16-0f17-4a6c-b821-b2ec002bfe41");

        [Serializable]
        public class CachedAchievements
        {
            public DateTime Created { get; set; }
            public string[] AchievementTypes { get; set; }
        }

        [Serializable]
        public class ProgressItem
        {
            public string Title { get; set; }
            public string ProgressUrl { get; set; }
            public string ProgressText { get; set; }
            public string ChartHtml { get; set; }
        }

        public List<ProgressItem> ProgressItems
        {
            set
            {
                ViewState[nameof(ProgressItems)] = value;
            }
            get
            {
                return ViewState[nameof(ProgressItems)] as List<ProgressItem>;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CmdsCompetencyDashboard.DataLoaded += (s, a) => BindProgress();

            if (Page is AdminBasePage admin)
                if (admin.Navigator != null)
                    admin.Navigator.IsCmds = true;

            LoadPermissionMatrix();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var organization = GetPartitionOrganizationCode();

            ApplyPermissionMatrix(organization);

            ProgressRepeater.DataSource = ProgressItems;
            ProgressRepeater.DataBind();
        }

        public Guid PersonID =>
            Guid.TryParse(Request["id"], out var personID) ? personID : User.UserIdentifier;

        protected void BindModelToControls()
        {
            BindToasts();
            BindShortcutsTab();
            BindProfilesTab();
            BindContactsTab();
            BindNewUsersTab();
            BindImpersonationsTab();
            BindEventsTab();

            var code = Organization.Code;

            CopAnchor.InnerText = AchievementTypes.Pluralize("Code of Practice", code);
            SopAnchor.InnerText = AchievementTypes.Pluralize("Safe Operating Practice", code);
            OppAnchor.InnerText = AchievementTypes.Pluralize("Site-Specific Operating Procedure", code);
            TrdAnchor.InnerText = AchievementTypes.Pluralize("Training Guide", code);

            OrientationAnchor.HRef = RelativeUrl.PortalHomeUrl;
        }

        private void BindToasts()
        {
            var filter = new EmployeeCompetencyFilter
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                UserIdentifier = User.UserIdentifier,
                Statuses = new[] { ValidationStatuses.SelfAssessed }
            };
            var count = UserCompetencyRepository.CountSearchResults(filter, null, null);

            InfoToast.Visible = count > 0;

            if (count > 0)
                InfoToast.Text = $"{User.FirstName}, you have {Shift.Common.Humanizer.ToQuantity(count, "completed self-assessment")} not yet submitted for validation. Remember to submit your competencies for validation.";
        }

        private void BindProgress()
        {
            var complianceSummary = CmdsCompetencyDashboard.ComplianceSummary;
            var learningSummary = CmdsCompetencyDashboard.LearningSummary;
            var orientationSummary = CmdsCompetencyDashboard.OrientationSummary;

            var dataItems = new List<SummaryDataItem>();
            dataItems.AddRange(complianceSummary.DataItems);
            dataItems.AddRange(learningSummary.DataItems);
            dataItems.AddRange(orientationSummary.DataItems);

            ProgressItems = dataItems.Select(x => new ProgressItem
            {
                Title = x.Title,
                ProgressUrl = x.ProgressUrl,
                ProgressText = x.ProgressText,
                ChartHtml = GetChartHtml(x.Total, x.Completed, x.Total == x.Completed),
            })
                .ToList();

            if (ShowSafetyAchievementsOnly())
                ProgressItems = ProgressItems.Where(x => x.Title == "Time-Sensitive Safety Certificates").ToList();
        }

        private string GetChartHtml(int total, int value, bool isSuccess)
        {
            var progressValue = total == 0 ? 0 : Number.CheckRange(Math.Round((decimal)value / total * 100m, 0), 0, 100);
            var color = isSuccess ? "success" : "danger";

            return $"<div class=\"circular-progress\" style=\"--ar-progress-value:{progressValue};--ar-progress-bar-bg-primary: var(--ar-{color});\">"
                 + $"<span>{value:n0}/{total:n0}</span>"
                 + $"</div>";
        }

        private void BindShortcutsTab()
        {
            var catalog = CourseSearch.GetCatalog(Organization.Identifier, CustomCatalogId);

            CustomCatalogLink.Visible = catalog != null && !catalog.IsHidden;

            if (catalog != null)
            {
                CustomCatalogLink.HRef = $"/ui/portal/learning/catalog?catalog={CustomCatalogId}&view=1";
                CustomCatalogLink.InnerHtml = "<i class='fa-solid fa-books me-2'></i>" + catalog.CatalogName;
            }

            var skillsPassportGroups = new string[]
            {
                "Skills Passport Administrators",
                "Skills Passport Developers",
                "Skills Passport Users"
            };

            OrientationAnchor.Visible = Identity.IsInGroup(skillsPassportGroups);
        }

        private void BindProfilesTab()
        {
            var profiles = new List<ProfileRepeaterItem>();

            LoadDepartments();
            LoadPrimaryProfile();
            LoadSecondaryProfiles();
            LoadTertiaryProfiles();

            void LoadDepartments()
            {
                var data = ContactRepository3.SelectPersonDepartments(CurrentIdentityFactory.ActiveOrganizationIdentifier, User.UserIdentifier,
                    new[] { MembershipType.Organization, MembershipType.Department });
                Departments.DataSource = data;
                Departments.DataBind();
                Departments.Visible = data.Rows.Count > 0;
            }

            void LoadPrimaryProfile()
            {
                var info = UserProfileRepository.SelectPrimaryProfile(User.UserIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier);
                var hasPrimary = info != null;
                PrimaryProfile.Visible = PrimaryProfileLink.Visible = hasPrimary;
                NoPrimaryProfile.Visible = !hasPrimary;

                if (hasPrimary)
                {
                    var profile = StandardSearch.Select(info.ProfileStandardIdentifier);
                    PrimaryProfileLink.NavigateUrl = "/ui/cmds/portal/validations/competencies/search?profile=" +
                                                     info.ProfileStandardIdentifier + "&userID=" + info.UserIdentifier +
                                                     "&department=" + info.DepartmentIdentifier;
                    ProfileName.Text = profile.ContentTitle;

                    profiles.Add(new ProfileRepeaterItem
                    {
                        ProfileTitle = ProfileName.Text,
                        ProfileUrl = PrimaryProfileLink.NavigateUrl
                    });
                }
            }

            void LoadSecondaryProfiles()
            {
                var tableForCompliance = UserProfileRepository.SelectSecondaryProfiles(User.UserIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier, true);
                var hasData = tableForCompliance.Rows.Count != 0;
                NoSecondaryProfilesForCompliance.Visible = !hasData;
                SecondaryProfilesForCompliance.Visible = hasData;
                SecondaryProfilesForCompliance.DataSource = tableForCompliance;
                SecondaryProfilesForCompliance.DataBind();

                foreach (System.Data.DataRow row in tableForCompliance.Rows)
                    profiles.Add(new ProfileRepeaterItem
                    {
                        ProfileTitle = (string)row["ProfileName"],
                        ProfileUrl = CreateSecondaryProfileUrl(row)
                    });
            }

            void LoadTertiaryProfiles()
            {
                var tableNotForCompliance = UserProfileRepository.SelectSecondaryProfiles(User.UserIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier, false);
                var hasData = tableNotForCompliance.Rows.Count != 0;
                NoSecondaryProfilesNotForCompliance.Visible = !hasData;
                SecondaryProfilesNotForCompliance.Visible = hasData;
                SecondaryProfilesNotForCompliance.DataSource = tableNotForCompliance;
                SecondaryProfilesNotForCompliance.DataBind();

                foreach (DataRow row in tableNotForCompliance.Rows)
                    profiles.Add(new ProfileRepeaterItem
                    {
                        ProfileTitle = (string)row["ProfileName"],
                        ProfileUrl = CreateSecondaryProfileUrl(row)
                    });
            }

            string CreateSecondaryProfileUrl(DataRow row)
            {
                return $"/ui/cmds/portal/validations/competencies/search?profile={row["ProfileStandardIdentifier"]}&userID={row["UserIdentifier"]}&department={row["DepartmentIdentifier"]}";
            }
        }

        private void BindContactsTab()
        {
            RelatedPersons.LoadData(User.UserIdentifier);
            MyContactsPanel.Visible = RelatedPersons.HasContacts();

            var roles = ContactRepository3.SelectUserRoles(User.UserIdentifier);
            Roles.DataSource = roles;
            Roles.DataBind();

            Authentications.LoadData();
        }

        private void BindNewUsersTab()
        {
            PendingApprovalUsers.LoadData();
        }

        private void BindImpersonationsTab()
        {
            ImpersonationGrid.LoadData();
        }

        private void BindEventsTab()
        {
            var eventCount = UpcomingSessions.BindModelToControls();

            EventCard.Visible = eventCount > 0;
        }

        protected string GetImpersonationName(Guid? user, string name)
        {
            string nameTag = string.Format("<strong>{0}</strong>", name);

            return user.HasValue
                ? string.Format("<a href='/ui/cmds/admin/users/edit?userID={1}'>{0}</a>", nameTag, user)
                : nameTag;
        }

        #region Access Control (improved)

        private static PermissionMatrix PermissionMatrix { get; set; }
        private static readonly object _lock = new object();

        private void LoadPermissionMatrix()
        {
            // In v25.7 this new feature is implemented and tested for E03 only.

            if (!ServiceLocator.Partition.IsE03())
                return;

            if (PermissionMatrix != null)
                return;

            lock (_lock)
            {
                PermissionMatrix = new PermissionMatrix();

                var accessDeniedConfigPath = ServiceLocator.AppSettings.Application.SecurityConfigurationPath;

                if (string.IsNullOrEmpty(accessDeniedConfigPath))
                    return;

                // FIXME: Settings in the PartitionSetting table should be moved to AppSettings, now that we have 
                // partition-specific transformation rules working correctly.

                var partition = ServiceLocator.Partition.Slug;

                var relativePathToConfigFile = Path.Combine(
                    accessDeniedConfigPath,
                    $"access-denied.{partition}.json");

                var physicalPathToConfigFile = Server.MapPath(relativePathToConfigFile);

                if (!File.Exists(physicalPathToConfigFile))
                    throw new FileNotFoundException(physicalPathToConfigFile);

                var json = File.ReadAllText(physicalPathToConfigFile);

                var resourcePermissions = JsonConvert.DeserializeObject<ResourcePermissions[]>(json);

                var organization = GetPartitionOrganizationCode();

                PermissionMatrix.AddPermissions(AccessOperation.Deny, resourcePermissions, organization);

                ApplyPermissionMatrix(organization);
            }
        }

        private void ApplyPermissionMatrix(string organization)
        {
            var isObserver = User.UserIdentifier != PersonID;
            var isDeveloper = Identity.IsInRole(CmdsRole.Programmers);
            var isGrantedValidators = Identity.IsGranted(PermissionNames.Custom_CMDS_Validators);
            var isGrantedWorkers = Identity.IsGranted(PermissionNames.Custom_CMDS_Workers);
            var isGrantedColleges = Identity.IsGranted(PermissionNames.Custom_CMDS_Colleges);
            var isGrantedFields = Identity.IsGranted(PermissionNames.Custom_CMDS_Fields);

            NavUsers.Visible = isDeveloper;
            NavImpersonations.Visible = isDeveloper;

            ValidateCompetenciesLink2.Visible = isGrantedValidators;

            ProfilesAndCertificatesPanel.Visible = isGrantedWorkers;
            CompetenciesCard.Visible = isGrantedWorkers;
            ViewMySelfAssessmentsLink.Visible = isGrantedWorkers;
            TrainingPlanLink2.Visible = isGrantedWorkers;

            SearchCollegeCertificatesLink.Visible = isGrantedColleges;

            SearchUploadsLink.Visible = isGrantedFields;
            ManageUploadsLink.Visible = isGrantedFields;

            if (PermissionMatrix == null)
                return;

            var permissions = PermissionMatrix.GetOrCreatePermissions(organization);

            var roles = Identity.Groups.Select(x => x.Name).ToList();

            var denials = new[]
            {
                new ControlHandle("ui/home#college-certificates", CollegeCertificatesCard),
                new ControlHandle("ui/home#competencies", CompetenciesCard),
                new ControlHandle("ui/home#elearning", ElearningLink),
                new ControlHandle("ui/home#education-and-training", EducationAndTrainingLink),
                new ControlHandle("ui/home#learning-portal", LearningPortalSection),
                new ControlHandle("ui/home#dashboard", NavDashboard),
                new ControlHandle("ui/home#profiles", NavProfiles),
                new ControlHandle("ui/home#progress", ProgressSection),
                new ControlHandle("ui/home#training-plan", TrainingPlanLink1),
                new ControlHandle("ui/home#training-plan", TrainingPlanLink2)
            };

            foreach (var denial in denials)
                if (permissions.IsDenied(denial.Resource, roles))
                    denial.Control.Visible = false;
        }

        private bool ShowSafetyAchievementsOnly()
        {
            if (PermissionMatrix == null)
                return false;

            var organization = GetPartitionOrganizationCode();

            var permissions = PermissionMatrix.GetOrCreatePermissions(organization);

            var roles = Identity.Groups.Select(x => x.Name).ToList();

            var safetyAchievementsOnly = permissions.IsDenied("ui/home#safety-achievements-only", roles);

            return safetyAchievementsOnly;
        }

        private string GetPartitionOrganizationCode()
        {
            // FIXME: TEC-363 is required before we can make CMDS the organization account that represents the
            // partition-wide (global) data set in the system. Until then, e03 and cmds are (confusingly) separate.

            return "cmds";
        }

        private class ControlHandle
        {
            public ControlHandle(string resource, Control control)
            {
                Resource = resource;
                Control = control;
            }

            public string Resource { get; set; }
            public Control Control { get; set; }
        }

        #endregion
    }

    public class ProfileRepeaterItem
    {
        public string ProfileTitle { get; set; }
        public string ProfileUrl { get; set; }
    }
}