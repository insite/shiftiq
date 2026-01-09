using System;
using System.Linq;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;
using InSite.Common.Web.UI;
using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;

using CheckBoxList = System.Web.UI.WebControls.CheckBoxList;
using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Cmds.Admin.People.Controls
{
    public partial class PersonDetails : BaseUserControl
    {
        #region Constants

        private const string TraineeGroupName = "Skills Passport";

        public static readonly string FieldPermission = PermissionNames.Custom_CMDS_Fields;

        #endregion

        #region Initialization and PreRender

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Country.ValueChanged += Country_ValueChanged;

            ValidateEmailUnique.ServerValidate += ValidateEmailUnique_ServerValidate;
        }

        #endregion

        #region Security

        private void ApplyUserGroupAssignmentPermissions()
        {
            ApplyUserGroupAssignmentPermission(UserRoleList, CmdsRole.Administrators, CmdsRole.Programmers);
            ApplyUserGroupAssignmentPermission(UserRoleList, CmdsRole.CollegeAdministrators, CmdsRole.Programmers);
            ApplyUserGroupAssignmentPermission(UserRoleList, CmdsRole.FieldAdministrators, CmdsRole.Programmers, CmdsRole.SystemAdministrators, CmdsRole.OfficeAdministrators);
            ApplyUserGroupAssignmentPermission(UserRoleList, CmdsRole.Impersonators, CmdsRole.Programmers);
            ApplyUserGroupAssignmentPermission(UserRoleList, CmdsRole.OfficeAdministrators, CmdsRole.Programmers);
            ApplyUserGroupAssignmentPermission(UserRoleList, CmdsRole.Programmers, CmdsRole.Programmers);
            ApplyUserGroupAssignmentPermission(UserRoleList, CmdsRole.SuperValidators, CmdsRole.Programmers);
            ApplyUserGroupAssignmentPermission(UserRoleList, CmdsRole.SystemAdministrators, CmdsRole.Programmers);
            ApplyUserGroupAssignmentPermission(UserRoleList, CmdsRole.Testers, CmdsRole.Programmers);
            ApplyUserGroupAssignmentPermission(UserRoleList, CmdsRole.Validators, CmdsRole.Programmers, CmdsRole.SystemAdministrators);

            ApplyUserGroupAssignmentPermission(UserRoleList, $"{TraineeGroupName} Administrators", $"{TraineeGroupName} Administrators", $"{TraineeGroupName} Developers");
            ApplyUserGroupAssignmentPermission(UserRoleList, $"{TraineeGroupName} Developers", $"{TraineeGroupName} Developers");
            ApplyUserGroupAssignmentPermission(UserRoleList, $"{TraineeGroupName} Instructors", $"{TraineeGroupName} Administrators");
            ApplyUserGroupAssignmentPermission(UserRoleList, $"{TraineeGroupName} Users", $"{TraineeGroupName} Administrators");

            ApplyUserGroupAssignmentPermission(UserRoleList, "TSSC Only Users", CmdsRole.Administrators, CmdsRole.Programmers, CmdsRole.SystemAdministrators);
        }

        private void ApplyUserGroupAssignmentPermission(CheckBoxList list, string role, params string[] accessors)
        {
            var item = list.Items.FindByText(role);
            if (item == null)
                return;

            var isVisible = accessors.Any(accessor => Identity.IsInRole(accessor));
            if (!isVisible)
                list.Items.Remove(item);
        }

        #endregion

        #region Properties

        public Guid? UserID
        {
            get => (Guid?)ViewState[nameof(UserID)];
            set => ViewState[nameof(UserID)] = value;
        }

        public bool IsUserDetailsVisible => LoginTab.Visible;

        public bool HasRole => (bool)ViewState[nameof(HasRole)];

        private void SetHasRole(bool value)
        {
            ViewState[nameof(HasRole)] = value;
        }

        #endregion

        #region Event handlers

        private void Country_ValueChanged(object sender, EventArgs e) => OnCountryChanged();

        private void OnCountryChanged()
        {
            ProvinceSelector.Value = null;
            ProvinceSelector.Country = Country.Value;
            ProvinceSelector.RefreshData();
            ProvinceSelectorView.IsActive = true;

            ProvinceText.Text = null;
        }

        private void ValidateEmailUnique_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var matches = UserSearch.Bind(x => x.UserIdentifier, new UserFilter { EmailExact = EmailWork.Text });

            args.IsValid = matches.Length == 0 || matches.Length == 1 && matches[0] == UserID;
        }

        #endregion

        #region Public methods

        public void ShowTimeZone()
        {
            RowTimeZone.Visible = true;
        }

        #endregion

        #region Getting and setting input values

        public void SetInputValues(Persistence.User user, Person person)
        {
            var isNew = user.UserIdentifier == Guid.Empty;
            var hasPerson = person != null;

            if (!hasPerson)
                person = new Person();

            if (!isNew)
            {
                UserID = user.UserIdentifier;

                FirstName.Text = user.FirstName;
                MiddleName.Text = user.MiddleName;
                LastName.Text = user.LastName;

                var homeAddress = hasPerson && person.HomeAddressIdentifier != null
                    ? AddressSearch.Select(person.HomeAddressIdentifier.Value)
                    : null;

                if (homeAddress != null)
                {
                    Street1.Text = homeAddress.Street1;
                    City.Text = homeAddress.City;
                    PostalCode.Text = homeAddress.PostalCode;

                    Country.EnsureDataBound();
                    Country.Value = homeAddress.Country;

                    OnCountryChanged();

                    if (homeAddress.Province.IsNotEmpty())
                    {
                        var option = ProvinceSelector.FindOptionByValue(homeAddress.Province);
                        if (option != null)
                        {
                            option.Selected = true;
                            ProvinceSelectorView.IsActive = true;
                            ProvinceText.Text = null;
                        }
                        else
                        {
                            ProvinceText.Text = homeAddress.Province;
                            ProvinceTextView.IsActive = true;
                            ProvinceSelector.Value = null;
                        }
                    }
                }
                else if (!hasPerson)
                    HidePersonFields();

                EmailWork.Text = user.Email;
                TimeZone.Value = user.TimeZone;
                PhoneMobile.Text = user.PhoneMobile;

                PhoneWork.Text = person.PhoneWork;
                PhoneHome.Text = person.PhoneHome;
                Birthdate.Value = person.Birthdate;

                PersonCode.Text = person.PersonCode;
                PersonType.Text = person.PersonType;
                EmployeeType.Text = person.EmployeeType;
                JobTitle.Text = person.JobTitle;
                JobDivision.Text = person.JobDivision;
                AgeGroup.Text = person.AgeGroup;

                EmailEnabled.Checked = person.EmailEnabled;

                bool approved = hasPerson && person.UserAccessGranted != null;

                EnableEmailNotificationField.Visible = approved && (
                       Identity.IsInRole(CmdsRole.Programmers)
                    || Identity.IsInRole(CmdsRole.SystemAdministrators)
                    || Identity.IsInRole(CmdsRole.OfficeAdministrators)
                    || Identity.IsInRole(CmdsRole.FieldAdministrators));
            }
            else
            {
                EnableEmailNotificationField.Visible = false;
            }

            var isUserDetailsVisible = Identity.IsGranted(FieldPermission);

            LoginTab.Visible = isUserDetailsVisible;
            GroupsTab.Visible = isUserDetailsVisible;

            if (isUserDetailsVisible)
            {
                LoginTab.Title = user.UtcArchived.HasValue ? "Login (Archived)" : "Login";

                if (hasPerson && person.UserAccessGranted != null)
                {
                    Status.Value = PersonStatusSelector.Approved;
                    StatusTimestamp.Text = $@"{person.UserAccessGrantedBy ?? UserNames.Someone} approved this login {person.UserAccessGranted.Humanize()}";
                }
                else
                {
                    Status.Value = PersonStatusSelector.Disabled;
                    StatusTimestamp.Text = string.Empty;
                }

                Status.Enabled = Identity.IsInRole(CmdsRole.Programmers)
                                 || Identity.IsInRole(CmdsRole.SystemAdministrators)
                                 || Identity.IsInRole(CmdsRole.OfficeAdministrators)
                                 || Identity.IsInRole(CmdsRole.FieldAdministrators)
                                 || Identity.IsOperator;

                PasswordExpires.Enabled = Status.Enabled;
                PasswordExpires.Value = user.UserPasswordExpired;

                LoadRoles1(user.UserIdentifier);
            }

            if (isUserDetailsVisible)
                LoadDuplicates(user);

            SendEmailField.Visible = hasPerson && isUserDetailsVisible && UserID != Guid.Empty && person.UserAccessGranted != null;
            SendEmailButton.NavigateUrl = $"/ui/cmds/admin/users/send?id={UserID}";

            SendEmailButton.Visible = hasPerson && person.EmailEnabled;
            SendEmailDisabled.Visible = !hasPerson || !person.EmailEnabled;

            ShowSentCount();

            LockEmploymentDetails();

            CommentRepeater.LoadData(user.UserIdentifier, Organization.Identifier);
        }

        private void LockEmploymentDetails()
        {
            var isLocked = Organization.Toolkits.Contacts?.ReadOnlyEmploymentDetails ?? false;

            foreach (var control in EmploymentPanel.Controls)
            {
                if (control is Common.Web.UI.TextBox box)
                {
                    box.Enabled = !isLocked;
                }
            }
        }

        private void HidePersonFields()
        {
            Street1Field.Visible = false;
            CityField.Visible = false;
            CountryField.Visible = false;
            ProvinceField.Visible = false;
            PostalCodeField.Visible = false;
            PhoneWorkField.Visible = false;
            PhoneHomeField.Visible = false;
            BirthdateField.Visible = false;
            EmploymentPanel.Visible = false;
        }

        public void GetInputValues(QUser user, QPerson person)
        {
            user.FirstName = FirstName.Text;
            user.MiddleName = MiddleName.Text;
            user.LastName = LastName.Text;
            user.Email = EmailWork.Text;
            user.TimeZone = TimeZone.Visible ? TimeZone.Value : CurrentSessionState.Identity.Organization.TimeZone.Id;
            person.EmailEnabled = EmailEnabled.Checked;

            if (PasswordExpires.Value.HasValue)
                user.UserPasswordExpired = PasswordExpires.Value.Value;

            if (Status.Visible)
            {
                if (Status.IsApproved == true)
                {
                    if (person.UserAccessGranted == null)
                    {
                        person.UserAccessGrantedBy = User.FullName;
                        person.UserAccessGranted = DateTimeOffset.UtcNow;

                        person.EmailEnabled = true;

                        if (user.IsNullPassword())
                            user.SetDefaultPassword();
                    }
                }
                else
                {
                    if (person.UserAccessGranted != null)
                    {
                        person.UserAccessGrantedBy = null;
                        person.UserAccessGranted = null;

                        person.AccessRevoked = DateTimeOffset.UtcNow;
                        person.AccessRevokedBy = User.FullName;

                        person.EmailEnabled = false;
                    }
                }
            }

            user.PhoneMobile = Phone.Format(PhoneMobile.Text);

            if (person != null)
            {
                person.PhoneWork = Phone.Format(PhoneWork.Text);
                person.PhoneHome = Phone.Format(PhoneHome.Text);
                person.Birthdate = Birthdate.Value;

                person.PersonCode = PersonCode.Text;
                person.PersonType = PersonType.Text;
                person.EmployeeType = EmployeeType.Text;
                person.JobTitle = JobTitle.Text;
                person.JobDivision = JobDivision.Text;
                person.AgeGroup = AgeGroup.Text;

                var homeAddress = person.GetAddress(ContactAddressType.Home);
                homeAddress.Street1 = Street1.Text;
                homeAddress.City = City.Text;
                homeAddress.Country = Country.Value;
                homeAddress.Province = ProvinceSelectorView.IsActive
                    ? ProvinceSelector.Value
                    : ProvinceText.Text;
                homeAddress.PostalCode = PostalCode.Text;
            }
        }

        public void SaveRoles(Guid userId)
        {
            SaveCmdsUserGroups(UserRoleList, userId);
        }

        #endregion

        #region Helper methods

        private void ShowSentCount()
        {
            var info = UserID.HasValue
                ? PersonSearch.Select(Organization.Identifier, UserID.Value)
                : null;

            SentCount.Text = info != null
                ? info.WelcomeEmailsSentToUser.ToString()
                : "0";
        }

        private void LoadDuplicates(Persistence.User user)
        {
            var isNew = user.UserIdentifier == Guid.Empty;

            DuplicatesTab.Visible = !isNew;

            if (isNew)
                return;

            Duplicates.SetVisibleColumns(new[] { "Name", "City", "EmailWork" });

            var filter = new PersonFilter
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsArchived = null,
                ExcludeUserIdentifier = user.UserIdentifier
            };

            Duplicates.LoadData(filter);

            DuplicatesTab.Visible = Duplicates.HasRows;

            if (Duplicates.HasRows)
            {
                var text = Duplicates.RowCount == 1
                    ? "is <strong>1</strong> person"
                    : $"are <strong>{Duplicates.RowCount}</strong> people";

                WarningMessage.Text = $@"There {text} in the database who {(Duplicates.RowCount == 1 ? "has" : "have")} the same name (or a name that is similarly pronounced).";
            }
        }

        #endregion

        #region Roles

        private void LoadRoles1(Guid user)
        {
            SetHasRole(false);

            var partition = ServiceLocator.Partition.Identifier;

            var groups = ServiceLocator.GroupSearch.GetUserRoles(partition, user);

            UserRoleList.DataSource = groups;
            UserRoleList.DataValueField = "GroupIdentifier";
            UserRoleList.DataTextField = "GroupName";
            UserRoleList.DataBind();

            foreach (var info in groups)
            {
                if (info.Selected)
                {
                    UserRoleList.Items.FindByValue(info.GroupIdentifier.ToString()).Selected = true;
                    SetHasRole(true);
                }
            }

            ApplyUserGroupAssignmentPermissions();
        }

        private void SaveCmdsUserGroups(CheckBoxList list, Guid userKey)
        {
            if (!list.Visible)
                return;

            var organizationId = Organization.Identifier;

            var user = UserSearch.Select(userKey);

            var memberships = MembershipSearch.Select(x => x.UserIdentifier == userKey, x => x.Group);

            foreach (System.Web.UI.WebControls.ListItem item in list.Items)
            {
                var groupId = Guid.Parse(item.Value);

                if (item.Selected)
                {
                    MembershipHelper.Save(groupId, user.UserIdentifier, "Membership");

                    if (item.Text.StartsWith($"{TraineeGroupName} Users") &&
                        Organization.Toolkits.Contacts.EnableTraineeDepartment
                        )
                    {
                        var departmentId = DepartmentSearch.BindFirst(
                            x => (Guid?)x.DepartmentIdentifier,
                            x => x.DepartmentName == TraineeGroupName && x.OrganizationIdentifier == organizationId);

                        if (departmentId != null)
                            MembershipStore.Save(MembershipFactory.Create(userKey, departmentId.Value, organizationId, "Department"));
                    }
                }
                else
                {
                    MembershipStore.Delete(MembershipSearch.Select(groupId, user.UserIdentifier));
                }
            }

            var isImpersonator = memberships.Any(x => x.Group.GroupName == "CMDS Impersonators");

            user = UserSearch.Select(userKey, x => x.Persons);

            foreach (var person in user.Persons)
                ServiceLocator.SendCommand(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.IsAdministrator, isImpersonator));
        }

        internal void SetPasswordExpires(DateTimeOffset expiry)
        {
            PasswordExpires.Value = expiry;
        }

        #endregion
    }
}