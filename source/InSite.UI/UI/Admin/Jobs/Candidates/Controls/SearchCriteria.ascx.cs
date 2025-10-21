using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Jobs.Candidates.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<PersonFilter>
    {
        protected override string[] DefaultShowColumns => new[] { "Name", "Email" };

        public override PersonFilter Filter
        {
            get
            {
                var filter = new PersonFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    GroupDepartmentIdentifiers = DepartmentGroupIdentifier.ValuesAsGuidArray,
                    FullName = Name.Text,
                    NameFilterType = NameFilterType.Value,
                    EmailContains = Email.Text,
                    AddressTypes = new[] { "Home" },
                    Cities = !string.IsNullOrEmpty(CityName.Text) ? new[] { CityName.Text } : null,
                    CandidateIsActivelySeeking = ActivelySeeking.ValueAsBoolean,
                    IsJobsApproved = Approved.ValueAsBoolean,
                    UtcCreatedSince = CreatedSince.Value?.UtcDateTime,
                    UtcCreatedBefore = CreatedBefore.Value?.UtcDateTime,
                    DayLastActive = DateLastActive.Visible ? DateLastActive.Value?.UtcDateTime : null,
                    CandidateOccupationKey = FindOccupationCode(OccupationalProfile.Value),
                    IsConsentToShare = true
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                DepartmentGroupIdentifier.ValuesAsGuid = value.GroupDepartmentIdentifiers;

                Name.Text = value.FullName;
                NameFilterType.Value = value.NameFilterType;
                Email.Text = value.EmailContains;
                CityName.Text = GetArrayValue(value.Cities);
                ActivelySeeking.ValueAsBoolean = value.CandidateIsActivelySeeking;
                Approved.ValueAsBoolean = value.IsJobsApproved;
                CreatedSince.Value = value.UtcCreatedSince;
                CreatedBefore.Value = value.UtcCreatedBefore;
                DateLastActive.Value = value.DayLastActive;
                OccupationalProfile.Value = FindOccupationId(value.CandidateOccupationKey);
            }
        }

        private static string GetArrayValue(string[] arr)
        {
            return arr != null && arr.Length > 0 ? arr[0] : null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentGroupIdentifier.ListFilter.OrganizationIdentifier = Organization.Identifier;
            DepartmentGroupIdentifier.ListFilter.GroupType = "Department";
            DepartmentGroupIdentifier.ClearSelection();

            if (!IsPostBack)
                BindOccupations();
        }

        public override void Clear()
        {
            DepartmentGroupIdentifier.ClearSelection();

            Name.Text = null;
            NameFilterType.Value = "Exact";
            Email.Text = null;
            CityName.Text = null;
            ActivelySeeking.ClearSelection();
            Approved.ClearSelection();

            CreatedSince.Value = null;
            CreatedBefore.Value = null;
            DateLastActive.Value = null;

            OccupationalProfile.Value = null;
        }

        private void BindOccupations()
        {
            OccupationalProfile.Filter.StandardTypes = new[] { StandardType.Profile };
            OccupationalProfile.Filter.StandardLabel = "Occupation";
        }

        private Guid? FindOccupationId(string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            var occupation = StandardSearch.SelectFirst(x =>
                x.OrganizationIdentifier == Organization.Identifier
                && x.StandardType == StandardType.Profile
                && x.StandardLabel == "Occupation"
                && x.Code == code
            );

            return occupation?.StandardIdentifier;
        }

        private string FindOccupationCode(Guid? id)
        {
            if (id == null)
                return null;

            var occupation = StandardSearch.SelectFirst(x =>
                x.OrganizationIdentifier == Organization.Identifier
                && x.StandardType == StandardType.Profile
                && x.StandardLabel == "Occupation"
                && x.StandardIdentifier == id
            );

            return occupation?.Code;
        }
    }
}