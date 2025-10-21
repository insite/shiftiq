using System;
using System.Web.UI;

using InSite.Application.Attempts.Read;
using InSite.Application.Records.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Home.Controls
{
    public enum SideMenuVariant { Auto = 0, Default = 1, Manager = 2, Learner = 3 }

    public partial class MyDashboards : BaseUserControl
    {
        public SideMenuVariant Variant { get; set; } = SideMenuVariant.Auto;

        public string ActiveLinksClientID
        {
            get
            {
                if (LinksManager.Visible) return LinksManager.ClientID;
                if (LinksLearner.Visible) return LinksLearner.ClientID;
                return LinksDefault.ClientID;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsAuthenticated)
            {
                LinksDefault.Visible = false;
                LinksManager.Visible = false;
                LinksLearner.Visible = false;
                return;
            }

            if (IsPostBack)
                return;

            var active = ResolveVariant();

            LinksDefault.Visible = active == SideMenuVariant.Default;
            LinksManager.Visible = active == SideMenuVariant.Manager;
            LinksLearner.Visible = active == SideMenuVariant.Learner;

            if (active == SideMenuVariant.Default)
            {
                HideMyCatalogs();

                SetMyChatsCount(Organization.Identifier, User.Identifier);
                SetMyCompetenciesCount(Organization.Identifier, User.Identifier);
                SetMyAchievementsCounter(Organization.Identifier, User.Identifier);
                SetMyGradesCounter(Organization.Identifier, User.Identifier);
                SetMyLogbooksCounter(Organization.Identifier, User.Identifier);
                SetMyCoursesCounter(Organization.Identifier, User.Identifier);
                SetMyProgramsCounter(Organization.Identifier, User.Identifier);
                SetMyAssessmentsCounter(Organization.Identifier, User.Identifier);
                SetMyEventsCounter(Organization.Identifier, User.Identifier);
                SetMySurveysCounter(Organization.Identifier, User.Identifier);
                SetMyMessagesCounter(Organization.Identifier, User.Identifier);
                SetMyReportsCounter(Organization.Identifier, User.Identifier);

                if (ServiceLocator.Partition.IsE03())
                    BindE03();
            }
        }

        /// <remarks>
        /// As per request on DEV-10010, the Catalogs item in the dashboard navigation is hidden
        /// until more detailed requirements are defined.
        /// </remarks>
        private void HideMyCatalogs()
        {
            MyCatalogs.Visible = false;
        }

        private PortalMaster GetPortalMaster()
        {
            MasterPage m = Page?.Master;
            while (m != null && !(m is PortalMaster))
                m = m.Master; 
            return m as PortalMaster;
        }

        private SideMenuVariant ResolveVariant()
        {
            if (Variant != SideMenuVariant.Auto)
                return Variant;

            var portalMaster = GetPortalMaster();

            if (portalMaster?.IsManagerGroupMember == true)
                return SideMenuVariant.Manager;

            if (portalMaster?.IsLearnerGroupMember == true)
                return SideMenuVariant.Learner;

            return SideMenuVariant.Default;
        }

        private void BindE03()
        {
            var learner = Request.QueryString["learner"];
            if (learner != null)
            {
                MyCompetencies.HRef = $"/ui/portal/home/competencies?learner={learner}";
                MyAchievements.HRef = $"/ui/portal/home/achievements?learner={learner}";
                MyMessages.HRef = $"/ui/portal/home/messages?learner={learner}";
                MyAccount.Visible = false;
            }

            MyGrades.Visible = false;
            MyLogbooks.Visible = false;
            MyCourses.Visible = false;
            MyPrograms.Visible = false;
            MyAssessments.Visible = false;
            MyEvents.Visible = false;
            MySurveys.Visible = false;
        }

        #region Methods (counters)

        private void SetMyAchievementsCounter(Guid organization, Guid user)
        {
            var filter = new VCredentialFilter
            {
                AchievementHasCertificate = true,
                CredentialStatus = CredentialStatus.Valid.ToString(),
                OrganizationIdentifier = organization,
                UserIdentifier = user
            };
            var count = ServiceLocator.AchievementSearch.CountCredentials(filter);

            MyAchievements.Attributes["data-count"] = count.ToString();

            var fields = Organization.Fields;
            MyAchievements.Visible = fields.IsVisible("achievements", fields.LearnerDashboard);
        }

        private void SetMyAssessmentsCounter(Guid organization, Guid learner)
        {
            var count = ServiceLocator.AttemptSearch.CountAttempts(new QAttemptFilter
            {
                LearnerUserIdentifier = learner,
                FormOrganizationIdentifier = organization,
                OrderBy = nameof(QAttempt.AttemptStarted) + " DESC",
            });

            MyAssessments.Attributes["data-count"] = count.ToString();

            var fields = Organization.Fields;
            MyAssessments.Visible = fields.IsVisible("assessments", fields.LearnerDashboard);
        }

        private void SetMyChatsCount(Guid organization, Guid user)
        {
            var fields = Organization.Fields;
            MyChats.Visible = fields.IsVisible("chats", fields.LearnerDashboard, false);
        }

        private void SetMyCompetenciesCount(Guid organization, Guid user)
        {
            var logbookCount = ServiceLocator.JournalSearch.CountExperienceCompetenciesFrameworks(new QExperienceCompetencyFilter
            {
                UserIdentifier = user,
                OrganizationIdentifier = organization
            });

            MyCompetencies.Attributes["data-count"] = logbookCount.ToString();

            var fields = Organization.Fields;
            MyCompetencies.Visible = fields.IsVisible("competencies", fields.LearnerDashboard);
        }

        private void SetMyCoursesCounter(Guid organization, Guid user)
        {
            var count = CourseSearch.CountMyEnrolledCourses(user, organization);

            MyCourses.Attributes["data-count"] = count.ToString();

            var fields = Organization.Fields;
            MyCourses.Visible = fields.IsVisible("courses", fields.LearnerDashboard);
        }

        private void SetMyProgramsCounter(Guid organization, Guid user)
        {
            var fields = Organization.Fields;
            MyPrograms.Visible = fields.IsVisible("programs", fields.LearnerDashboard);
        }

        private void SetMyEventsCounter(Guid organization, Guid user)
        {
            var filter = new QRegistrationFilter
            {
                OrganizationIdentifier = organization,
                CandidateIdentifier = user
            };
            var count = ServiceLocator.RegistrationSearch.CountRegistrations(filter);

            MyEvents.Attributes["data-count"] = count.ToString();

            var fields = Organization.Fields;
            MyEvents.Visible = fields.IsVisible("events", fields.LearnerDashboard);
        }

        private void SetMyGradesCounter(Guid organization, Guid user)
        {
            var filter = new QGradebookFilter
            {
                OrganizationIdentifier = organization,
                StudentIdentifier = user
            };
            var count = ServiceLocator.RecordSearch.CountGradebooks(filter);

            MyGrades.Attributes["data-count"] = count.ToString();

            var fields = Organization.Fields;
            MyGrades.Visible = fields.IsVisible("grades", fields.LearnerDashboard);
        }

        private void SetMyLogbooksCounter(Guid organization, Guid user)
        {
            var count = ServiceLocator.JournalSearch.CountLearnerJournals(organization, user);

            MyLogbooks.Attributes["data-count"] = count.ToString();

            var fields = Organization.Fields;
            MyLogbooks.Visible = fields.IsVisible("logbooks", fields.LearnerDashboard);
        }

        private void SetMyMessagesCounter(Guid organization, Guid user)
        {
            var count = TEmailSearch.CountMyMessages(user, organization);

            MyMessages.Attributes["data-count"] = count.ToString();

            var fields = Organization.Fields;
            MyMessages.Visible = fields.IsVisible("messages", fields.LearnerDashboard);
        }

        private void SetMySurveysCounter(Guid organization, Guid user)
        {
            var filter = new QResponseSessionFilter
            {
                RespondentUserIdentifier = user,
                OrganizationIdentifier = organization
            };
            var count = ServiceLocator.SurveySearch.CountResponseSessions(filter);

            MySurveys.Attributes["data-count"] = count.ToString();

            var fields = Organization.Fields;
            MySurveys.Visible = fields.IsVisible("surveys", fields.LearnerDashboard);
        }

        private void SetMyReportsCounter(Guid organization, Guid user)
        {
            var fields = Organization.Fields;
            MyReports.Visible = fields.IsVisible("reports", fields.LearnerDashboard, false);
        }

        #endregion

        #region Helper methods

        protected string GetLocalStorageKey()
        {
            var hash = User.Identifier.GetHashCode();
            var bytes = BitConverter.GetBytes(hash);
            var key = Convert.ToBase64String(bytes);

            return key.TrimEnd('=');
        }

        #endregion
    }
}