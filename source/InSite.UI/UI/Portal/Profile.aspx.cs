using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Contacts.Read;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Common.Controls;
using InSite.UI.Layout.Portal;
using InSite.UI.Lobby.Utilities;
using InSite.Web.Data;
using InSite.Web.Helpers;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal
{
    public partial class Profile : PortalBasePage
    {
        #region Classes

        private class ReminderItem
        {
            public bool? HasReminder { get; set; }
            public Guid? Message { get; set; }
        }

        #endregion

        #region Fields

        private static readonly PortalFieldHandler<NavItem> _fieldHandler = new PortalFieldHandler<NavItem>();

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PersonCodeUniqueValidator.ServerValidate += PersonCodeUniqueValidator_ServerValidate;
            EmailUniqueValidator.ServerValidate += EmailUniqueValidator_ServerValidate;

            EmployerGroupIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;

            SaveButton.Click += SaveButton_Click;
            SaveButtonTop.Click += SaveButton_Click;

            ProfilePictureUploadControl.ProfileUploadCompleted += ProfilePictureUploadCompletedHandler;

            RegenerateSecret.Click += (x, y) =>
            {
                var person = ServiceLocator.PersonSearch.GetPerson(User.Identifier, Organization.Identifier);

                var personId = person.PersonIdentifier;

                TokenHelper.GetClientSecret(personId, true);

                HttpResponseHelper.Redirect("/ui/portal/profile");
            };

            GenerateBearer.Click += (x, y) =>
            {
                var person = ServiceLocator.PersonSearch.GetPerson(User.Identifier, Organization.Identifier, p => p.Secrets);

                var secret = person.Secrets.FirstOrDefault(s => s.SecretName == SecretName.ShiftClientSecret);

                var response = RetrieveJwt(secret.SecretValue);

                var token = response.AccessToken;

                var expiry = DateTimeOffset.Now.AddSeconds(response.ExpiresIn);

                var lifetime = TimeSpan.FromSeconds(response.ExpiresIn).Humanize();

                var validUntil = Shift.Common.TimeZones.Format(expiry, User.TimeZone) + $" ({lifetime} from now)";

                var instructions = $@"
<div class='alert alert-success fs-sm'>
<p>Your new token has been created successfully. It is valid until <strong>{validUntil}</strong>. Copy it now, because you will not be able to retrieve it again after leaving this page. (For your security, this token is <strong>not</strong> saved on our servers, so if you lose it before it expires, you will need to generate a new one.)</p>
<p>Store your token in a secure place within your application or password manager.</p>
<p>Your token is a <em>Bearer Authorization JSON Web Token</em>. Use it in your API requests via the Authorization header.</p>
</div>
";
                ApiAccessToken.InnerText = token;
                ApiAccessTokenInstruction.Text = instructions;
                ApiAccessTokenPanel.Visible = true;
            };
        }

        private JwtResponse RetrieveJwt(string secret)
        {
            var data = new { Secret = secret, Debug = true };

            {
                var oneDay = 24 * 60 * 60; // 86,400 seconds

                var request = new JwtRequest { Secret = secret, Lifetime = oneDay };

                var serializer = new JsonSerializer2();

                var baseAddress = new Uri(ServiceLocator.AppSettings.Shift.Api.Hosting.V2.BaseUrl);

                var client = new ApiClientSynchronous(new HttpClientFactory(baseAddress, secret), serializer);

                var response = client.HttpPost<JwtResponse>("security/tokens/generate", request);

                return response.Data;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            if (!IsPostBack)
                Open();

            DeveloperDocsUrl.HRef = ServiceLocator.Urls.DeveloperHelpUrl;
            DeveloperDocsUrl.InnerText = DeveloperDocsUrl.HRef;
        }

        #endregion

        #region Event handlers

        private void ProfilePictureUploadCompletedHandler(object sender, ProfileUploadEventArgs e)
        {
            if (e.Success)
                ChangesSavedToastMessage(e.Message);
            else
                ChangesErrorToastMessage(e.Message);
        }

        private void PersonCodeUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !ServiceLocator.PersonSearch.IsPersonExist(Organization.Identifier, args.Value, User.Identifier);
        }

        private void EmailUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var emailUser = ServiceLocator.UserSearch.GetUserByEmail(args.Value);

            EmailUniqueValidator.ErrorMessage = $"There is another user already registered with the login name <strong>{HttpUtility.HtmlEncode(args.Value)}</strong>. " +
                "If this is a valid email that belongs solely to you, please use the Support button above to report the issue to an administrator.";

            args.IsValid = emailUser == null || emailUser.UserIdentifier == User.UserIdentifier;
        }

        private void CertificateButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("/ui/portal/record/credentials/learners/search");
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid || !Save())
                return;

            ChangesSavedToastMessage("Your changes are saved.");
        }

        private void GroupTypeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var identity = CurrentSessionState.Identity;
            var item = (string)e.Item.DataItem;
            var repeater = (Repeater)e.Item.FindControl("GroupNameRepeater");

            var memberships = MembershipSearch
                .SelectMembershipDetails(null, identity.User.UserIdentifier)
                .Where(x => x.GroupType == item)
                .ToList();

            foreach (var membership in memberships)
                membership.IsActive = identity.IsInGroup(membership.GroupName);

            repeater.DataSource = memberships;
            repeater.DataBind();
        }

        #endregion

        #region Methods (set/get)

        private void SetControlsVisibility()
        {
            EmployerGroupIdentifier.Enabled = Identity.IsGranted("ui/portal/profile/employer-write");
        }

        private void Open()
        {
            FeatureFlagCard.Visible = Identity.IsAdministrator && ServiceLocator.AppSettings.Environment.IsPreProduction();

            FeatureFlagHeading.InnerText = ServiceLocator.Partition.GetPlatformName() + " Feature Flags";

            var (user, person) = GetUserAndPerson();

            SetControlsVisibility();

            PageHelper.AutoBindHeader(this);

            SetupFieldsConfiguration(person);

            if (!string.IsNullOrEmpty(person?.PersonCode))
            {
                PortalMaster.Breadcrumbs.BindTitleAndSubtitleNoTranslate(
                    ActionModel.ActionName,
                    GetDisplayText(ActionModel.ActionName)
                    + $"<span class='pt-2 d-block fs-6'>{GetDisplayText(LabelHelper.GetLabelContentText("Person Code"))}: {HttpUtility.HtmlEncode(person.PersonCode)}</span>", null);
            }

            Language.Settings.IncludeLanguage = CurrentSessionState.Identity.Organization.Languages
                .Select(x => x.TwoLetterISOLanguageName)
                .Append(MultilingualString.DefaultLanguage)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            Language.RefreshData();

            FirstName.Text = user.FirstName;
            MiddleName.Text = user.MiddleName;
            LastName.Text = user.LastName;

            UnionInfo.Text = person?.EmployeeUnion;

            EmergencyContactName.Text = person?.EmergencyContactName;
            EmergencyContactPhone.Text = person?.EmergencyContactPhone;
            EmergencyContactRelationship.Text = person?.EmergencyContactRelationship;
            FirstLanguage.Checked = string.Equals(person?.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase) ? true : false;
            Language.Value = person?.Language;
            TimeZone.Value = user.TimeZone;

            Email.Text = string.IsNullOrEmpty(user.Email) ? string.Empty : user.Email.ToLower();

            EmployerGroupIdentifier.Value = person?.EmployerGroupIdentifier;

            ProfilePictureUploadControl.LoadProfilePicture(user);

            JobTitle.Text = person?.JobTitle;
            PersonCode.Text = person?.PersonCode;
            Phone.Text = person?.Phone;
            PhoneHome.Text = person?.PhoneHome;
            PhoneWork.Text = person?.PhoneWork;
            PhoneMobile.Text = user.PhoneMobile;
            PhoneOther.Text = person?.PhoneOther;

            AddressList.SetInputValues(person);

            CancelButton.NavigateUrl = CancelButtonTop.NavigateUrl = RelativeUrl.PortalHomeUrl;
            MFAButtonTop.NavigateUrl = "/ui/portal/identity/authenticate";
            ChangePasswordButtonTop.NavigateUrl = "/ui/portal/identity/password";

            DeveloperPanel.Visible = Identity.IsDeveloper;

            var toast = ToastItem.UrlDecode(Page.Request["toast"]);
            if (toast != null)
            {
                ProfileToast.Indicator = toast.Color;
                ProfileToast.Icon = toast.Icon;
                ProfileToast.Title = toast.Header;
                ProfileToast.Text = toast.Body;
                ProfileToast.Visible = true;
            }

            if (Page.Master is PortalMaster h)
                h.SidebarVisible(false);

            var secret = ServiceLocator.PersonSecretSearch
                .GetByPerson(person.PersonIdentifier, SecretName.ShiftClientSecret);

            if (secret != null)
                PersonSecretValue.Text = secret.SecretValue;

            ApiBaseUrl2.Text = ServiceLocator.AppSettings.Shift.Api.Hosting.V2.BaseUrl;
            ApiBaseUrl1.Text = ServiceLocator.AppSettings.Shift.Api.Hosting.V1.BaseUrl;
        }

        private void SetupFieldsConfiguration(QPerson person)
        {
            var organizationFields = Organization.Fields.User;
            foreach (var defaultField in PortalFieldInfo.UserProfile)
            {
                var orgField = organizationFields?.FirstOrDefault(x => x.FieldName == defaultField.FieldName);
                _fieldHandler.Init(AccountSection, defaultField, orgField, null);
            }

            if (UnionInfoField.Visible && person == null)
                UnionInfoField.Visible = false;

            PersonalGroup.Visible = true;
            PersonalGroup.Visible = FirstNameField.Visible
                || MiddleNameField.Visible
                || LastNameField.Visible
                || EmailField.Visible
                || LanguageField.Visible
                || TimeZoneField.Visible
                || FirstLanguageField.Visible;

            EmploymentGroup.Visible = true;
            EmploymentGroup.Visible = EmployerGroupIdentifierField.Visible
                || JobTitleField.Visible
                || PersonCodeField.Visible
                || UnionInfoField.Visible;

            EmergencyContactGroup.Visible = true;
            EmergencyContactGroup.Visible = EmergencyContactNameField.Visible
                || EmergencyContactPhoneField.Visible
                || EmergencyContactRelationshipField.Visible;

            PhoneNumbersGroup.Visible = true;
            PhoneNumbersGroup.Visible = PhoneField.Visible
                || PhoneHomeField.Visible
                || PhoneWorkField.Visible
                || PhoneMobileField.Visible
                || PhoneOtherField.Visible;
        }

        private bool Save()
        {
            var userAndPerson = GetUserAndPerson();
            var user = userAndPerson.User;
            var person = userAndPerson.Person;

            user.FirstName = FirstName.Text;
            user.MiddleName = MiddleName.Text;
            user.LastName = LastName.Text;
            user.TimeZone = TimeZone.Value;
            user.Email = Email.Text;
            user.PhoneMobile = Shift.Common.Phone.Format(PhoneMobile.Text);

            UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));

            if (person != null)
            {
                person.Language = Language.Value;
                person.JobTitle = JobTitle.Text;
                person.PersonCode = PersonCode.Text;
                person.Phone = Shift.Common.Phone.Format(Phone.Text);
                person.PhoneHome = Shift.Common.Phone.Format(PhoneHome.Text);
                person.PhoneWork = Shift.Common.Phone.Format(PhoneWork.Text);
                person.PhoneOther = Shift.Common.Phone.Format(PhoneOther.Text);

                person.EmergencyContactName = EmergencyContactName.Text;
                person.EmergencyContactPhone = Shift.Common.Phone.Format(EmergencyContactPhone.Text);
                person.EmergencyContactRelationship = EmergencyContactRelationship.Text;
                person.EmployerGroupIdentifier = EmployerGroupIdentifier.Value;

                person.EmployeeUnion = UnionInfo.Text;

                DeleteEmployeeMemberships(EmployerGroupIdentifier.Value, user.UserIdentifier);

                if (EmployerGroupIdentifier.HasValue)
                    AddEmployeeMembership(EmployerGroupIdentifier.Value.Value, user.UserIdentifier);

                AddressList.GetInputValues(person);

                PersonStore.Update(person);
            }

            FirstLanguage.Checked = string.Equals(person?.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase) ? true : false;

            var i = CurrentSessionState.Identity;
            i.User.FirstName = user.FirstName;
            i.User.LastName = user.LastName;
            i.User.FullName = $"{user.FirstName} {user.LastName}";
            CurrentSessionState.Identity = i;

            return true;
        }

        #endregion

        #region Helper methods

        private (QUser User, QPerson Person) GetUserAndPerson()
        {
            var user = ServiceLocator.UserSearch.GetUser(User.UserIdentifier);

            var person = ServiceLocator.PersonSearch.GetPerson(user.UserIdentifier, Organization.OrganizationIdentifier,
                x => x.HomeAddress,
                x => x.WorkAddress,
                x => x.BillingAddress,
                x => x.ShippingAddress
            );

            return (user, person);
        }

        protected string GetGroupStatusHtml(object active)
        {
            if (active != null)
                if ((bool)active)
                    return $"<span class='badge bg-success'>{Translate("Active")}</span>";

            return string.Empty;
        }

        private static void ChangesErrorToastMessage(string text)
        {
            var toast = ToastItem.UrlEncode(AlertType.Error, "fas fa-times-circle", "Error", $"{text}");
            HttpResponseHelper.Redirect($"/ui/portal/profile?toast={toast}", false);
        }

        private static void ChangesSavedToastMessage(string text)
        {
            var toast = ToastItem.UrlEncode(AlertType.Success, "fas fa-check-circle", "Success", $"{text}");
            HttpResponseHelper.Redirect($"/ui/portal/profile?toast={toast}", false);
        }

        private static void DeleteEmployeeMemberships(Guid? groupId, Guid userId)
        {
            var employments = MembershipSearch.Select(
                x => x.UserIdentifier == userId
                  && x.MembershipType == MembershipType.Employee,
                x => x.Group);

            foreach (var employment in employments)
            {
                if (employment.GroupIdentifier == groupId)
                    continue;

                var employmentDistrict = MembershipSearch.SelectFirst(x =>
                    x.UserIdentifier == userId
                    && x.GroupIdentifier == employment.Group.ParentGroupIdentifier
                );

                if (employmentDistrict != null)
                    MembershipStore.Delete(employmentDistrict);

                MembershipStore.Delete(MembershipSearch.Select(employment.GroupIdentifier, employment.UserIdentifier));
            }
        }

        private static void AddEmployeeMembership(Guid groupId, Guid userId)
        {
            if (MembershipSearch.Exists(x => x.UserIdentifier == userId && x.GroupIdentifier == groupId)
                || !MembershipPermissionHelper.CanModifyMembership(groupId)
                )
            {
                return;
            }

            var employer = ServiceLocator.GroupSearch.GetGroup(groupId);
            if (employer == null)
                return;

            var newEmployment = new Membership
            {
                GroupIdentifier = groupId,
                UserIdentifier = userId,
                MembershipType = MembershipType.Employee,
                Assigned = DateTimeOffset.UtcNow
            };

            MembershipHelper.Save(newEmployment);

            if (!employer.ParentGroupIdentifier.HasValue)
                return;

            MembershipHelper.Save(new Membership
            {
                GroupIdentifier = employer.ParentGroupIdentifier.Value,
                UserIdentifier = userId,
                Assigned = DateTimeOffset.UtcNow
            });
        }

        #endregion
    }
}