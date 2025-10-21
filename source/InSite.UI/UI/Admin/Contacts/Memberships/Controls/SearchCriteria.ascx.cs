using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Contacts.Memberships.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<MembershipFilter>
    {
        public override MembershipFilter Filter
        {
            get
            {
                var filter = new MembershipFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    GroupType = GroupType.Value,
                    GroupLabel = GroupLabel.Text,
                    GroupName = GroupName.Text,
                    UserFullName = UserFullName.Text,
                    UserCode = UserCode.Text,
                    UserEmail = UserEmail.Text,
                    MembershipStatuses = MembershipStatusItemKey.ValuesAsIntArray,
                    MembershipFunctions = MembershipFunction.ValuesArray,
                    HasMembershipFunction = MembershipFunctionStatus.ValueAsBoolean,
                    EffectiveSince = EffectiveSince.Value,
                    EffectiveBefore = EffectiveBefore.Value,
                    ExpirySince = ExpirySince.Value,
                    ExpiryBefore = ExpiryBefore.Value,
                };

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                GroupType.Value = value.GroupType;
                GroupLabel.Text = value.GroupLabel;
                GroupName.Text = value.GroupName;
                UserFullName.Text = value.UserFullName;
                UserCode.Text = value.UserCode;
                UserEmail.Text = value.UserEmail;
                MembershipStatusItemKey.ValuesAsInt = value.MembershipStatuses;
                MembershipFunction.Values = value.MembershipFunctions;
                MembershipFunctionStatus.ValueAsBoolean = value.HasMembershipFunction;

                EffectiveSince.Value = value.EffectiveSince;
                EffectiveBefore.Value = value.EffectiveBefore;
                ExpirySince.Value = value.ExpirySince;
                ExpiryBefore.Value = value.ExpiryBefore;

                SortColumns.Value = value.OrderBy;
            }
        }

        public override void Clear()
        {
            GroupType.ClearSelection();
            GroupLabel.Text = null;
            GroupName.Text = null;
            UserFullName.Text = null;
            UserCode.Text = null;
            UserEmail.Text = null;
            MembershipFunction.ClearSelection();
            MembershipFunctionStatus.ClearSelection();
            MembershipStatusItemKey.ClearSelection();

            EffectiveSince.Value = null;
            EffectiveBefore.Value = null;
            ExpirySince.Value = null;
            ExpiryBefore.Value = null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MembershipStatusItemKey.Settings.CollectionName = CollectionName.Contacts_People_Membership_Status;
            MembershipStatusItemKey.Settings.OrganizationIdentifier = Organization.Key;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                SetCheckAll(MembershipStatusItemKey, "Membership Status");
            }
        }
    }
}