using System;
using System.Globalization;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Timeline.Changes;
using Shift.Constant;

namespace InSite.Domain.Organizations
{
    /// <summary>
    /// The When methods cannot be added to the OrganizationState class because the ASP.NET session state provider 
    /// cannot serialize delegates over dynamic methods or methods outside the delegate creator's assembly. If you add
    /// these methods to OrganizationState class (which has instances stored in session state) then you'll get a run-
    /// time exception that says "Unable to serialize the session state. In 'StateServer' and 'SQLServer' mode, ASP.NET 
    /// will serialize the session state objects, and as a result non-serializable objects or MarshalByRef objects are 
    /// not permitted." That exception message is NOT helpful because the OrganizationState class is serializable and
    /// it is not a MarshalByRef object.
    /// </summary>
    [Serializable]
    public class OrganizationState : AggregateState
    {
        public string AccountWarning { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDomain { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationSecret { get; set; }
        public string OrganizationType { get; set; }
        public string StandardContentLabels { get; set; }

        public Guid? AdministratorUserIdentifier { get; set; }
        public Guid? AdministratorGroupIdentifier { get; set; }
        public Guid? GlossaryIdentifier { get; set; }
        public Guid? ParentOrganizationIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public DateTimeOffset AccountOpened { get; set; }
        public DateTimeOffset? AccountClosed { get; set; }

        public CultureInfo[] Languages { get; set; }

        [JsonConverter(typeof(JsonTimeZoneConverter))]
        public TimeZoneInfo TimeZone { get; set; }

        public AccountStatus AccountStatus { get; set; }
        public CompanyDescription CompanyDescription { get; set; }
        public PlatformCustomization PlatformCustomization { get; set; }

        public OrganizationFields Fields { get; set; }
        public OrganizationIntegrations Integrations { get; set; }
        public ToolkitSettings Toolkits { get; set; }

        public OrganizationState()
        {
            CompanyDescription = new CompanyDescription();
            Languages = new[] { new CultureInfo("en") };
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            PlatformCustomization = new PlatformCustomization();
            Fields = new OrganizationFields();
            Integrations = new OrganizationIntegrations();
            Toolkits = new ToolkitSettings();
        }

        [JsonIgnore]
        public string Code => OrganizationCode;

        [JsonIgnore]
        public Guid Identifier => OrganizationIdentifier;

        [JsonIgnore]
        public bool IsAssociation => OrganizationType == "Association";

        [JsonIgnore]
        public Guid Key => OrganizationIdentifier;

        [JsonIgnore]
        public string Name => CompanyName;

        [JsonIgnore]
        public string LegalName => CompanyDescription?.LegalName ?? CompanyName;

        public string[] GetStandardContentLabels()
        {
            return StandardContentLabels.IsEmpty()
                ? new string[0]
                : StandardContentLabels.Split(new[] { ',' }).Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
        }

        #region Methods (when)

        public void When(OrganizationAccountSettingsModified e)
        {
            var accounts = Toolkits.Accounts;
            accounts.AutomaticGroupJoin = e.Accounts.AutomaticGroupJoin;
            accounts.DisplayDashboardPrototype = e.Accounts.DisplayDashboardPrototype;

            if (e.Accounts.PersonCodeAutoincrement != null)
            {
                var autoincrementState = accounts.PersonCodeAutoincrement;
                var autoincrementChange = e.Accounts.PersonCodeAutoincrement;

                autoincrementState.Enabled = autoincrementChange.Enabled;
                autoincrementState.StartNumber = autoincrementChange.StartNumber;
                autoincrementState.Format = autoincrementChange.Format.NullIfEmpty();
            }
        }

        public void When(OrganizationAchievementSettingsModified e)
        {
            var achievements = Toolkits.Achievements;
            achievements.DefaultAchievementType = e.Achievements.DefaultAchievementType.NullIfEmpty();
            achievements.DefaultCertificateLayout = e.Achievements.DefaultCertificateLayout.NullIfEmpty();
            achievements.CertificateFileNameTemplate = e.Achievements.CertificateFileNameTemplate.NullIfEmpty();
            achievements.IsChangeNotificationEnabled = e.Achievements.IsChangeNotificationEnabled;
            achievements.HideModulesInLearningSummary = e.Achievements.HideModulesInLearningSummary;
            achievements.ShowAchievementsInComplianceSummary = e.Achievements.ShowAchievementsInComplianceSummary;
        }

        public void When(OrganizationAdministratorModified e)
        {
            AdministratorUserIdentifier = e.UserId;
            AdministratorGroupIdentifier = e.GroupId;
        }

        public void When(OrganizationAnnouncementModified e)
        {
            AccountWarning = e.Announcement.NullIfEmpty();
        }

        public void When(OrganizationAssessmentSettingsModified e)
        {
            var assessments = Toolkits.Assessments;
            assessments.DisableStrictQuestionCompetencySelection = e.Assessments.DisableStrictQuestionCompetencySelection;
            assessments.EnableQuestionSubCompetencySelection = e.Assessments.EnableQuestionSubCompetencySelection;
            assessments.LockPublishedQuestions = e.Assessments.LockPublishedQuestions;
            assessments.AttemptGradingAssessor = e.Assessments.AttemptGradingAssessor;
            assessments.RubricReGradeKeepInitialScores = e.Assessments.RubricReGradeKeepInitialScores;
            assessments.ShowPersonNameToGradingAssessor = e.Assessments.ShowPersonNameToGradingAssessor;
            assessments.RequireAutoStart = e.Assessments.RequireAutoStart;

            if (e.Assessments.PerformanceReport != null)
            {
                var reportState = assessments.PerformanceReport;
                var reportChange = e.Assessments.PerformanceReport;

                reportState.Enabled = reportChange.Enabled;

                if (reportChange.AssessmentTypeWeights != null)
                    reportState.AssessmentTypeWeights = reportChange.AssessmentTypeWeights.Select(x => x.Clone()).ToArray();

                if (reportChange.Reports != null)
                    reportState.Reports = reportChange.Reports.Select(x => x.Clone()).ToArray();
            }
        }

        public void When(OrganizationAutomaticCompetencyExpirationModified e)
        {
            var settings = PlatformCustomization.AutomaticCompetencyExpiration;
            settings.Type = e.Settings.Type;
            settings.Month = e.Settings.Month;
            settings.Day = e.Settings.Day;
        }

        public void When(OrganizationClosed e)
        {
            AccountClosed = e.Closed ?? e.ChangeTime;
            AccountStatus = AccountStatus.Closed;
        }

        public void When(OrganizationContactSettingsModified e)
        {
            var contacts = Toolkits.Contacts;
            contacts.FullNamePolicy = e.Contacts.FullNamePolicy.NullIfEmpty();
            contacts.DefaultMFA = e.Contacts.DefaultMFA;
            contacts.PortalSearchActiveMembershipReasons = e.Contacts.PortalSearchActiveMembershipReasons;
            contacts.ReadOnlyEmploymentDetails = e.Contacts.ReadOnlyEmploymentDetails;
            contacts.DisableLeaderRelationshipCreation = e.Contacts.DisableLeaderRelationshipCreation;
            contacts.EnableOperatorGroup = e.Contacts.EnableOperatorGroup;
            contacts.EnableTraineeDepartment = e.Contacts.EnableTraineeDepartment;
        }

        public void When(OrganizationCreated e)
        {
            OrganizationIdentifier = e.AggregateIdentifier;
            AccountOpened = e.Opened ?? e.ChangeTime;
            AccountStatus = AccountStatus.Opened;
        }

        public void When(OrganizationDeleted e)
        {
            AccountStatus = AccountStatus.Destroyed;
        }

        public void When(OrganizationDescriptionModified e)
        {
            CompanyDescription.CompanySize = e.Description.CompanySize;
            CompanyDescription.LegalName = e.Description.LegalName.NullIfEmpty();
            CompanyDescription.CompanySummary = e.Description.CompanySummary.NullIfEmpty();
        }

        public void When(OrganizationEventSettingsModified e)
        {
            var events = Toolkits.Events;
            events.AllowLoginAnyTime = e.Events.AllowLoginAnyTime;
            events.AllowUserAccountCreationDuringRegistration = e.Events.AllowUserAccountCreationDuringRegistration;
            events.AllowUsersRegisterEmployees = e.Events.AllowUsersRegisterEmployees;
            events.HideReturnToCalendar = e.Events.HideReturnToCalendar;
            events.CompanySelectionAndCreationDisabledDuringRegistration = e.Events.CompanySelectionAndCreationDisabledDuringRegistration;
            events.ShowUnapplicableSeats = e.Events.ShowUnapplicableSeats;
            events.AllowClassRegistrationFields = e.Events.AllowClassRegistrationFields;
            events.RegisterEmployeesSearchRequirement = e.Events.RegisterEmployeesSearchRequirement;
        }

        public void When(OrganizationFieldsModified e)
        {
            if (e.Fields.User != null)
                Fields.User = e.Fields.User.Select(x => x.Clone()).ToList();

            if (e.Fields.ClassRegistration != null)
                Fields.ClassRegistration = e.Fields.ClassRegistration.Select(x => x.Clone()).ToList();

            if (e.Fields.LearnerDashboard != null)
                Fields.LearnerDashboard = e.Fields.LearnerDashboard.Select(x => x.Clone()).ToList();

            if (e.Fields.InvoiceBillingAddress != null)
                Fields.InvoiceBillingAddress = e.Fields.InvoiceBillingAddress.Select(x => x.Clone()).ToList();
        }

        public void When(OrganizationGlossaryModified e)
        {
            GlossaryIdentifier = e.GlossaryId;
        }

        public void When(OrganizationGradebookSettingsModified e)
        {
            var gradebooks = Toolkits.Gradebooks;
            gradebooks.DefaultPassPercent = e.Gradebooks.DefaultPassPercent;
            gradebooks.HideIgnoreScoreCheckbox = e.Gradebooks.HideIgnoreScoreCheckbox;
        }

        public void When(OrganizationIdentificationModified e)
        {
            OrganizationCode = e.Code.NullIfEmpty();
            CompanyName = e.Name.NullIfEmpty();
            CompanyDomain = e.Domain.NullIfEmpty();
        }

        public void When(OrganizationIntegrationSettingsModified e)
        {
            var integrations = Integrations;

            if (e.Integrations.Bambora != null)
                integrations.Bambora = e.Integrations.Bambora.Clone();

            if (e.Integrations.BCMail != null)
                integrations.BCMail = e.Integrations.BCMail.Clone();

            if (e.Integrations.Recaptcha != null)
                integrations.Recaptcha = e.Integrations.Recaptcha.Clone();

            if (e.Integrations.Prometric != null)
                integrations.Prometric = e.Integrations.Prometric.Clone();

            if (e.Integrations.ScormCloud != null)
                integrations.ScormCloud = e.Integrations.ScormCloud.Clone();
        }

        public void When(OrganizationIssueSettingsModified e)
        {
            var issues = Toolkits.Issues;
            issues.EnableWorkflowManagement = e.Issues.EnableWorkflowManagement;
            issues.DisplayOnlyConnectedCases = e.Issues.DisplayOnlyConnectedCases;
            issues.DefaultCandidateUploadFileView = e.Issues.DefaultCandidateUploadFileView;
            issues.DefaultAdministratorUploadFileView = e.Issues.DefaultAdministratorUploadFileView;
            issues.PortalUploadClaimGroups = e.Issues.PortalUploadClaimGroups.EmptyIfNull().Select(x => x).ToArray();
        }

        public void When(OrganizationLocalizationModified e)
        {
            Languages = e.Languages.Select(x => new CultureInfo(x)).ToArray();
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById(e.TimeZone);
        }

        public void When(OrganizationLocationModified e)
        {
            var location = PlatformCustomization.TenantLocation;
            location.LocationType = e.Location.LocationType;
            location.Description = e.Location.Description.NullIfEmpty();
            location.TimeZone = e.Location.TimeZone.NullIfEmpty();
            location.Street = e.Location.Street.NullIfEmpty();
            location.City = e.Location.City.NullIfEmpty();
            location.Province = e.Location.Province.NullIfEmpty();
            location.PostalCode = e.Location.PostalCode.NullIfEmpty();
            location.Country = e.Location.Country.NullIfEmpty();
            location.Phone = e.Location.Phone.NullIfEmpty();
            location.TollFree = e.Location.TollFree.NullIfEmpty();
            location.Mobile = e.Location.Mobile.NullIfEmpty();
            location.Fax = e.Location.Fax.NullIfEmpty();
            location.Email = e.Location.Email.NullIfEmpty();
        }

        public void When(OrganizationNCSHASettingsModified e)
        {
            var settings = Toolkits.NCSHA;
            settings.ShowLastYearToEveryone = e.Settings.ShowLastYearToEveryone;
        }

        public void When(OrganizationOpened e)
        {
            AccountOpened = e.ChangeTime;
            AccountClosed = null;
            AccountStatus = AccountStatus.Opened;
        }

        public void When(OrganizationParentModified e)
        {
            ParentOrganizationIdentifier = e.ParentOrganizationId;
        }

        public void When(OrganizationPlatformSettingsModified e)
        {
            PlatformCustomization.InlineInstructionsUrl = e.InlineInstructionsUrl.NullIfEmpty();
            PlatformCustomization.InlineLabelsUrl = e.InlineLabelsUrl.NullIfEmpty();
            PlatformCustomization.SafeExamBrowserUserAgentSuffix = e.SafeExamBrowserUserAgentSuffix.NullIfEmpty();
        }

        public void When(OrganizationPlatformUrlModified e)
        {
            var url = PlatformCustomization.PlatformUrl;
            url.Logo = e.Url.Logo.NullIfEmpty();
            url.Wallpaper = e.Url.Wallpaper.NullIfEmpty();
            url.Support = e.Url.Support.NullIfEmpty();
            url.Contact = e.Url.Contact.NullIfEmpty();
        }

        public void When(OrganizationPortalSettingsModified e)
        {
            var portal = Toolkits.Portal;
            portal.ShowMyDashboard = e.Portal.ShowMyDashboard;
            portal.ShowMyDashboardAfterLogin = e.Portal.ShowMyDashboardAfterLogin;
            portal.NotSendWelcomeMessage = e.Portal.NotSendWelcomeMessage;
        }

        public void When(OrganizationRegistrationSettingsModified e)
        {
            var registration = PlatformCustomization.UserRegistration;
            registration.RegistrationMode = e.Registration.RegistrationMode;
            registration.AutomaticApproval = e.Registration.AutomaticApproval;
            registration.ConvertProvinceAbbreviation = e.Registration.ConvertProvinceAbbreviation;
            registration.FieldMask = e.Registration.FieldMask.Clone();
        }

        public void When(OrganizationSalesSettingsModified e)
        {
            var settings = Toolkits.Sales;
            settings.ProductClassEventVenueGroup = e.Settings.ProductClassEventVenueGroup;
            settings.ProductCustomerGroup = e.Settings.ProductCustomerGroup;
            settings.ManagerGroup = e.Settings.ManagerGroup;
            settings.LearnerGroup = e.Settings.LearnerGroup;
        }

        public void When(OrganizationSecretModified e)
        {
            OrganizationSecret = e.Secret.NullIfEmpty();
        }

        public void When(OrganizationSignInModified e)
        {
            var settings = PlatformCustomization.SignIn;
            settings.AllowGoogleSignIn = e.Settings.AllowGoogleSignIn;
            settings.AllowMicrosoftSignIn = e.Settings.AllowMicrosoftSignIn;
        }

        public void When(OrganizationSiteSettingsModified e)
        {
            var sites = Toolkits.Sites;
            sites.PortalLogo = e.Sites.PortalLogo.NullIfEmpty();
            sites.PublicSiteCacheResetUrl = e.Sites.PublicSiteCacheResetUrl.NullIfEmpty();
        }

        public void When(OrganizationStandardContentLabelsModified e)
        {
            StandardContentLabels = e.Labels.IsEmpty() ? null : string.Join(",", e.Labels);
        }

        public void When(OrganizationStandardSettingsModified e)
        {
            var settings = Toolkits.Standards;
            settings.ShowStandardCategories = e.Standards.ShowStandardCategories;
        }

        public void When(OrganizationSurveySettingsModified e)
        {
            var surveys = Toolkits.Surveys;
            surveys.EnableUserConfidentiality = e.Surveys.EnableUserConfidentiality;
            surveys.LockUserConfidentiality = e.Surveys.LockUserConfidentiality;
            surveys.ResponseUploadClaimGroups = e.Surveys.ResponseUploadClaimGroups.EmptyIfNull().Select(x => x).ToArray();
        }

        public void When(OrganizationTypeModified e)
        {
            OrganizationType = e.Type.NullIfEmpty();
        }

        public void When(OrganizationUploadSettingsModified e)
        {
            var settings = PlatformCustomization.UploadSettings;

            if (e.Settings.Images != null)
                settings.Images = e.Settings.Images.Clone();

            if (e.Settings.Documents != null)
                settings.Documents = e.Settings.Documents.Clone();
        }

        public void When(OrganizationUrlsModified e)
        {
            var url = PlatformCustomization.TenantUrl;
            url.Facebook = e.Url.Facebook.NullIfEmpty();
            url.Twitter = e.Url.Twitter.NullIfEmpty();
            url.LinkedIn = e.Url.LinkedIn.NullIfEmpty();
            url.Instagram = e.Url.Instagram.NullIfEmpty();
            url.YouTube = e.Url.YouTube.NullIfEmpty();
            url.Other = e.Url.Other.NullIfEmpty();
            url.WebSite = e.Url.WebSite.NullIfEmpty();
        }

        #endregion
    }
}