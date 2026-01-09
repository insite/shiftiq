using System;
using System.Linq;
using System.Web.UI;

using InSite.Admin.Contacts.People.Controls;
using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Contacts.Read;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Admin.Contacts.People.Controls;
using InSite.UI.Admin.Reports.Changes.Models;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class Edit : AdminBasePage
    {
        #region Constants

        private const string CreateUrl = "/ui/admin/contacts/people/create";
        private const string SearchUrl = "/ui/admin/contacts/people/search";

        private const string GroupNavigationItemsVarName = "Toolkits.Settings.Navigation.GroupItems";

        public enum UpdateStatus
        {
            None,
            WasNotCreatedBefore,
            WasSharedBefore
        }

        #endregion

        #region Properties

        protected Guid UserId => Guid.TryParse(Request.QueryString["contact"], out var value) ? value : Guid.Empty;

        #endregion

        #region Fields

        private static readonly string[] _employerAddressOrder = new[]
        {
            AddressType.Physical.GetName(),
            AddressType.Shipping.GetName(),
            AddressType.Billing.GetName()
        };

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DetailsTabContent.EmployerSelected += DetailsTabContent_EmployerSelected;
            DetailsTabContent.ImageUpdated += DetailsTabContent_ImageUpdated;

            OtherTabContent.StatusUpdated += TabContent_Alert;
            SignInTabContent.StatusUpdated += TabContent_Alert;

            PeopleIncomingGrid.Refreshed += Connections_Refreshed;
            PeopleOutgoingGrid.Refreshed += Connections_Refreshed;


            SaveButton.Click += SaveButton_Click;

            ResetPasswordUpdatePanel.Request += ResetPasswordUpdatePanel_Request;
            ResetPasswordButton.Click += ResetPasswordButton_Click;

            IsGroupNavigationItems.AutoPostBack = true;
            IsGroupNavigationItems.CheckedChanged += (s, a) =>
            {
                var check = IsGroupNavigationItems.Checked;
                TUserSettingStore.SetValue(OrganizationIdentifiers.Global, UserId, GroupNavigationItemsVarName, "Boolean", check);
                Session[GroupNavigationItemsVarName] = check;
            };
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;

            DetailsTabContent.ApplyAccessControl();
            OtherTabContent.ApplyAccessControl();
            SignInTabContent.ApplyAccessControl();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            OrganizationSubTab.Visible = CurrentSessionState.Identity.IsOperator
                || ServiceLocator.PersonSearch.CountPersons(new QPersonFilter { UserIdentifier = User.UserIdentifier, IsAdministrator = true }) >= 2;
            SettingsSubTab.Visible = CurrentSessionState.Identity.IsOperator;

            SetStatusOnLoad();
            SetPanelOnLoad();

            Open();

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void SetStatusOnLoad()
        {
            var status = Request.QueryString["status"];

            if (status == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);
            else if (status == "combined")
                ScreenStatus.AddMessage(AlertType.Information, $"People are combined.");
        }

        private void SetPanelOnLoad()
        {
            var panel = Request["panel"].EmptyIfNull().ToLower();

            if (panel == "attachments")
            {
                PersonalTab.IsSelected = true;
                DocumentsTab.IsSelected = true;
            }
            else if (panel == "comments")
            {
                PersonalTab.IsSelected = true;
                CommentSubTab.IsSelected = true;
            }
            else if (panel == "people")
            {
                MembershipTab.IsSelected = true;
                PeopleSubTab.IsSelected = true;
            }
            else if (panel == "people-in")
            {
                MembershipTab.IsSelected = true;
                PeopleSubTab.IsSelected = true;
                PeopleSubTabIncomingTab.IsSelected = true;
            }
            else if (panel == "people-out")
            {
                MembershipTab.IsSelected = true;
                PeopleSubTab.IsSelected = true;
                PeopleSubTabOutgoingTab.IsSelected = true;
            }
            else if (panel == "groups")
            {
                MembershipTab.IsSelected = true;
                GroupSubTab.IsSelected = true;
            }
            else if (panel == "referrals")
            {
                MembershipTab.IsSelected = true;
                ReferralSubTab.IsSelected = true;
            }
            else if (panel == "registrations")
            {
                RecordTab.IsSelected = true;
                RegistrationSubTab.IsSelected = true;
            }
            else if (panel == "grades")
            {
                RecordTab.IsSelected = true;
                GradebookSubTab.IsSelected = true;
            }
            else if (Request["panel"] == "credentials")
            {
                RecordTab.IsSelected = true;
                AchievementSubTab.IsSelected = true;
            }
            else if (panel == "outcomes")
            {
                RecordTab.IsSelected = true;
                OutcomeSubTab.IsSelected = true;
            }
            else if (panel == "forms")
            {
                RecordTab.IsSelected = true;
                SurveySubTab.IsSelected = true;
            }
        }

        #endregion

        #region Event handlers

        private void TabContent_Alert(object sender, AlertArgs args)
        {
            ScreenStatus.AddMessage(args);
        }

        private void DetailsTabContent_EmployerSelected(object sender, EditTabDetails.EmployerArgs args)
        {
            OtherTabContent.OnEmployerChanged(args.NewEmployer);

            var person = new QPerson();
            var isChanged = false;

            AddressList.GetInputValues(person);

            var workAddress = person.WorkAddress;

            workAddress.Trim();

            if (!workAddress.IsEmpty() && args.OldEmployerIdentifier.HasValue)
            {
                var isReset = ServiceLocator.GroupSearch
                    .GetAddresses(args.OldEmployerIdentifier.Value)
                    .Any(x => _employerAddressOrder.Contains(x.AddressType)
                           && workAddress.Equals(x, StringComparison.OrdinalIgnoreCase));

                if (isReset)
                {
                    workAddress = person.WorkAddress = new QPersonAddress();
                    isChanged = true;
                }
            }

            if (args.NewEmployerIdentifier.HasValue && workAddress.IsEmpty())
            {
                var employerAddress = GetEmployerAddress(args.NewEmployerIdentifier.Value);
                if (employerAddress != null)
                {
                    person.WorkAddress = employerAddress;
                    isChanged = true;
                }
            }

            if (isChanged)
                AddressList.SetInputValues(person);
        }

        private void DetailsTabContent_ImageUpdated(object sender, EditTabDetails.ImageUploadArgs args)
        {
            Open();

            ScreenStatus.AddMessage(args.AlertType, args.Message);
        }

        private void Connections_Refreshed(object sender, EventArgs e)
        {
            GroupGrid.RefreshGrid();

            SetupGroupSubTab();
            SetupPeopleSubTab();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid || !Save())
                return;

            Open();

            SetStatus(ScreenStatus, StatusType.Saved);
        }

        private void ResetPasswordUpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value == "init")
            {
                var password = RandomStringGenerator.CreateUserPassword();

                ResetPasswordInput.Text = password;
                ResetPasswordConfirm.Text = password;
            }
        }

        private void ResetPasswordButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var user = ServiceLocator.UserSearch.GetUser(UserId);
            var password = ResetPasswordInput.Text;

            user.SetPassword(password);
            user.SetDefaultPassword(password);

            UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));

            ScreenStatus.AddMessage(AlertType.Success, "The password has been changed.");

            ScreenStatusUpdatePanel.Update();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Edit),
                "close_reset_password",
                $"modalManager.close('{ResetPasswordWindow.ClientID}');",
                true);
        }

        #endregion

        #region Methods (open)

        public void Open()
        {
            var person = ServiceLocator.PersonSearch.GetPerson(
                UserId,
                Organization.Identifier,
                x => x.User,
                x => x.EmployerGroup,
                x => x.EmployerGroup.Parent,
                x => x.BillingAddress,
                x => x.HomeAddress,
                x => x.ShippingAddress,
                x => x.WorkAddress);

            if (person == null)
            {
                var url = UserId == Guid.Empty ? CreateUrl : SearchUrl;

                HttpResponseHelper.Redirect(url, true);
            }

            if (person.User.AccountCloaked.HasValue && !User.IsCloaked)
                HttpResponseHelper.Redirect(SearchUrl, true);

            var title = $"{person.User.FullName} <span class='form-text'>{person?.PersonCode}</span>";
            if (person.IsArchived || person.User.UtcArchived.HasValue)
                title += " <span class='badge bg-danger'>Archived</span>";

            PageHelper.AutoBindHeader(Page, qualifier: title);

            SetInputValues(person);
        }

        private void SetInputValues(QPerson person)
        {
            // PersonalTab

            DetailsTabContent.SetInputValues(person);
            OtherTabContent.SetInputValues(person);
            DocumentsTabContent.SetInputValues(person.UserIdentifier);

            if (person.EmployerGroupIdentifier.HasValue && (person.WorkAddress == null || person.WorkAddress.IsEmpty()))
            {
                var employerAddress = GetEmployerAddress(person.EmployerGroupIdentifier.Value);
                if (employerAddress != null)
                    person.WorkAddress = employerAddress;
            }

            AddressList.SetInputValues(person);

            CommentRepeater.LoadData(person.UserIdentifier, Organization.Identifier);

            // MembershipTab

            GroupGrid.AllowEdit = CanEdit;
            GroupGrid.LoadData(person.UserIdentifier);
            SetupGroupSubTab();

            ReferralGrid.ReturnQuery = $"contact={person.UserIdentifier}&panel=referrals";
            ReferralGrid.AllowEdit = CanEdit;
            ReferralGrid.LoadData(person.UserIdentifier);
            SetupReferralSubTab();

            PeopleIncomingGrid.LoadData(person, ConnectionDirection.Incoming, CanEdit);
            PeopleOutgoingGrid.LoadData(person, ConnectionDirection.Outgoing, CanEdit);
            SetupPeopleSubTab();

            // RecordTab

            GradebookGrid.LoadData(person.UserIdentifier);

            var canSeeLogbooks = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Records)
                || CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Portal_Logbooks);

            LogbookSubTab.Visible = canSeeLogbooks;

            if (canSeeLogbooks)
            {
                var logbookCount = LogbookList.LoadData(person.UserIdentifier);
                LogbookSubTab.SetTitle("Logbooks", logbookCount);
            }

            var canWriteEvents = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Events, PermissionOperation.Write);
            RegistrationGrid.LoadData(person.UserIdentifier, canWriteEvents);
            SetupRegistrationsTab();


            CredentialGrid.LoadDataByUserID(person.UserIdentifier);
            AchievementSubTab.SetTitle("Achievements", CredentialGrid.RowCount);


            SurveyResponseGrid.LoadForEmployer(new QResponseSessionFilter
            {
                RespondentUserIdentifier = person.UserIdentifier,
                OrganizationIdentifier = Organization.Identifier
            });
            SurveyResponseGrid.RefreshGrid();
            SurveySubTab.SetTitle("Forms", SurveyResponseGrid.RowCount);

            SurveyResponseGrid.Visible = SurveyResponseGrid.HasData;
            NoSurveyResponses.Visible = !SurveyResponseGrid.HasData;

            var hasCanvasAccess = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Integrations_Canvas);
            OutcomeSubTab.Visible = hasCanvasAccess;

            if (hasCanvasAccess)
                OutcomeTree.LoadData(person.UserIdentifier);

            // SystemAccessTab

            SignInSubTab.Visible = person.UserIdentifier != Guid.Empty;
            SignInTabContent.SetSecutiry(CanEdit, CanDelete);
            SignInTabContent.SetInputValues(person);


            OrganizationList.LoadData(person.UserIdentifier);


            AuthenticationGrid.LoadData(person.UserIdentifier, Organization.OrganizationIdentifier);


            IsUserAccountCloaked.Checked = person.User.AccountCloaked.HasValue;
            IsUserAccountCloaked.Visible = User.IsCloaked;

            IsGroupNavigationItems.Checked = TUserSettingStore.GetValue<bool>(OrganizationIdentifiers.Global, UserId, GroupNavigationItemsVarName, "Boolean", false);

            AccountVisibilityField.Visible = User.IsCloaked;
        }

        private void SetupPeopleSubTab()
        {
            PeopleSubTabIncomingTab.Visible = PeopleIncomingGrid.RowCount > 0;

            PeopleSubTabIncomingTab.SetTitle("Upstream", PeopleIncomingGrid.RowCount);
            PeopleSubTabOutgoingTab.SetTitle("Downstream", PeopleOutgoingGrid.RowCount);

            PeopleSubTab.SetTitle(
                "People",
                new ControlHelper.TabSubtitleCollection(x => x.Count.Value > 0)
                {
                    { "<i class='fas fa-level-up-alt'></i>", PeopleIncomingGrid.RowCount },
                    { "<i class='fas fa-level-down-alt'></i>", PeopleOutgoingGrid.RowCount }
                },
                false);
        }

        private void SetupGroupSubTab()
        {
            GroupSubTab.SetTitle("Groups", GroupGrid.RowCount);
        }

        private void SetupReferralSubTab()
        {
            ReferralSubTab.SetTitle("Referrals", ReferralGrid.RowCount);
        }

        private void SetupRegistrationsTab()
        {
            RegistrationSubTab.SetTitle("Registrations", RegistrationGrid.RowCount);
        }

        #endregion

        #region Methods (save)

        public bool Save()
        {
            var user = ServiceLocator.UserSearch.GetUser(UserId);
            if (user == null)
                HttpResponseHelper.Redirect(SearchUrl, true);

            var person = ServiceLocator.PersonSearch.GetPerson(
                user.UserIdentifier,
                Organization.OrganizationIdentifier,
                x => x.HomeAddress,
                x => x.WorkAddress,
                x => x.BillingAddress,
                x => x.ShippingAddress
            );

            if (person == null)
                HttpResponseHelper.Redirect(SearchUrl, true);

            var contactState = Shift.Common.ObjectComparer.GetSnapshot(user, QUser.DiffExclusions);

            var accessGrantedBefore = person.UserAccessGranted;
            var emailBefore = user.Email;

            GetInputValues(user, person);

            if (user.Email.IsEmpty())
            {
                ScreenStatus.AddMessage(AlertType.Error, "The email address must be specified for user.");
                return false;
            }

            if (UserSearch.IsEmailDuplicate(user.UserIdentifier, user.Email))
            {
                if (person.UserAccessGranted.HasValue)
                {
                    ScreenStatus.AddMessage(AlertType.Error, $"There is another user already registered with the login name <strong>{user.Email}</strong>");
                    return false;
                }
                else
                {
                    user.Email = UniqueIdentifier.Create().ToString() + "@insitemessages.com";
                }
            }

            var isAccessGranted = person.UserAccessGranted.HasValue && accessGrantedBefore != person.UserAccessGranted;
            var isPasswordReset = false;

            if (person.UserAccessGranted.HasValue && user.IsNullPassword())
                user.SetDefaultPassword();

            if (isAccessGranted && user.IsDefaultPassword() && user.DefaultPasswordExpired < DateTimeOffset.UtcNow)
            {
                var password = RandomStringGenerator.CreateUserPassword();

                user.SetPassword(password);
                user.SetDefaultPassword(password);

                isPasswordReset = true;
            }

            var beforeUpdate = DateTimeOffset.UtcNow;
            UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));

            try
            {
                if (person.PersonIdentifier != Guid.Empty)
                    PersonStore.Update(person);
                else
                {
                    person.UserIdentifier = user.UserIdentifier;
                    PersonStore.Insert(person);
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                var message = ex?.InnerException?.InnerException?.Message;
                if (message != null && message.StartsWith("Cannot insert duplicate key row in object 'contacts.Person' with unique index 'IX_Person_PersonCode'."))
                {
                    ScreenStatus.AddMessage(AlertType.Error, $"The code <strong>{person.PersonCode}</strong> is already assigned to a different person in the {Organization.CompanyName} account.");
                    return false;
                }
                throw ex;
            }

            DetailsTabContent.Save();
            OtherTabContent.Save();
            SignInTabContent.Save();

            ShowChanges(beforeUpdate);

            if (isPasswordReset)
                ScreenStatus.AddMessage(AlertType.Success, "The password has been reset.");

            if (isAccessGranted && person.EmailEnabled)
            {
                PersonHelper.SendWelcomeMessage(Organization.OrganizationIdentifier, user.UserIdentifier);
                ScreenStatus.AddMessage(AlertType.Success, "Welcome email sent.");
            }

            var changes = Shift.Common.ObjectComparer.Compare(contactState, user);

            var isNameChanged = changes.Any(x => x.PropertyName == nameof(Persistence.User.FullName));
            if (isNameChanged)
                ServiceLocator.ChangeQueue.Publish(new PersonRenamed(user.UserIdentifier, user.FirstName, user.LastName));

            var isEmailChanged = changes.Any(x => x.PropertyName == nameof(Persistence.User.Email));
            if (isEmailChanged)
                ServiceLocator.ChangeQueue.Publish(new PersonEmailChanged(user.UserIdentifier, user.Email));

            var utcArchivedChange = changes.FirstOrDefault(x => x.PropertyName == nameof(Persistence.User.UtcArchived));
            var utcUnarchivedChange = changes.FirstOrDefault(x => x.PropertyName == nameof(Persistence.User.UtcUnarchived));

            if (utcArchivedChange != null && utcArchivedChange.ValueAfter != null)
            {
                ServiceLocator.AlertMailer.Send(Organization.OrganizationIdentifier, user.UserIdentifier, new AlertUserAccountArchived
                {
                    TenantIdentifier = Organization.OrganizationIdentifier,
                    Email = user.Email,
                    Name = user.FullName,
                    Status = "Archived"
                });
            }
            else if (utcUnarchivedChange != null && utcUnarchivedChange.ValueAfter != null)
            {
                ServiceLocator.AlertMailer.Send(Organization.OrganizationIdentifier, user.UserIdentifier, new AlertUserAccountArchived
                {
                    TenantIdentifier = Organization.OrganizationIdentifier,
                    Email = user.Email,
                    Name = user.FullName,
                    Status = "Not Archived"
                });
            }

            Web.SignIn.SignInLogic.UpdateLoginName(emailBefore, user.Email);

            return true;
        }

        private void ShowChanges(DateTimeOffset beforeUpdate)
        {
            var changeHtml = HistoryReader.ReadUserLatestAsHtml(UserId, Organization.Identifier, beforeUpdate);
            if (changeHtml.IsNotEmpty())
                ScreenStatus.AddMessage(AlertType.Information, changeHtml);
        }

        private void GetInputValues(QUser user, QPerson person)
        {
            AddressList.GetInputValues(person);

            DetailsTabContent.GetInputValues(user, person);
            OtherTabContent.GetInputValues(user, person);
            SignInTabContent.GetInputValues(user, person);

            if (IsUserAccountCloaked.Checked && !user.AccountCloaked.HasValue)
                user.AccountCloaked = DateTimeOffset.Now;
            else if (!IsUserAccountCloaked.Checked && user.AccountCloaked.HasValue)
                user.AccountCloaked = null;
        }

        #endregion

        #region Methods (helpers)

        internal static QPersonAddress GetEmployerAddress(Guid employerId)
        {
            var employerAddresses = ServiceLocator.GroupSearch.GetAddresses(employerId);
            var copyAddress = _employerAddressOrder
                .Select(x => employerAddresses.FirstOrDefault(y => y.AddressType == x))
                .Where(x => x != null && x.HasAddress)
                .FirstOrDefault();

            if (copyAddress == null)
                return null;

            return new QPersonAddress
            {
                City = copyAddress.City,
                Country = copyAddress.Country,
                Description = copyAddress.Description,
                PostalCode = copyAddress.PostalCode,
                Province = copyAddress.Province,
                Street1 = copyAddress.Street1,
                Street2 = copyAddress.Street2
            };
        }

        #endregion
    }
}