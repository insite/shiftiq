using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Contacts.MembershipReasons.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QMembershipReasonFilter>
    {
        public override QMembershipReasonFilter Filter
        {
            get
            {
                var filter = new QMembershipReasonFilter
                {
                    GroupOrganizationIdentifiers = new[] { Organization.OrganizationIdentifier },
                    GroupIdentifier = MembershipGroup.Value,
                    UserIdentifier = MembershipUser.Value,
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    ReasonType = ReasonType.Value,
                    ReasonSubtype = ReasonSubtype.Text,
                    ReasonEffectiveSince = ReasonEffectiveSince.Value,
                    ReasonEffectiveBefore = ReasonEffectiveBefore.Value,
                    ReasonExpirySince = ReasonExpirySince.Value,
                    ReasonExpiryBefore = ReasonExpiryBefore.Value,
                    PersonOccupation = PersonOccupation.Text,
                    ReasonCreatedBy = ReasonCreatedBy.Value,
                    PersonCode = PersonCode.Text,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                MembershipGroup.Value = value.GroupIdentifier;
                MembershipUser.Value = value.UserIdentifier;
                ReasonType.Value = value.ReasonType;
                ReasonSubtype.Text = value.ReasonSubtype;
                ReasonEffectiveSince.Value = value.ReasonEffectiveSince;
                ReasonEffectiveBefore.Value = value.ReasonEffectiveBefore;
                ReasonExpirySince.Value = value.ReasonExpirySince;
                ReasonExpiryBefore.Value = value.ReasonExpiryBefore;
                PersonOccupation.Text = value.PersonOccupation;
                ReasonCreatedBy.Value = value.ReasonCreatedBy;
                PersonCode.Text = value.PersonCode;
            }
        }

        public override void Clear()
        {
            MembershipGroup.Value = null;
            MembershipUser.Value = null;
            ReasonType.Value = null;
            ReasonSubtype.Text = null;
            ReasonEffectiveSince.Value = null;
            ReasonEffectiveBefore.Value = null;
            ReasonExpirySince.Value = null;
            ReasonExpiryBefore.Value = null;
            PersonOccupation.Text = null;
            ReasonCreatedBy.Value = null;
            PersonCode.Text = null;
        }
    }
}