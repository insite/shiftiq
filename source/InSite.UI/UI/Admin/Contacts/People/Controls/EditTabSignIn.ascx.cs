using System;
using System.Collections.Generic;

using Humanizer;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Admin.Contacts.People.Controls
{
    public partial class EditTabSignIn : BaseUserControl
    {
        #region Events

        public event AlertHandler StatusUpdated;

        private void OnAlert(AlertType type, string message) =>
            StatusUpdated?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Properties

        private Guid? UserIdentifier
        {
            get => (Guid?)ViewState[nameof(UserIdentifier)];
            set => ViewState[nameof(UserIdentifier)] = value;
        }

        #endregion

        #region Initialization

        public void ApplyAccessControl()
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //MfaValidator.ServerValidate += (sender, args) =>
            //{
            //    args.IsValid = !MfaEnabled.Checked
            //        || PhoneMobile.Text.HasValue() && new Phone(PhoneMobile.Text).PlainNumber.Length == 10;
            //};
            MfaDisableButton.Click += (sender, args) =>
            {
                var mfa = TUserAuthenticationFactorSearch.GetMFARecordById(new TUserAuthenticationFactorFilter
                {
                    UserIdentifier = UserIdentifier.Value
                });

                if (mfa != null && mfa.OtpMode != OtpModes.None)
                {
                    mfa.OtpMode = OtpModes.None;
                    TUserAuthenticationFactorStore.Update(mfa);
                }

                ShowMfaDetails();

                OnAlert(AlertType.Success, "Multi-factor athentication was successfully reset for this user");
            };
        }

        #endregion

        #region Methods (set/get)

        public void SetSecutiry(bool canEdit, bool canDelete)
        {
            ResetPassword.Visible = canEdit;
            DeleteLink.Visible = canDelete;
        }

        public void SetInputValues(QPerson person)
        {
            var identity = CurrentSessionState.Identity;
            var user = person.User;

            ImpersonateAnchor.Visible =
                TGroupPermissionSearch.AllowImpersonation(identity.Groups)
                && person.UserAccessGranted.HasValue
                && !identity.IsImpersonating;

            if (person.UserAccessGranted.HasValue)
            {
                ImpersonateAnchor.NavigateUrl = "/ui/portal/identity/impersonate?user=" + person.UserIdentifier;
                ImpersonateAnchor.Attributes["class"] = "btn btn-primary";
            }

            UserIdentifier = person.UserIdentifier;

            BindArchiveStatus(person);

            if (person.CreatedBy != Guid.Empty)
            {
                var creator = UserSearch.Bind(person.CreatedBy, x => x.FullName);
                AccountCreatedLabel.Text = $"Account Created <small class='text-body-secondary'>{person.Created.Humanize()} by {creator.IfNullOrEmpty(UserNames.Someone)}</small>";
            }
            else
                AccountCreatedLabel.Text = $"Account Created <small class='text-body-secondary'>{person.Created.Humanize()}</small>";
            AccountCreatedInfo.Text = TimeZones.Format(person.Created, Identity.User.TimeZone, true, false);

            UserIdentifierOutput.Text = person.UserIdentifier.ToString();
            DeleteLink.NavigateUrl = $"/ui/admin/contacts/people/delete?user={UserIdentifier}";

            IsUserAccessGranted.Checked = person.UserAccessGranted.HasValue;
            if (person.UserAccessGranted.HasValue && person.UserAccessGrantedBy.HasValue())
            {
                IsUserAccessGranted.Text = $"Access Granted to Organization <small class='text-body-secondary'>{person.UserAccessGranted.Humanize()} by {person.UserAccessGrantedBy ?? UserNames.Someone}</small>";
                IsUserAccessGrantedDateTime.Visible = true;
                IsUserAccessGrantedDateTime.Text = TimeZones.Format(person.UserAccessGranted.Value, Identity.User.TimeZone, true, false);
            }
            else if (person.UserAccessGranted.HasValue)
            {
                IsUserAccessGranted.Text = $"Access Granted to Organization";
                IsUserAccessGrantedDateTime.Visible = true;
                IsUserAccessGrantedDateTime.Text = TimeZones.Format(person.UserAccessGranted.Value, Identity.User.TimeZone, true, false);
            }
            else
            {
                IsUserAccessGrantedDateTime.Text = string.Empty;
                IsUserAccessGrantedDateTime.Visible = false;
                IsUserAccessGranted.Text = "Access not Granted to Organization";
            }

            IsUserAccountApprovedCmds.Visible = ServiceLocator.Partition.IsE03();
            IsUserAccountApprovedCmds.Checked = user.AccessGrantedToCmds;

            UserAccountApproved.Value = person.JobsApproved;

            IsUserLicenseAccepted.Checked = user.UserLicenseAccepted.HasValue;
            IsUserLicenseAccepted.Text = user.UserLicenseAccepted.HasValue ? $"License Accepted {user.UserLicenseAccepted.Value.Humanize()}" : "Licensed";
            IsUserLicenseAccepted.Enabled = user.UserLicenseAccepted.HasValue;
            IsUserLicenseAcceptedDateTime.Text = user.UserLicenseAccepted.HasValue ? TimeZones.Format(user.UserLicenseAccepted.Value, Identity.User.TimeZone, true, false) : "";
            IsUserLicenseAcceptedDateTime.Visible = user.UserLicenseAccepted.HasValue;

            if (user.UserPasswordExpired != null)
            {
                PasswordExpires.Enabled = true;
                PasswordExpires.DefaultTimeZone = User.TimeZone.Id;
                PasswordExpires.Value = user.UserPasswordExpired;
            }
            else
            {
                PasswordExpires.Value = null;
            }

            var count = ServiceLocator.PersonSearch.CountPersons(new QPersonFilter { UserIdentifier = person.UserIdentifier });
            AccountType.Text = count > 1
                ? "Multi-Organization User"
                : (count == 1 ? "Single-Organization User" : "Zero-Organization User");

            MfaDisabled.Checked = !user.MultiFactorAuthentication;
            MfaEnabled.Checked = user.MultiFactorAuthentication;

            ShowMfaDetails();

            //MfaPhoneMobile.Text = person.PhoneMobile.HasValue() ? person.PhoneMobile : "None";
            //SendSmsButton.Visible = person.PhoneMobile.HasValue() && new Phone(person.PhoneMobile).PlainNumber.Length == 10;

            LoadRoles();
        }

        private void BindArchiveStatus(QPerson person)
        {
            ModifyArchiveStatus.NavigateUrl = $"/ui/admin/contacts/people/archive?user={person.UserIdentifier}";

            ArchiveStatus.Text = "<span class='text-success'>Not Archived</span>";

            var archived = person.User.UtcArchived;

            if (archived.HasValue)
            {
                ArchiveStatus.Text = "<span class='text-danger'>Archived</span>";
                ArchiveStatusHelp.Text = $"This user was archived on {archived:MMM d, yyyy} ({archived.Humanize()})";

                // If the user is archived (as opposed to the person) then disallow modification 
                // here. The user must be unarchived before the person's archive status is modified.
                ModifyArchiveStatus.Visible = false;

                return;
            }

            archived = person.WhenArchived;

            if (archived.HasValue)
            {
                ArchiveStatus.Text = "<span class='text-danger'>Archived</span>";
                ArchiveStatusHelp.Text = $"This person was archived on {archived:MMM d, yyyy} ({archived.Humanize()})";
                return;
            }

            var unarchived = person.WhenUnarchived;

            if (unarchived.HasValue)
            {
                ArchiveStatusHelp.Text = $"This person was unarchived on {unarchived:MMM d, yyyy} ({unarchived.Humanize()})";
                return;
            }
        }

        private void ShowMfaDetails()
        {
            var mfa = TUserAuthenticationFactorSearch.GetMFARecordById(new TUserAuthenticationFactorFilter
            {
                UserIdentifier = UserIdentifier.Value
            });

            CurrentMfaMode.Text = (mfa?.OtpMode ?? OtpModes.None).GetDescription();

            MfaDisableField.Visible = mfa != null && mfa.OtpMode != OtpModes.None;
        }

        public void GetInputValues(QUser user, QPerson person)
        {
            person.UserIdentifier = user.UserIdentifier;

            if (IsUserAccessGranted.Checked)
            {
                if (!person.UserAccessGranted.HasValue)
                {
                    person.UserAccessGranted = DateTimeOffset.UtcNow;
                    person.UserAccessGrantedBy = User.FullName;
                    person.AccessRevoked = null;
                    person.AccessRevokedBy = null;
                }
            }
            else
            {
                if (person.UserAccessGranted.HasValue && !person.AccessRevoked.HasValue)
                {
                    person.UserAccessGranted = null;
                    person.UserAccessGrantedBy = null;
                    person.AccessRevoked = DateTimeOffset.UtcNow;
                    person.AccessRevokedBy = User.FullName;
                }
            }

            user.AccessGrantedToCmds = IsUserAccountApprovedCmds.Checked;
            person.JobsApproved = UserAccountApproved.Value;

            if (!IsUserLicenseAccepted.Checked && user.UserLicenseAccepted.HasValue)
                user.UserLicenseAccepted = null;

            if (PasswordExpires.Value.HasValue)
                user.UserPasswordExpired = PasswordExpires.Value.Value;

            user.MultiFactorAuthentication = MfaEnabled.Checked;
        }

        public void Save()
        {
            SaveRoles();
        }

        #endregion

        #region Methods (roles)

        private void LoadRoles()
        {
            BindRoles();
        }

        private void BindRoles()
        {
            var roles = ServiceLocator.GroupSearch.GetUserRoles(Organization.Identifier, UserIdentifier.Value);

            var isE03 = ServiceLocator.Partition.IsE03();
            if (isE03 && Organization.ParentOrganizationIdentifier.HasValue)
                AppendRoles(Organization.ParentOrganizationIdentifier.Value, roles);

            RoleCheckList.DataSource = roles;
            RoleCheckList.DataValueField = "GroupIdentifier";
            RoleCheckList.DataTextField = "GroupName";
            RoleCheckList.DataBind();

            var canModifyAdminRoles = MembershipPermissionHelper.CanModifyAdminMemberships();
            var hasDisabledRoles = false;

            foreach (var role in roles)
            {
                var item = RoleCheckList.Items.FindByValue(role.GroupIdentifier.ToString());

                item.Selected = role.Selected;
                item.Enabled = canModifyAdminRoles || !role.OnlyOperatorCanAddUser || role.Selected;

                if (!item.Enabled)
                    hasDisabledRoles = true;
            }

            SupportEmail.Text = string.Format("<a href='mailto:{0}'>{0}</a>", ServiceLocator.Partition.Email);

            PermissionInfo.Visible = hasDisabledRoles;
        }

        private void AppendRoles(Guid organizationId, List<UserRoleItem> list)
        {
            var roles = ServiceLocator.GroupSearch.GetUserRoles(organizationId, UserIdentifier.Value);
            foreach (var role in roles)
                list.Add(role);
        }

        private void SaveRoles()
        {
            if (!RoleCheckList.Visible)
                return;

            var user = UserSearch.Select(UserIdentifier.Value);

            foreach (System.Web.UI.WebControls.ListItem item in RoleCheckList.Items)
            {
                var groupId = Guid.Parse(item.Value);
                if (item.Selected)
                {
                    if (MembershipPermissionHelper.CanModifyMembership(groupId))
                        MembershipHelper.Save(groupId, user.UserIdentifier, "Membership");
                }
                else
                    MembershipStore.Delete(MembershipSearch.Select(groupId, user.UserIdentifier));
            }
        }

        #endregion
    }
}