using System;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio
{
    public abstract class BaseView : Page
    {
        #region Fields

        private Repeater
            _educationList,
            _industryInterestSectorList,
            _industryInterestAreaList,
            _professionalDesignationList,
            _professionalMembershipList,
            _volunteerExperienceList,
            _otherTrainingList;

        private HtmlGenericControl
            _professionalDesignationPanel,
            _professionalMembershipPanel,
            _volunteerExperiencePanel,
            _otherTrainingPanel;

        private System.Web.UI.WebControls.Literal
            _userName,
            _activelySeeking
            ;

        private HtmlGenericControl
            _languageAssessmentTestTakenBlock,
            _languageAssessmentTestNotTakenText;

        private Repeater
            _languagesRepeater;

        private Person _currentUser;
        private bool _isCurrentUserLoaded;

        #endregion

        #region Properties

        protected virtual bool IsCheckAuthentication => true;

        protected Guid OrganizationID => CurrentSessionState.Identity.Organization.Identifier;

        public virtual Person CurrentUser
        {
            get
            {
                if (!_isCurrentUserLoaded)
                {
                    _currentUser = null;

                    if (CurrentSessionState.Identity != null)
                    {
                        if (CurrentSessionState.Identity.User != null)
                            _currentUser = PersonSearch.Select(OrganizationID, CurrentSessionState.Identity.User.UserIdentifier);
                    }

                    _isCurrentUserLoaded = true;
                }

                return _currentUser;
            }
        }

        public virtual string CurrentUserRole
        {
            get
            {
                if (CurrentUser == null)
                    return null;

                return CurrentUser.JobsApproved.HasValue
                    ? "Employer"
                    : "Candidate";
            }
        }

        protected abstract Guid ContactIdentifier { get; }

        protected abstract Guid ContactId { get; set; }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            if (HandleAjaxRequest(Page, CurrentUser))
                return;

            base.OnInit(e);

            if (IsCheckAuthentication && !Request.IsAuthenticated)
                InSite.Web.SignIn.SignInLogic.RedirectToSignInWithReturnURL(Request.RawUrl, true);

            if (!IsPostBack)
                _isCurrentUserLoaded = false;

            _educationList = (Repeater)ControlHelper.GetControl(this, "EducationList");
            _industryInterestSectorList = (Repeater)ControlHelper.GetControl(this, "IndustryInterestSectorList");
            _industryInterestAreaList = (Repeater)ControlHelper.GetControl(this, "IndustryInterestAreaList");
            _professionalDesignationList = (Repeater)ControlHelper.GetControl(this, "ProfessionalDesignationList");
            _professionalMembershipList = (Repeater)ControlHelper.GetControl(this, "ProfessionalMembershipList");
            _volunteerExperienceList = (Repeater)ControlHelper.GetControl(this, "VolunteerExperienceList");
            _otherTrainingList = (Repeater)ControlHelper.GetControl(this, "OtherTrainingList");

            _professionalDesignationPanel = (HtmlGenericControl)ControlHelper.GetControl(this, "ProfessionalDesignationPanel");
            _professionalMembershipPanel = (HtmlGenericControl)ControlHelper.GetControl(this, "ProfessionalMembershipPanel");
            _volunteerExperiencePanel = (HtmlGenericControl)ControlHelper.GetControl(this, "VolunteerExperiencePanel");
            _otherTrainingPanel = (HtmlGenericControl)ControlHelper.GetControl(this, "OtherTrainingPanel");

            _userName = (System.Web.UI.WebControls.Literal)ControlHelper.GetControl(this, "UserName");
            _activelySeeking = (System.Web.UI.WebControls.Literal)ControlHelper.GetControl(this, "ActivelySeeking");

            _languageAssessmentTestTakenBlock = (HtmlGenericControl)ControlHelper.GetControl(this, "LanguageAssessmentTestTakenBlock");
            _languageAssessmentTestNotTakenText = (HtmlGenericControl)ControlHelper.GetControl(this, "LanguageAssessmentTestNotTakenText");

            _languagesRepeater = (Repeater)ControlHelper.GetControl(this, "LanguagesRepeater");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();
            }
        }

        public static bool HandleAjaxRequest(Page page, Person CurrentUser)
        {
            if (!HttpRequestHelper.IsAjaxRequest || !bool.TryParse(page.Request.Form["IsPageAjax"], out bool isAjax) || !isAjax)
                return false;

            page.Response.Clear();

            var action = page.Request.Form["action"];

            switch (action)
            {
                case "iecbc_send_contact_request":
                    {
                        page.Response.ContentType = "application/json";

                        var candidateId = Guid.Parse(page.Request.Form["candidateId"]);
                        var employerName = page.Request.Form["employerName"];
                        var emailAddress = page.Request.Form["emailAddress"];
                        var companyName = page.Request.Form["companyName"];
                        var message = page.Request.Form["message"];

                        if (CurrentUser != null)
                        {
                            var organization = CurrentSessionState.Identity.Organization;
                            var contact = PersonSearch.Select(organization.Identifier, candidateId, x => x.User);

                            if (contact != null)
                            {
                                var approved = contact.JobsApproved.HasValue;

                                if (!string.IsNullOrEmpty(employerName) && !string.IsNullOrEmpty(emailAddress) && !string.IsNullOrEmpty(companyName) && !string.IsNullOrEmpty(message) &&
                                    approved)
                                {

                                    ServiceLocator.AlertMailer.Send(organization.Identifier, candidateId, new AlertJobsCandidateContactRequested
                                    {
                                        CandidateFirstName = contact.User.FirstName,
                                        CandidateLastName = contact.User.LastName,
                                        CompanyName = companyName,
                                        Message = message,
                                        EmailAddress = emailAddress,
                                        EmployerName = employerName,
                                    });

                                    if (CurrentUser.EmployerGroupIdentifier.HasValue)
                                        MembershipStore.Save(MembershipFactory.Create(candidateId, CurrentUser.EmployerGroupIdentifier.Value, organization.Identifier, "Jobs Candidate"));

                                    page.Response.StatusCode = (int)HttpStatusCode.OK;
                                    page.Response.Write(JsonConvert.SerializeObject(new { success = true }));
                                }
                                else
                                {
                                    page.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    page.Response.Write(JsonConvert.SerializeObject(new { error = "User is not approved yet" }));
                                }
                            }
                            else
                            {
                                page.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                page.Response.Write(JsonConvert.SerializeObject(new { error = "Record not found or insufficient permissions" }));
                            }
                        }
                        else
                        {
                            page.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            page.Response.Write(JsonConvert.SerializeObject(new { error = "Your account is not registered yet" }));
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException($"Unexpected action name: {action}");
            }

            page.Response.End();

            return true;
        }

        #endregion

        #region Load & Save

        protected virtual void Open()
        {
            var contact = PersonSearch.Select(OrganizationID, ContactIdentifier, x => x.User, x => x.HomeAddress);
            if (contact == null)
            {
                Response.Redirect("/");
                return;
            }

            ContactId = contact.UserIdentifier;

            SetInputValues(contact);

            LoadLanguages(contact);
            LoadEducation();
            LoadIndustryInterestSectors();
            LoadIndustryInterestAreas();
            LoadProfessionalDesignations();
            LoadProfessionalMemberships();
            LoadVolunteerExperience();
            LoadOtherTrainings();
        }

        #endregion

        #region Setting and getting input values

        protected virtual void SetInputValues(Person candidate)
        {
            _userName.Text = $"{candidate.User.FirstName} {candidate.User.LastName}";

            _activelySeeking.Text = GetString(candidate.CandidateIsActivelySeeking);
        }

        #endregion

        #region Load data

        private string GetBarClass(int testValue)
        {
            string barClass = string.Empty;

            switch (testValue)
            {
                case 0:
                    barClass = "level-limited";
                    break;
                case 1:
                    barClass = "level-basic";
                    break;
                case 2:
                case 3:
                    barClass = "level-advanced";
                    break;
                default:
                    barClass = "level-fluent";
                    break;
            }

            return barClass;
        }

        protected virtual void LoadLanguages(Person candidate)
        {
            var proficiencies = TCandidateLanguageProficiencySearch.SelectByUser(candidate.UserIdentifier, candidate.OrganizationIdentifier);

            var data = proficiencies
                .Select(x => new
                {
                    LanguageName = TCollectionItemCache.GetName(x.LanguageItemIdentifier),
                    ProficiencyDescription = x.ProficiencyLevel >= 0 && x.ProficiencyLevel < ProficiencyDescriptions.List.Length
                        ? ProficiencyDescriptions.List[x.ProficiencyLevel]
                        : null,
                    LanguageLevel = x.ProficiencyLevel,
                    BarClass = GetBarClass(x.ProficiencyLevel),
                    BarWidth = $"width: {Calculator.GetPercentInteger(x.ProficiencyLevel, 5)}%;"
                })
                .ToList();

            _languageAssessmentTestNotTakenText.Visible = data.Count == 0;
            _languageAssessmentTestTakenBlock.Visible = data.Count > 0;

            _languagesRepeater.DataSource = data;
            _languagesRepeater.DataBind();
        }

        protected virtual void LoadEducation()
        {
            var data = TCandidateEducationSearch.SelectByContact(ContactId);

            _educationList.DataSource = data.OrderByDescending(x => x.EducationDateFrom).ThenBy(x => x.EducationInstitution);
            _educationList.DataBind();
        }

        protected virtual void LoadIndustryInterestSectors()
        {
            if (_industryInterestSectorList == null)
                return;

            var data = TPersonFieldSearch.Bind(
                x => x,
                x => x.OrganizationIdentifier == OrganizationID && x.UserIdentifier == ContactId && x.FieldName == "Industry Interest Sector",
                null,
                nameof(TPersonField.FieldSequence)
            );

            _industryInterestSectorList.DataSource = data;
            _industryInterestSectorList.DataBind();
        }

        protected virtual void LoadIndustryInterestAreas()
        {
            var data = TPersonFieldSearch.Bind(
                x => x,
                x => x.OrganizationIdentifier == OrganizationID && x.UserIdentifier == ContactId && x.FieldName == "Industry Interest Area",
                null,
                nameof(TPersonField.FieldSequence)
            );

            _industryInterestAreaList.DataSource = data;
            _industryInterestAreaList.DataBind();
        }

        protected virtual void LoadProfessionalDesignations()
        {
            var data = TPersonFieldSearch.Bind(
                x => x,
                x => x.OrganizationIdentifier == OrganizationID && x.UserIdentifier == ContactId && x.FieldName == "Professional Designation",
                null,
                nameof(TPersonField.FieldSequence)
            );

            _professionalDesignationList.DataSource = data;
            _professionalDesignationList.DataBind();

            _professionalDesignationPanel.Visible = data.Count > 0;
        }

        protected virtual void LoadProfessionalMemberships()
        {
            var data = TPersonFieldSearch.Bind(
                x => x,
                x => x.OrganizationIdentifier == OrganizationID && x.UserIdentifier == ContactId && x.FieldName == "Professional Membership",
                null,
                nameof(TPersonField.FieldSequence)
            );

            _professionalMembershipList.DataSource = data;
            _professionalMembershipList.DataBind();

            _professionalMembershipPanel.Visible = data.Count > 0;
        }

        protected virtual void LoadVolunteerExperience()
        {
            var data = TPersonFieldSearch.Bind(
                x => x,
                x => x.OrganizationIdentifier == OrganizationID && x.UserIdentifier == ContactId && x.FieldName == "Volunteer Experience",
                null,
                nameof(TPersonField.FieldSequence)
            );

            _volunteerExperienceList.DataSource = data;
            _volunteerExperienceList.DataBind();

            _volunteerExperiencePanel.Visible = data.Count > 0;
        }

        protected virtual void LoadOtherTrainings()
        {
            var data = TPersonFieldSearch.Bind(
                x => x,
                x => x.OrganizationIdentifier == OrganizationID && x.UserIdentifier == ContactId && x.FieldName == "Other Training",
                null,
                nameof(TPersonField.FieldSequence)
            );

            _otherTrainingList.DataSource = data;
            _otherTrainingList.DataBind();

            _otherTrainingPanel.Visible = data.Count > 0;
        }

        #endregion

        #region Helper methods

        protected virtual string GetString(bool val)
        {
            return val ? "Yes" : "No";
        }

        protected virtual string GetString(bool? val)
        {
            if (val.HasValue) return GetString(val.Value);

            return "-";
        }

        protected virtual string GetString(int? val)
        {
            if (val.HasValue) return val.Value.ToString();

            return "-";
        }

        protected virtual string GetString(string str)
        {
            if (!string.IsNullOrEmpty(str)) return str;

            return "-";
        }

        protected virtual string GetDateString(DateTimeOffset date)
        {
            return date.ToString("MMM d, yyyy");
        }

        protected virtual string GetDateString(DateTimeOffset? date)
        {
            if (date.HasValue) return GetDateString(date.Value);

            return "-";
        }

        protected virtual string GetYear(DateTime? date)
        {
            if (date.HasValue) return date.Value.Year.ToString();

            return "Present";
        }

        protected virtual string GetIndustryInterestAreaText(string areaText)
        {
            var occupation = StandardSearch.SelectFirst(x =>
                x.OrganizationIdentifier == OrganizationID
                && x.StandardType == "Profile"
                && x.StandardLabel == "Occupation"
                && x.SourceDescriptor == "custom_iecbc.CandidateOccupation"
                && x.Code == areaText
            );

            return occupation != null
                ? occupation.ContentTitle
                : areaText;
        }

        #endregion
    }
}