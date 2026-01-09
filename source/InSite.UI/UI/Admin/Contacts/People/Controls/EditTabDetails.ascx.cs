using System;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Common.Controls;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Contacts.People.Controls
{
    public partial class EditTabDetails : BaseUserControl
    {
        #region Events

        public class ImageUploadArgs : EventArgs
        {
            public string Message { get; }
            public AlertType AlertType { get; }

            public ImageUploadArgs(string message, AlertType alertType)
            {
                Message = message;
                AlertType = alertType;
            }
        }

        public delegate void ImageUpdatedHandler(object sender, ImageUploadArgs args);

        public event ImageUpdatedHandler ImageUpdated;

        private void OnImageUpdated(string message, AlertType alertType) =>
            ImageUpdated?.Invoke(this, new ImageUploadArgs(message, alertType));


        public class EmployerArgs : EventArgs
        {
            public Guid? OldEmployerIdentifier { get; }
            public Guid? NewEmployerIdentifier { get; }

            public QGroup NewEmployer { get; }

            public EmployerArgs(Guid? oldEmployerId, Guid? newEmployerId, QGroup newEmployer)
            {
                OldEmployerIdentifier = oldEmployerId;
                NewEmployerIdentifier = newEmployerId;

                NewEmployer = newEmployer;
            }
        }

        public delegate void EmployerHandler(object sender, EmployerArgs args);

        public event EmployerHandler EmployerSelected;

        private void OnEmployerSelected(Guid? oldId, Guid? newId, QGroup employer) =>
            EmployerSelected?.Invoke(this, new EmployerArgs(oldId, newId, employer));

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
            EmailRequiredValidator.Visible = true;
            EmailRequiredValidator.Enabled = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EmployerGroupIdentifier.AutoPostBack = true;
            EmployerGroupIdentifier.ValueChanged += (sender, args) =>
            {
                var employer = OnEmployerGroupIdentifierSelectedIndexChanged();
                OnEmployerSelected(args.OldValue, args.NewValue, employer);
            };

            UniquePersonCode.ServerValidate += UniquePersonCode_ServerValidate;

            ProfilePictureUploadControl.ProfileUploadCompleted += ProfilePictureUploadCompletedHandler;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            AccountStatusId.Settings.CollectionName = CollectionName.Contacts_People_Membership_Status;
            AccountStatusId.Settings.OrganizationIdentifier = Organization.Key;
            AccountStatusId.EmptyMessage = $"{Organization.Code.ToUpper()} Membership Status";
            AccountStatusId.RefreshData();

            OccupationIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            OccupationIdentifier.ListFilter.StandardTypes = new[] { Shift.Constant.StandardType.Profile };
            OccupationIdentifier.RefreshData();
        }

        #endregion

        #region Event handlers

        private void ProfilePictureUploadCompletedHandler(object sender, ProfileUploadEventArgs e)
        {
            if (e.Success)
                OnImageUpdated(e.Message, AlertType.Success);
            else
                OnImageUpdated(e.Message, AlertType.Error);
        }

        private QGroup OnEmployerGroupIdentifierSelectedIndexChanged()
        {
            var employerId = EmployerGroupIdentifier.Value;
            var employer = employerId.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(employerId.Value)
                : null;

            var status = string.Empty;
            var label = "Jobs and Employment";

            if (employer != null)
            {
                status = TCollectionItemCache.GetName(employer.GroupStatusItemIdentifier);

                if (employer.GroupLabel.IsNotEmpty())
                    label = employer.GroupLabel;
            }

            EmployerBadge.Visible = status.IsNotEmpty();
            EmployerBadge.InnerText = status;

            EmployerTag.InnerText = label;

            return employer;
        }

        private void UniquePersonCode_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Value))
                return;

            var persons = ServiceLocator.PersonSearch.GetPersonsByPersonCodes(new[] { args.Value }, Organization.Key);

            args.IsValid = persons.Count == 0 || persons[0].UserIdentifier == UserIdentifier;
        }

        #endregion

        #region Methods (set/get)

        public void SetInputValues(QPerson person)
        {
            var user = person.User;

            UserIdentifier = person.UserIdentifier;

            MoreInfoButton.Items.Clear();

            foreach (var item in InSite.Admin.Contacts.People.Forms.Report.MoreInfoItems)
                MoreInfoButton.Items.Add(new DropDownButtonLinkItem
                {
                    Text = item.Text,
                    NavigateUrl = $"/ui/admin/contacts/people/report?contact={person.UserIdentifier}#{item.Anchor}"
                });

            SendEmail.Visible = person.UserAccessGranted != null && user.UserPasswordHash != null;
            ((DropDownButtonLinkItem)SendEmail.Items["Correspondence"]).NavigateUrl = $"/ui/admin/contacts/people/send-email?contact={person.UserIdentifier}&type=correspondence";
            ((DropDownButtonLinkItem)SendEmail.Items["Welcome"]).NavigateUrl = $"/ui/admin/contacts/people/send-email?contact={person.UserIdentifier}&type=welcome";

            EmployerGroupIdentifier.Filter.OrganizationIdentifier = Organization.Key;

            EmployerGroupIdentifier.Value = person.EmployerGroupIdentifier;
            OnEmployerGroupIdentifierSelectedIndexChanged();

            var hasEmployerGroup = person.EmployerGroup != null;

            EmployerGroupLink.Visible = hasEmployerGroup;

            if (hasEmployerGroup)
                EmployerGroupLink.NavigateUrl = $"/ui/admin/contacts/groups/edit?contact={person.EmployerGroup.GroupIdentifier}";

            FirstName.Text = user.FirstName;
            MiddleName.Text = user.MiddleName;
            LastName.Text = user.LastName;
            JobTitle.Text = person.JobTitle;
            JobDivision.Text = person.JobDivision;
            ConsentToShare.Value = person.ConsentToShare;

            OccupationIdentifier.ValueAsGuid = person.OccupationStandardIdentifier;

            PersonType.InnerText = person?.PersonType;
            PersonType.Visible = PersonType.InnerText.IsNotEmpty();
            ContactCode.Text = person?.PersonCode;

            AssociationStartDate.Value = person.MemberStartDate;
            AssociationEndDate.Value = person.MemberEndDate;
            Birthdate.Value = person.Birthdate;

            EmergencyContactName.Text = person.EmergencyContactName;
            EmergencyContactPhoneNumber.Text = person.EmergencyContactPhone;
            EmergencyContactRelationship.Text = person.EmergencyContactRelationship;

            if (Organization.IsAssociation)
                AccountStatusLabel.InnerText = $"{Organization.Code.ToUpper()} Membership Status";

            AccountStatusId.ValueAsGuid = person.MembershipStatusItemIdentifier;

            Email.Text = string.IsNullOrWhiteSpace(user.Email)
                ? string.Empty
                : user.Email.ToLower();
            Email.Enabled = true;
            EmailDisabled.Checked = !person.EmailEnabled;
            MarketingEmailDisabled.Checked = !person.MarketingEmailEnabled;
            EmailCommand.Visible = !string.IsNullOrEmpty(user.Email);
            EmailCommand.NavigateUrl = $"mailto:{user.Email}";
            EmailAlternate.Text = string.IsNullOrWhiteSpace(user.EmailAlternate)
                ? string.Empty
                : user.EmailAlternate.ToLower();
            EmailAlternateDisabled.Checked = !person.EmailAlternateEnabled;

            GenderCombo.EnsureDataBound();
            var gender = GenderCombo.FindOptionByValue(person.Gender);
            if (gender != null)
            {
                gender.Selected = true;
                GenderBadge.Visible = false;
            }
            else
            {
                GenderBadge.InnerText = person.Gender;
                GenderBadge.Visible = true;
            }

            Phone.Text = person.Phone;
            PhoneHome.Text = person.PhoneHome;
            PhoneWork.Text = person.PhoneWork;
            PhoneMobile.Text = user.PhoneMobile;
            PhoneOther.Text = person.PhoneOther;

            ProfilePictureUploadControl.LoadProfilePicture(user);
        }

        public void GetInputValues(QUser user, QPerson person)
        {
            person.EmployerGroupIdentifier = EmployerGroupIdentifier.Value;

            user.FirstName = FirstName.Text;
            user.MiddleName = MiddleName.Text;
            user.LastName = LastName.Text;
            person.JobTitle = JobTitle.Text;
            person.JobDivision = JobDivision.Text;
            person.OccupationStandardIdentifier = OccupationIdentifier.ValueAsGuid;

            person.PersonCode = ContactCode.Text;

            person.MemberStartDate = AssociationStartDate.Value;
            person.MemberEndDate = AssociationEndDate.Value;
            person.Birthdate = Birthdate.Value;

            person.EmergencyContactName = EmergencyContactName.Text;
            person.EmergencyContactPhone = Shift.Common.Phone.Format(EmergencyContactPhoneNumber.Text);
            person.EmergencyContactRelationship = EmergencyContactRelationship.Text;

            person.MembershipStatusItemIdentifier = AccountStatusId.ValueAsGuid;

            user.Email = Email.Text;
            person.EmailEnabled = !EmailDisabled.Checked;
            person.MarketingEmailEnabled = !MarketingEmailDisabled.Checked;
            user.EmailAlternate = EmailAlternate.Text;
            person.EmailAlternateEnabled = !EmailAlternateDisabled.Checked;

            if (GenderCombo.Value.IsNotEmpty() || GenderCombo.FindOptionByValue(person.Gender) != null)
                person.Gender = GenderCombo.Value;

            person.Phone = Shift.Common.Phone.Format(Phone.Text);
            person.PhoneHome = Shift.Common.Phone.Format(PhoneHome.Text);
            person.PhoneWork = Shift.Common.Phone.Format(PhoneWork.Text);
            user.PhoneMobile = Shift.Common.Phone.Format(PhoneMobile.Text);
            person.PhoneOther = Shift.Common.Phone.Format(PhoneOther.Text);
        }

        public void Save()
        {
            var groupId = EmployerGroupIdentifier.Value;
            var userId = UserIdentifier.Value;

            DeleteEmployeeMemberships(groupId, userId);

            if (groupId.HasValue)
                AddEmployeeMembership(groupId.Value, UserIdentifier.Value);
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

        internal static void AddEmployeeMembership(Guid groupId, Guid userId)
        {
            if (MembershipSearch.Exists(x => x.UserIdentifier == userId && x.GroupIdentifier == groupId))
                return;

            var employer = ServiceLocator.GroupSearch.GetGroup(groupId);
            if (employer == null || !MembershipPermissionHelper.CanModifyMembership(employer))
                return;

            var newEmployment = new Membership
            {
                GroupIdentifier = groupId,
                UserIdentifier = userId,
                MembershipType = MembershipType.Employee,
                Assigned = DateTimeOffset.UtcNow
            };

            MembershipHelper.Save(newEmployment);

            if (!employer.ParentGroupIdentifier.HasValue || !MembershipPermissionHelper.CanModifyMembership(employer.ParentGroupIdentifier.Value))
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