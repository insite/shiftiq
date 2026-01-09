using System;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.Groups.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QGroupFilter>
    {
        private static readonly string[] _defaultShowColumns = new string[]
        {
            "GROUP NAME",
            "GROUP TAG",
            "GROUP CATEGORY",
            "GROUP STATUS",
            "GROUP REGION",
            "PHYSICAL ADDRESS",
            "GROUP SIZE",
        };

        public string DefaultGroupType => Request.QueryString["type"];
        public string DefaultGroupLabel => Request.QueryString["label"];

        protected override string[] DefaultShowColumns => _defaultShowColumns;

        public override QGroupFilter Filter
        {
            get
            {
                var filter = new QGroupFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    OrganizationIdentifiers = CurrentSessionState.Identity.Organizations.Select(x => x.Identifier).ToArray(),

                    GroupNameLike = Name.Text,
                    GroupType = !IsPostBack && DefaultGroupType.IsNotEmpty() ? DefaultGroupType : GroupType.Value,
                    GroupLabel = !IsPostBack && DefaultGroupLabel.IsNotEmpty() ? DefaultGroupLabel : GroupLabel.Value,
                    GroupCode = GroupCode.Text,
                    GroupCategory = GroupCategory.Text,
                    GroupStatusIdentifier = GroupStatusId.ValueAsGuid,

                    GroupRegion = GroupRegion.Value,
                    AddressTypes = AddressType.ValuesArray,
                    Country = Country.Value,
                    Provinces = Province.ValuesArray,
                    Cities = City.ValuesArray,

                    UtcCreatedSince = UtcCreatedSince.Value?.UtcDateTime,
                    UtcCreatedBefore = UtcCreatedBefore.Value?.UtcDateTime,

                    GroupExpirySince = GroupExpirySince.Value?.UtcDateTime,
                    GroupExpiryBefore = GroupExpiryBefore.Value?.UtcDateTime,

                    AnyParentGroupIdentifier = ParentGroupIdentifier.Value,
                    Statuses = MembershipStatusItemId.ValuesAsGuid != null ? MembershipStatusItemId.ValuesAsGuid.ToArray() : null, 

                    SurveyFormIdentifier = SurveyFormIdentifier.Value,
                    MembershipProductIdentifier = MembershipProductIdentifier.Value
                };

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                Name.Text = value.GroupNameLike;
                GroupType.Value = !IsPostBack && DefaultGroupType.IsNotEmpty() ? DefaultGroupType : value.GroupType;
                OnGroupTypeSelected();
                GroupLabel.Value = !IsPostBack && DefaultGroupLabel.IsNotEmpty() ? DefaultGroupLabel : value.GroupLabel;
                GroupCode.Text = value.GroupCode;
                GroupCategory.Text = value.GroupCategory;
                GroupStatusId.ValueAsGuid = value.GroupStatusIdentifier;

                GroupRegion.Value = value.GroupRegion;
                AddressType.Values = value.AddressTypes;
                Country.Value = value.Country;
                Province.Values = value.Provinces;
                City.Values = value.Cities;

                UtcCreatedSince.Value = value.UtcCreatedSince;
                UtcCreatedBefore.Value = value.UtcCreatedBefore;

                GroupExpirySince.Value = value.GroupExpirySince;
                GroupExpiryBefore.Value = value.GroupExpiryBefore;

                ParentGroupIdentifier.Value = value.AnyParentGroupIdentifier;
                MembershipStatusItemId.ValuesAsGuid = value.Statuses;

                SurveyFormIdentifier.Value = value.SurveyFormIdentifier;
                MembershipProductIdentifier.Value = value.MembershipProductIdentifier;

                SortColumns.Value = value.OrderBy;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            OnGroupTypeSelected();

            ParentGroupIdentifier.Filter.OrganizationIdentifier = Organization.Key;

            SetCheckAll(MembershipStatusItemId, "Membership Status");
            SetCheckAll(AddressType, "Address Types");
            SetCheckAll(Province, "Provinces");
            SetCheckAll(City, "Cities");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupType.AutoPostBack = true;
            GroupType.ValueChanged += GroupType_ValueChanged;

            MembershipStatusItemId.Settings.CollectionName = CollectionName.Contacts_People_Membership_Status;
            MembershipStatusItemId.Settings.OrganizationIdentifier = Organization.Key;

            GroupRegion.Settings.CollectionName = CollectionName.Contacts_People_Location_Region;
            GroupRegion.Settings.OrganizationIdentifier = Organization.Key;

            GroupStatusId.ListFilter.OrganizationIdentifier = Organization.Key;
            GroupStatusId.ListFilter.CollectionName = CollectionName.Contacts_Groups_Status_Name;
        }

        private void GroupType_ValueChanged(object sender, EventArgs e)
        {
            OnGroupTypeSelected();
        }

        private void OnGroupTypeSelected()
        {
            GroupLabel.GroupType = GroupType.Value.NullIfEmpty();
            GroupLabel.RefreshData();
        }

        public override void Clear()
        {
            Name.Text = null;
            GroupType.Value = DefaultGroupType;
            OnGroupTypeSelected();
            GroupLabel.Value = DefaultGroupLabel;
            GroupCode.Text = null;
            GroupStatusId.ClearSelection();

            GroupRegion.Value = null;
            AddressType.ClearSelection();
            Country.ClearSelection();
            Province.ClearSelection();
            City.ClearSelection();

            UtcCreatedSince.Value = null;
            UtcCreatedBefore.Value = null;
            GroupExpirySince.Value = null;
            GroupExpiryBefore.Value = null;
            ParentGroupIdentifier.Value = null;
            MembershipStatusItemId.ClearSelection();

            SurveyFormIdentifier.Value = null;
            MembershipProductIdentifier.Value = null;
        }
    }
}