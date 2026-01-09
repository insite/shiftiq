using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Contacts.Reports.Employees.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<EmployeeFilter>
    {
        public override EmployeeFilter Filter
        {
            get
            {
                var filter = new EmployeeFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    EmployerGroupIdentifier = EmployerGroupIdentifier.Value,

                    EmployeeName = EmployeeName.Text,
                    EmployeeEmail = EmployeeEmail.Text,
                    EmployeeJobTitle = EmployeeJobTitle.Text,
                    MembershipStatuses = MembershipStatus.ValuesArray,
                    EmployeeGender = EmployeeGender.Value,
                    EmployeeJoinedSince = EmployeeJoinedSince.Value?.UtcDateTime,
                    EmployeeJoinedBefore = EmployeeJoinedBefore.Value?.UtcDateTime,
                    EmployeeEndedSince = EmployeeEndedSince.Value?.UtcDateTime,
                    EmployeeEndedBefore = EmployeeEndedBefore.Value?.UtcDateTime,

                    EmployerNumber = EmployerNumber.Text,
                    EmployerDistrictIdentifier = EmployerDistrictIdentifier.Value,
                    MembershipGroupIdentifier = EmployeeGroupIdentifier.Value,
                };

                GetCheckedShowColumns(filter);

                filter.SortByColumn = SortColumns.Value;

                return filter;
            }
            set
            {
                EmployerGroupIdentifier.Value = value.EmployerGroupIdentifier;

                EmployeeName.Text = value.EmployeeName;
                EmployeeEmail.Text = value.EmployeeEmail;
                EmployeeJobTitle.Text = value.EmployeeJobTitle;
                MembershipStatus.Values = value.MembershipStatuses;
                EmployeeGender.Value = value.EmployeeGender;
                EmployeeJoinedSince.Value = value.EmployeeJoinedSince;
                EmployeeJoinedBefore.Value = value.EmployeeJoinedBefore;
                EmployeeEndedSince.Value = value.EmployeeEndedSince;
                EmployeeEndedBefore.Value = value.EmployeeEndedBefore;

                EmployerNumber.Text = value.EmployerNumber;
                EmployerDistrictIdentifier.Value = value.EmployerDistrictIdentifier;
                EmployeeGroupIdentifier.Value = value.MembershipGroupIdentifier;

                SortColumns.Value = value.SortByColumn;
            }
        }

        public override void Clear()
        {
            EmployerGroupIdentifier.Value = null;

            EmployeeName.Text = null;
            EmployeeEmail.Text = null;
            EmployeeJobTitle.Text = null;
            MembershipStatus.ClearSelection();
            EmployeeGender.ClearSelection();
            EmployeeJoinedSince.Value = null;
            EmployeeJoinedBefore.Value = null;
            EmployeeEndedSince.Value = null;
            EmployeeEndedBefore.Value = null;

            EmployerNumber.Text = null;
            EmployerDistrictIdentifier.Value = null;
            EmployeeGroupIdentifier.Value = null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MembershipStatus.Settings.CollectionName = CollectionName.Contacts_People_Membership_Status;
            MembershipStatus.Settings.OrganizationIdentifier = Organization.Key;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            EmployerGroupIdentifier.Filter.OrganizationIdentifier = Organization.Key;
            EmployerGroupIdentifier.Filter.IsEmployer = true;

            EmployerDistrictIdentifier.Filter.OrganizationIdentifier = Organization.Key;
            EmployerDistrictIdentifier.Filter.GroupType = GroupTypes.Employer;

            EmployeeGroupIdentifier.Filter.OrganizationIdentifier = Organization.Key;

            SetCheckAll(MembershipStatus, "Membership Status");
        }
    }
}