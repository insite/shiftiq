using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Application.People.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Accounts.Tenants.Models;
using InSite.Domain.Contacts;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Layout.Lobby.Controls;
using InSite.UI.Lobby.Utilities;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Lobby
{
    public partial class Register : Layout.Lobby.LobbyBasePage
    {
        private RegisterState _state;

        #region Properties
        private string RegistrationGroup => Page.Request.QueryString["group"]?.ToString();
        private string ReturnVerifiedUrl => Page.Request.QueryString["returnVerified"]?.ToString();

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Language.AutoPostBack = true;
            Language.ValueChanged += (s, a) =>
            {
                CookieTokenModule.Current.Language = Language.Value;

                var url = HttpRequestHelper.GetCurrentWebUrl();
                url.QueryString.Remove("language");

                HttpResponseHelper.Redirect(url);
            };

            EmployerGroupSelector.Filter.OrganizationIdentifier = Organization.Identifier;
            EmployerGroupSelector.Filter.GroupType = GroupType.Employer.ToString();

            RegisterSubmitButton.Click += RegisterSubmitButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _state = new RegisterState(Organization, Session, Request.QueryString, FormKey.Value);

            if (IsPostBack)
                return;

            Title = LabelHelper.GetTranslation(ActionModel.ActionName);
            LoadSelfSubscriptionRoles();
            SetupCustomContent();
            LoadMiddleNamePanel();

            if (LoadClosedOrganizationView())
                return;

            if (LoadCmdsView())
                return;

            if (LoadRegisterView())
                return;

            LoadDisabledView();

        }

        private void LoadMiddleNamePanel()
        {
            var policy = Organization.Toolkits.Contacts?.FullNamePolicy;
            if (policy != null && policy.Length > 0 && policy.Contains("{Middle}", StringComparison.OrdinalIgnoreCase))
                MiddleNamePanel.Visible = true;
        }

        private bool LoadCmdsView()
        {
            var cmds = Organization != null && ServiceLocator.Partition.IsE03() && !CurrentSessionState.EnableUserRegistration;
            if (cmds)
            {
                ScreenViews.SetActiveView(CmdsView);
                return true;
            }
            return false;
        }

        private void LoadSelfSubscriptionRoles()
        {
            var registerGroup = _state.RequestedGroupName;

            var filter = new QGroupFilter
            {
                OrganizationIdentifier = Organization.Key,
                GroupType = GroupTypes.Role,
                AllowSelfSubscription = true
            };

            var groups = ServiceLocator.GroupSearch.GetGroups(filter);
            var items = groups.Select(x => new GroupCategoryItem
            {
                Value = x.GroupIdentifier,
                Text = x.GroupDescription.IfNullOrEmpty(x.GroupName),
                IsSelected = registerGroup != null && x.GroupName.Equals(registerGroup, StringComparison.OrdinalIgnoreCase),
                Category = x.GroupCategory?.Trim()
            }).ToList();

            bool allCategoriesEmpty = items.All(i => string.IsNullOrEmpty(i.Category));

            var grouped = items
                .GroupBy(x => allCategoriesEmpty ? string.Empty : string.IsNullOrEmpty(x.Category) ? "Others" : x.Category)
                .Select(g => new GroupCategoryList
                {
                    Category = g.Key,
                    Items = g.OrderBy(i => i.Text).ToList()
                })
                .OrderBy(g => g.Category == "Others" ? 1 : 0)
                .ThenBy(g => g.Category)
                .ToList();

            // Build HTML header elements.
            foreach (var category in grouped.Where(x => !string.IsNullOrEmpty(x.Category)))
                category.Heading = $"<h4 class='fs-6 fw-bold'>{category.Category}</h4>";

            // If there is one and only one group then it should be selected by default.
            if (grouped.Count == 1 && ServiceLocator.Partition.IsE03())
            {
                var groupedItems = grouped[0].Items;
                if (groupedItems.Count == 1)
                    groupedItems[0].IsSelected = true;
            }

            GroupList.DataSource = grouped;
            GroupList.DataBind();

            RolePanel.Visible = items.Count > 0
                && (string.IsNullOrEmpty(registerGroup) || !items.Any(i => i.IsSelected));
        }

        private class GroupCategoryList
        {
            public string Heading { get; set; }
            public string Category { get; set; }
            public List<GroupCategoryItem> Items { get; set; }
        }

        private class GroupCategoryItem
        {
            public string Category { get; set; }
            public Guid Value { get; set; }
            public string Text { get; set; }
            public bool IsSelected { get; set; }
        }

        private bool LoadRegisterView()
        {
            if (!IsRegistrationEnabled(Organization, RegistrationGroup))
                return false;

            _state.ServerFormKey = new RegisterFormValidationKey();
            _state.ClientFormKey = _state.ServerFormKey.Id;
            FormKey.Value = _state.ClientFormKey.ToString();

            RegisterSubmitButton.Text = LabelHelper.GetTranslation("Continue");

            { // Company

            }

            { // Fields Visibility
                var r = Organization.PlatformCustomization.UserRegistration;
                RegisterEmployerField.Visible = r.FieldMask.Company;
            }

            ScreenViews.SetActiveView(RegisterView);

            var orgLanguages = CurrentSessionState.Identity.Organization.Languages
                .Select(x => x.TwoLetterISOLanguageName)
                .Append(MultilingualString.DefaultLanguage)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            LanguageField.Visible = orgLanguages.Length > 1;
            Language.Settings.IncludeLanguage = orgLanguages;
            Language.RefreshData();
            Language.Value = CookieTokenModule.Current.Language;

            return true;
        }

        private bool LoadClosedOrganizationView()
        {
            if (!_state.IsOrganizationAccountClosed(Organization))
                return false;

            ClosedOrganizationText.InnerHtml = Markdown.ToHtml(GetDisplayText("User Account Registration Disabled: Organization Account Closed", null).Replace("$TenantName", Organization.Name));
            ScreenViews.SetActiveView(ClosedOrganizationView);
            return true;
        }

        private void LoadDisabledView()
        {
            DisabledText.InnerHtml = Markdown.ToHtml(GetDisplayText("User Account Registration Disabled", null).Replace("$TenantName", Organization.Name));
            ScreenViews.SetActiveView(DisabledView);
        }

        private void LoadExpiredView()
        {
            ScreenViews.SetActiveView(ExpiredView);
        }

        private void LoadSubmittedView()
        {
            SubmittedSignInLink.NavigateUrl = InSite.Web.SignIn.SignInLogic.GetUrl();
            SubmittedSignInLink.Text = LabelHelper.GetTranslation("Sign In");

            ScreenViews.SetActiveView(SubmittedView);
        }

        #endregion

        #region Actions

        private void RegisterSubmitButton_Click(object sender, EventArgs e)
        {
            if (!ValidateSubmit())
                return;

            if (_state.IsExpired)
            {
                LoadExpiredView();
                return;
            }

            var factory = RegisterUser();

            AddUserToGroups(factory.User.UserIdentifier, factory.Person.EmployerGroupIdentifier);
            AddPersonDepartment(factory.Person, Request["group"]);

            QPersonSecret secret = null;

            if (ReturnVerifiedUrl.HasValue())
                secret = TokenHelper.GetClientSecret(factory.Person.PersonIdentifier, false);

            bool isRegistrationSubmitted;
            var autoApprove = Organization.PlatformCustomization.UserRegistration.AutomaticApproval;

            try
            {
                if (autoApprove)
                    SendEmailVerificationRequested(factory);
                else
                    SendOrganizationAccessRequested(factory);

                SendUserRegistrationSubmitted(factory);

                isRegistrationSubmitted = true;
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
                isRegistrationSubmitted = false;
            }

            _state.ServerFormKey.IsSubmitted = isRegistrationSubmitted;

            if (isRegistrationSubmitted && autoApprove)
                ServiceLocator.SendCommand(new GrantPersonAccess(factory.Person.PersonIdentifier, DateTimeOffset.UtcNow, factory.User.FullName));

            if (ReturnVerifiedUrl.HasValue() && secret != null)
                RedirectReturnVerifiedUrl(factory.User, factory.Person, secret);

            if (isRegistrationSubmitted && autoApprove)
                SignInBasePage.RedirectToSignInSucceedPage(true, factory.User.UserIdentifier);

            if (isRegistrationSubmitted)
                LoadSubmittedView();
            else
                ShowRegisterError(Translate("An unexpected error occurred."));
        }

        private void SendEmailVerificationRequested(UserFactory factory)
        {
            ServiceLocator.AlertMailer.Send(Organization.OrganizationIdentifier, factory.User.UserIdentifier, new AlertUserEmailVerificationRequested
            {
                AppUrl = HttpRequestHelper.CurrentRootUrl,
                Organization = Organization.LegalName,
                UserEmail = factory.User.Email,
                UserIdentifier = factory.User.UserIdentifier
            });
        }

        private void SendOrganizationAccessRequested(UserFactory factory)
        {
            ServiceLocator.AlertMailer.Send(Organization.OrganizationIdentifier, factory.User.UserIdentifier, new AlertApplicationAccessRequested
            {
                AppUrl = ServiceLocator.Urls.GetApplicationUrl(Organization.Code),
                Organization = Organization.LegalName,
                SourceUrl = Request.Url.Host + Request.RawUrl,
                UserEmail = factory.User.Email,
                UserIdentifier = factory.User.UserIdentifier,
                UserName = factory.User.FullName
            });
        }

        private void SendUserRegistrationSubmitted(UserFactory factory)
        {
            var homeAddress = factory.Person.GetAddress(ContactAddressType.Home);

            var logo = Organization.PlatformCustomization.PlatformUrl.Logo;

            var organizationLogoUrl = !string.IsNullOrEmpty(logo) && logo.StartsWith("/")
                ? HttpRequestHelper.CurrentRootUrl + logo
                : logo;

            try
            {
                ServiceLocator.AlertMailer.Send(Organization.OrganizationIdentifier, factory.User.UserIdentifier, new AlertUserRegistrationSubmitted
                {
                    ApprovalUrl = HttpRequestHelper.CurrentRootUrl + $"/ui/admin/identity/permissions/grant-access?user={factory.User.UserIdentifier}&organization={Organization.OrganizationCode}",
                    City = homeAddress.City,
                    Email = factory.User.Email,
                    FirstName = factory.User.FirstName,
                    LastName = factory.User.LastName,
                    Organization = Organization.LegalName,
                    Phone = factory.Person.PhoneHome,
                    Province = homeAddress.Province,
                    RegistrationUrl = HttpRequestHelper.CurrentRootUrl + "/ui/lobby/register",
                    Thumbprint = factory.User.UserIdentifier
                });
            }
            catch
            {
                // Intentionally ignored.
            }
        }

        private UserFactory RegisterUser()
        {
            var employer = GetEmployerGroupIdentifier();

            var factory = new UserFactory();
            factory.RegisterUser(
                RegisterEmail.Text.Trim(),
                Organization.Identifier,
                RegisterFirstName.Text,
                RegisterLastName.Text,
                RegisterPassword.Text,
                employer,
                null,
                Language.Visible ? Language.Value : null,
                Organization.Toolkits.Contacts?.DefaultMFA ?? false,
                RegisterMiddleName.Text);

            return factory;
        }

        private void RedirectReturnVerifiedUrl(QUser user, QPerson person, QPersonSecret secret)
        {
            var decodedReturnVerifiedUrl = TokenHelper.DecodeReturnBackUrl(ReturnVerifiedUrl);

            if (decodedReturnVerifiedUrl.HasNoValue())
                return;

            var tokenRequest = new Shift.Common.JwtRequest
            {
                Secret = secret.SecretValue,
                Organization = Organization?.Identifier
            };

            string token = TokenHelper.GenerateToken(tokenRequest, HttpContext.Current.Request.Url.ToString());

            if (token == null)
            {
                ServiceLocator.Logger.Information($"Token not generated for userId: {user.UserIdentifier} orgId: {Organization.Identifier} secret: {secret.SecretValue}");
                token = "INVALID-TOKEN";
            }

            HttpResponseHelper.Redirect($"{decodedReturnVerifiedUrl}?token={token}");

        }

        #endregion

        #region Adding

        private void AddPersonDepartment(QPerson person, string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                AssignPersonDepartment(person, groupName);
            }
            else if (GroupList.Items != null && GroupList.Items.Count > 0)
            {
                foreach (RepeaterItem outer in GroupList.Items)
                {
                    var inner = (Repeater)outer.FindControl("InnerRepeater");
                    foreach (RepeaterItem innerItem in inner.Items)
                    {
                        var toggle = (IRadioButton)innerItem.FindControl("GroupIdentifier");
                        if (toggle.Checked)
                        {
                            AssignPersonDepartment(person, toggle.Text);
                            return;
                        }
                    }
                }
            }
        }

        private static void AssignPersonDepartment(QPerson person, string groupName)
        {
            Guid? departmentId = null;

            if (departmentId.HasValue && MembershipPermissionHelper.CanModifyMembership(departmentId.Value))
            {
                var p = PersonSearch.Select(Organization.Identifier, person.UserIdentifier);
                if (p != null && p.CandidateIsActivelySeeking != true)
                    ServiceLocator.SendCommand(new ModifyPersonFieldBool(p.PersonIdentifier, PersonField.CandidateIsActivelySeeking, true));

                MembershipStore.Save(MembershipFactory.Create(person.UserIdentifier, departmentId.Value, Organization.Identifier));
            }
        }

        private void AddUserToGroups(Guid user, Guid? group)
        {
            if (group.HasValue)
                MembershipHelper.Save(group.Value, user, null);

            if (CurrentSessionState.EnableUserRegistration && (Organization.ParentOrganizationIdentifier == OrganizationIdentifiers.CMDS))
                MembershipHelper.Save(OrganizationIdentifiers.CMDS, GroupTypes.Role, "Skills Passport Users", user, "Membership");

            foreach (RepeaterItem outer in GroupList.Items)
            {
                var inner = (Repeater)outer.FindControl("InnerRepeater");
                foreach (RepeaterItem innerItem in inner.Items)
                {
                    var toggle = (IRadioButton)innerItem.FindControl("GroupIdentifier");
                    if (toggle.Checked)
                    {
                        var groupId = Guid.Parse(toggle.Value);
                        if (MembershipPermissionHelper.CanModifyMembership(groupId))
                            MembershipHelper.Save(groupId, user, null);

                        break;
                    }
                }
            }
        }

        #endregion

        #region Validating

        private bool ValidateSubmit()
        {
            if (!ValidateEmployer())
                return false;

            if (!ValidateRole())
                return false;

            var emailAddress = RegisterEmail.Text.Trim();
            var confirmEmailAddress = RegisterConfirmEmail.Text.Trim();
            if (emailAddress != confirmEmailAddress)
            {
                Status.AddMessage(AlertType.Error, Translate("Emails do not match."));
                return false;
            }

            if (RegisterPassword.Text != RegisterConfirmPassword.Text)
            {
                Status.AddMessage(AlertType.Error, Translate("Password and confirmation does not match."));
                return false;
            }

            if (!RegisterPasswordStrength.Validate())
            {
                Status.AddMessage(AlertType.Error, RegisterPasswordStrength.ValidationError);
                return false;
            }

            return ValidateUser(emailAddress);
        }

        private bool ValidateUser(string emailAddress)
        {
            var contact = UserSearch.BindFirst(
                x => new
                {
                    Organizations = x.Persons.Select(y => y.OrganizationIdentifier).ToList(),
                    x.UserPasswordHash,
                    x.UserLicenseAccepted,
                    x.UserIdentifier,
                    x.FullName
                },
                new UserFilter
                {
                    EmailExact = emailAddress,
                });

            if (contact == null)
                return true;

            if (!contact.Organizations.Contains(Organization.Key))
            {
                PersonStore.Insert(PersonFactory.Create(contact.UserIdentifier, Organization.Key, null, false, null));
                contact.Organizations.Add(Organization.Key);
                return true;
            }

            var person = PersonSearch.Select(Organization.Key, contact.UserIdentifier);

            if (person != null && person.UserAccessGranted.HasValue)
            {
                var errorMessage = GetDisplayText("User Account Registration Failed: Duplicate Email Address", null)
                    .Replace("$RegisterEmail", emailAddress)
                    .Replace("$SignInUrl", InSite.Web.SignIn.SignInLogic.GetUrl())
                    .Replace("$ResetPasswordUrl", ResetPassword.GetUrl());

                Status.AddMessage(AlertType.Error, Markdown.ToHtml(errorMessage));
                return false;
            }

            return true;
        }

        private bool ValidateEmployer()
        {
            if (!RegisterEmployerField.Visible)
                return true;

            var employer = GetEmployerGroupIdentifier();

            if (employer == null)
            {
                Status.AddMessage(AlertType.Error, Translate("Company is a required field."));
                return false;
            }

            return true;
        }

        private bool ValidateRole()
        {
            if (GroupList.Items.Count == 0)
                return true;

            foreach (RepeaterItem outer in GroupList.Items)
            {
                var inner = (Repeater)outer.FindControl("InnerRepeater");
                foreach (RepeaterItem innerItem in inner.Items)
                {
                    var toggle = (IRadioButton)innerItem.FindControl("GroupIdentifier");
                    if (toggle.Checked)
                        return true;
                }
            }

            Status.AddMessage(AlertType.Error, Translate("Role or Program is a required field."));
            return false;
        }

        #endregion

        #region Helper Methods

        private void ShowRegisterError(string error)
        {
            RegisterStatusLiteral.Text += $@"<div class='alert alert-danger'><i class='fas fa-stop-circle'></i>{HttpUtility.HtmlEncode(error)}</div>";
        }

        private void SetupCustomContent()
        {
            CustomContentCard.Visible = false;

            var slug = Request.Url.AbsolutePath.Substring(1).Replace("/", "-");

            var pages = ServiceLocator.PageSearch.Bind(
                x => x.PageIdentifier,
                x => x.OrganizationIdentifier == Organization.Identifier && x.PageSlug == slug);

            if (pages.Length != 1)
                return;

            var contents = ServiceLocator.ContentSearch.SelectContainerByLabel(pages[0], "Body");
            var content = contents.FirstOrDefault(x => x.ContentLanguage == CurrentLanguage);
            if (content?.ContentText == null)
                return;

            CustomContentCard.Visible = true;
            CustomContentHtml.Text = Markdown.ToHtml(content.ContentText);
        }

        internal static bool IsRegistrationEnabled(Domain.Organizations.OrganizationState organization, string groupName)
        {
            if (organization == null)
                return false;

            var userRegistrationMode = Organization.PlatformCustomization?.UserRegistration?.RegistrationMode
                ?? UserRegistrationMode.DisallowSelfRegistration;

            switch (userRegistrationMode)
            {
                case UserRegistrationMode.DisallowSelfRegistration:
                    return false;
                case UserRegistrationMode.AllowSelfRegistrationOnLogin:
                    return true;
                case UserRegistrationMode.AllowSelfRegistrationByLink:
                    if (groupName != null)
                    {
                        var filter = new QGroupFilter
                        {
                            OrganizationIdentifier = Organization.Key,
                            GroupType = GroupTypes.Role,
                            AllowSelfSubscription = true
                        };
                        return ServiceLocator.GroupSearch.GetGroups(filter)
                            .Any(x => x.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
                    }
                    return false;
                default:
                    throw new ArgumentException($"Unknown mode: {userRegistrationMode}");
            }
        }

        private Guid? GetEmployerGroupIdentifier()
        {
            if (!RegisterEmployerField.Visible)
                return null;

            var employer = Guid.Empty;

            if (EmployerGroupTextView.IsActive)
            {
                var groupName = EmployerGroupText.Text.Trim();

                if (groupName.IsNotEmpty())
                {
                    var filter = new QGroupFilter
                    {
                        OrganizationIdentifier = Organization.Identifier,
                        GroupName = groupName,
                        GroupType = GroupTypes.Employer
                    };

                    var group = ServiceLocator.GroupSearch.GetGroups(filter).FirstOrDefault()?.GroupIdentifier;

                    if (group == null)
                    {
                        group = UniqueIdentifier.Create();

                        ServiceLocator.SendCommand(new CreateGroup(group.Value, Organization.Identifier, GroupTypes.Employer, groupName));
                    }

                    employer = group.Value;
                }
            }
            else if (EmployerGroupSelector.HasValue)
            {
                employer = EmployerGroupSelector.Value.Value;
            }

            return employer != Guid.Empty ? employer : (Guid?)null;
        }

        #endregion
    }
}