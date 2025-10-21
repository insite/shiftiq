using System;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class JobInterestSection : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AccountStatusId.Settings.CollectionName = CollectionName.Contacts_People_Membership_Status;
            AccountStatusId.Settings.OrganizationIdentifier = Organization.Key;
            AccountStatusId.EmptyMessage = $"{Organization.Code.ToUpper()} Membership Status";
            AccountStatusId.RefreshData();
        }

        public void BindControlsToModel(QPerson person, QPersonAddress homeAddress)
        {
            person.ConsentToShare = ConsentToShare.Value;

            if (ActivelySeekingYes.Checked)
                person.CandidateIsActivelySeeking = true;
            else if (ActivelySeekingNo.Checked)
                person.CandidateIsActivelySeeking = false;

            homeAddress.City = HomeAddressCity.Text;

            person.CandidateLinkedInUrl = LinkedInUrl.Text;
            person.OccupationStandardIdentifier = OccupationIdentifier.ValueAsGuid;
            person.MembershipStatusItemIdentifier = AccountStatusId.ValueAsGuid;
        }

        public void BindModelToControls(Person person)
        {
            ConsentToShare.Value = person.ConsentToShare;

            var isActivelySeeking = person.CandidateIsActivelySeeking == true;
            ActivelySeekingYes.Checked = isActivelySeeking;
            ActivelySeekingNo.Checked = !isActivelySeeking;

            HomeAddressCity.Text = person.HomeAddress?.City;

            RelocateInformation.Visible = true;

            IsWillingToRelocateYes.Checked = person.CandidateIsWillingToRelocate == true;
            IsWillingToRelocateNo.Checked = person.CandidateIsWillingToRelocate == false;
            IsWillingToRelocateUnsure.Checked = person.CandidateIsWillingToRelocate == null;

            LinkedInUrl.Text = person.CandidateLinkedInUrl;

            OccupationIdentifier.ListFilter.OrganizationIdentifier = Organization.Identifier;
            OccupationIdentifier.ListFilter.StandardTypes = new[] { StandardType.Profile };
            OccupationIdentifier.EnsureDataBound();
            OccupationIdentifier.ValueAsGuid = person.OccupationStandardIdentifier;

            if (Organization.Identifier == Shift.Constant.OrganizationIdentifiers.BCPVPA)
                AccountStatusLabel.InnerText = $"{Organization.Code.ToUpper()} Membership Status";

            AccountStatusId.ValueAsGuid = person.MembershipStatusItemIdentifier;
        }
    }
}