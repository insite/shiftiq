using System;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.Enumerations;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<PersonFilter>
    {
        #region Properties

        protected override string[] DefaultShowColumns => new[] { "Name", "Email", "Address" };

        public override PersonFilter Filter
        {
            get
            {
                var filter = new PersonFilter
                {
                    OrganizationIdentifier = Organization.Key,

                    FullName = FullName.Text,
                    FirstName = FirstName.Text,
                    LastName = LastName.Text,
                    NameFilterType = NameFilterType.Value,

                    CodeContains = Code.Text,
                    CommentKeyword = CommentKeyword.Text,

                    Gender = Gender.Value,

                    EmailContains = Email.Text,
                    EmailAlternateContains = EmailAlternate.Text,
                    EmailEnabled = EmailEnabled.ValueAsBoolean,
                    EmailStatus = EmailStatus.Value,
                    EmailVerified = EmailVerified.ValueAsBoolean,

                    EmployerParentGroupIdentifier = EmployerParentGroupID.Value,
                    EmployerGroupIdentifier = EmployerGroupID.Value,
                    JobTitle = JobTitle.Text,

                    AccountStatuses = AccountStatusId.ValuesAsGuid.ToArray(),

                    CommentsFlag = CommentFlags.ValuesAsIntArray,
                    IsApproved = IsApproved.ValueAsBoolean,
                    IsArchived = IsArchived.ValueAsBoolean,
                    IsMultiOrganization = IsMultiOrganization.ValueAsBoolean,
                    OrganizationPersonTypes = OrganizationRole.ValuesArray,

                    ValidAchievementIdentifier = ValidAchievementIdentifier.Value,

                    AddressTypes = AddressType.ValuesArray,
                    Country = Country.Value,
                    Provinces = Province.ValuesArray,
                    Cities = City.ValuesArray,
                    Phone = Phone.Text,
                    Region = Region.Value,

                    GroupIdentifier = MembershipGroupID.Value,
                    GroupMembershipDate = GroupMembershipDateField.Visible ? GroupMembershipDate.Value : null,
                    ExcludeGroupIdentifier = ExcludeMembershipGroupID.Value,

                    UtcCreatedSince = CreatedSince.Value?.UtcDateTime,
                    UtcCreatedBefore = CreatedBefore.Value?.UtcDateTime,
                    ModifiedSince = ModifiedSince.Value?.UtcDateTime,
                    ModifiedBefore = ModifiedBefore.Value?.UtcDateTime,

                    LastAuthenticatedSince = LastAuthenticatedSince.Value?.UtcDateTime,
                    LastAuthenticatedBefore = LastAuthenticatedBefore.Value?.UtcDateTime,

                    CloakedUsers = User.IsCloaked ? InclusionType.Include : InclusionType.Exclude,
                    PersonIssue = Issues.Value.ToEnum(PersonCaseType.None)
                };

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                FullName.Text = value.FullName;
                FirstName.Text = value.FirstName;
                LastName.Text = value.LastName;
                NameFilterType.Value = value.NameFilterType;

                Code.Text = value.CodeContains;
                CommentKeyword.Text = value.CommentKeyword;

                Gender.Value = value.Gender;

                Email.Text = value.EmailContains;
                EmailAlternate.Text = value.EmailAlternateContains;
                EmailEnabled.ValueAsBoolean = value.EmailEnabled;
                EmailStatus.Value = value.EmailStatus;
                EmailVerified.ValueAsBoolean = value.EmailVerified;

                EmployerParentGroupID.Value = value.EmployerParentGroupIdentifier;
                OnEmployerParentGroupSelected();
                EmployerGroupID.Value = value.EmployerGroupIdentifier;
                JobTitle.Text = value.JobTitle;

                AccountStatusId.ValuesAsGuid = value.AccountStatuses?.ToList();

                CommentFlags.ValuesAsInt = value.CommentsFlag;
                IsApproved.ValueAsBoolean = value.IsApproved;
                IsArchived.ValueAsBoolean = value.IsArchived;
                IsMultiOrganization.ValueAsBoolean = value.IsMultiOrganization;
                OrganizationRole.Values = value.OrganizationPersonTypes;

                ValidAchievementIdentifier.Value = value.ValidAchievementIdentifier;

                AddressType.Values = value.AddressTypes;
                Country.Value = value.Country;
                Province.Values = value.Provinces;
                City.Values = value.Cities;
                Phone.Text = value.Phone;
                Region.Value = value.Region;

                MembershipGroupID.Value = value.GroupIdentifier;
                OnMembershipGroupChanged();
                GroupMembershipDate.Value = value.GroupMembershipDate;
                ExcludeMembershipGroupID.Value = value.ExcludeGroupIdentifier;

                CreatedSince.Value = value.UtcCreatedSince;
                CreatedBefore.Value = value.UtcCreatedBefore;
                ModifiedSince.Value = value.ModifiedSince;
                ModifiedBefore.Value = value.ModifiedBefore;
                LastAuthenticatedSince.Value = value.LastAuthenticatedSince;
                LastAuthenticatedBefore.Value = value.LastAuthenticatedBefore;

                SortColumns.Value = value.OrderBy;
                Issues.Value = value.PersonIssue.GetName(PersonCaseType.None);
            }
        }

        #endregion

        #region Methods (security)

        private void ApplySecurity()
        {
            EmailStatusField.Visible = true;
        }

        #endregion

        #region Methods (initialization and loading)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EmployerParentGroupID.AutoPostBack = true;
            EmployerParentGroupID.ValueChanged += EmployerParentGroupID_ValueChanged;

            MembershipGroupID.AutoPostBack = true;
            MembershipGroupID.ValueChanged += MembershipGroupID_ValueChanged;

            Region.Settings.CollectionName = CollectionName.Contacts_People_Location_Region;
            Region.Settings.OrganizationIdentifier = Organization.Key;

            AccountStatusId.Settings.CollectionName = CollectionName.Contacts_People_Membership_Status;
            AccountStatusId.Settings.OrganizationIdentifier = Organization.Key;
            AccountStatusId.EmptyMessage = $"{Organization.Code.ToUpper()} Membership Status";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            EmployerParentGroupID.Filter.HasChildren = true;

            EmployerGroupID.Filter.GroupType = GroupTypes.Employer;

            OnGroupOrganizationListChanged();

            ApplySecurity();

            SetCheckAll(AccountStatusId, "Account Status");

            SetCheckAll(OrganizationRole, "Organization Role");
            SetCheckAll(AddressType, "Address Types");
            SetCheckAll(Province, "Provinces");
            SetCheckAll(City, "Cities");
            SetCheckAll(CommentFlags, "Comments Flag");
        }

        #endregion

        #region Event handlers

        private void GroupOrganizationList_ValueChanged(object sender, EventArgs e) => OnGroupOrganizationListChanged();

        private void MembershipGroupID_ValueChanged(object sender, EventArgs e) => OnMembershipGroupChanged();

        private void EmployerParentGroupID_ValueChanged(object sender, EventArgs e) => OnEmployerParentGroupSelected();

        private void OnEmployerParentGroupSelected()
        {
            EmployerGroupID.Filter.ParentGroupIdentifier = EmployerParentGroupID.Value;
            EmployerGroupID.Value = null;
        }

        private void OnGroupOrganizationListChanged()
        {
            MembershipGroupID.Filter.OrganizationIdentifier = Organization.Identifier;
            MembershipGroupID.Value = null;

            ExcludeMembershipGroupID.Filter.OrganizationIdentifier = Organization.Identifier;
            ExcludeMembershipGroupID.Value = null;

            OnMembershipGroupChanged();
        }

        private void OnMembershipGroupChanged()
        {
            GroupMembershipDateField.Visible = MembershipGroupID.HasValue;
            GroupMembershipDate.Value = null;
        }

        #endregion

        #region Methods (helpers)

        public override void Clear()
        {
            FullName.Text = null;
            FirstName.Text = null;
            LastName.Text = null;
            NameFilterType.ClearSelection();

            Code.Text = null;
            CommentKeyword.Text = null;

            Gender.Value = null;

            Email.Text = null;
            EmailAlternate.Text = null;
            EmailEnabled.ClearSelection();
            EmailStatus.ClearSelection();
            EmailVerified.ClearSelection();

            EmployerParentGroupID.Value = null;
            OnEmployerParentGroupSelected();
            EmployerGroupID.Value = null;
            JobTitle.Text = null;

            AccountStatusId.ClearSelection();

            CommentFlags.ClearSelection();
            IsApproved.ClearSelection();
            IsArchived.ValueAsBoolean = false;
            IsMultiOrganization.ClearSelection();
            OrganizationRole.ClearSelection();

            ValidAchievementIdentifier.Value = null;

            AddressType.ClearSelection();
            Country.ClearSelection();
            Province.ClearSelection();
            City.ClearSelection();
            Phone.Text = null;
            Region.Value = null;

            MembershipGroupID.Value = null;
            OnMembershipGroupChanged();
            GroupMembershipDate.Value = null;
            ExcludeMembershipGroupID.Value = null;

            CreatedSince.Value = null;
            CreatedBefore.Value = null;
            ModifiedSince.Value = null;
            ModifiedBefore.Value = null;
            LastAuthenticatedSince.Value = null;
            LastAuthenticatedBefore.Value = null;
            Issues.Value = null;
        }

        public void SetDefaultShowColumnsAndShowResults()
        {
            SetDefaultShowColumns();
        }

        public void SetShowColumnsAndShowResults(PersonFilter filter)
        {
            SetCheckedShowColumns(filter);
        }

        #endregion
    }
}