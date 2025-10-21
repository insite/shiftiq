using System;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Jobs.Candidates.Controls
{
    public partial class CandidateCard : BaseUserControl
    {
        private DateTimeOffset? JobsApprovedOn
        {
            get => (DateTimeOffset?)ViewState[nameof(JobsApprovedOn)];
            set => ViewState[nameof(JobsApprovedOn)] = value;
        }

        private string JobsApprovedBy
        {
            get => (string)ViewState[nameof(JobsApprovedBy)];
            set => ViewState[nameof(JobsApprovedBy)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IsJobsApproved.AutoPostBack = true;
            IsJobsApproved.CheckedChanged += (s, a) =>
            {
                if (IsJobsApproved.Checked)
                {
                    JobsApprovedOn = TimeZones.ConvertFromUtc(DateTimeOffset.UtcNow, User.TimeZone);
                    JobsApprovedBy = User.Email;
                }
                else
                {
                    JobsApprovedOn = null;
                    JobsApprovedBy = null;
                }

                OnUserAccountApproved();
            };
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
            OccupationIdentifier.ListFilter.StandardTypes = new[] { StandardType.Profile };
            OccupationIdentifier.RefreshData();
        }

        private void OnUserAccountApproved()
        {
            var approvedByName = JobsApprovedBy.IsNotEmpty()
                ? ServiceLocator.ContactSearch.BindFirst(x => x.UserFullName, x => x.UserEmail == JobsApprovedBy).IfNullOrEmpty(JobsApprovedBy)
                : string.Empty;

            IsJobsApprovedDateTime.InnerHtml = JobsApprovedOn.Format(Identity.User.TimeZone, true, false) + " by " + approvedByName;
            IsJobsApprovedDateTime.Visible = JobsApprovedOn.HasValue;
        }

        public void SetInputValues(Person person, Address address)
        {
            var user = person.User;

            CandidateName.Text = user.FullName;
            CandidateName.NavigateUrl = $"/ui/admin/contacts/people/edit?contact={user.UserIdentifier}";

            CandidateEmail.Text = user.Email;
            CandidateEmail.NavigateUrl = $"mailto:{user.Email}";

            ConsentToShare.Value = person.ConsentToShare;
            IsActivelySeeking.ValueAsBoolean = person.CandidateIsActivelySeeking;
            AddressCity.Text = address?.City;
            IsWillingToRelocate.ValueAsBoolean = person.CandidateIsWillingToRelocate;
            LinkedInUrl.Text = person.CandidateLinkedInUrl;

            OccupationIdentifier.ValueAsGuid = person.OccupationStandardIdentifier;

            if (Organization.Identifier == Shift.Constant.OrganizationIdentifiers.BCPVPA)
                AccountStatusLabel.InnerText = $"{Organization.Code.ToUpper()} Membership Status";

            AccountStatusId.ValueAsGuid = person.MembershipStatusItemIdentifier;

            JobsAccessField.Visible = Organization.IsIndustrySpecialist();
            IsJobsApproved.Checked = person.JobsApproved.HasValue;
            JobsApprovedOn = person.JobsApproved;
            JobsApprovedBy = person.JobsApprovedBy;

            OnUserAccountApproved();
        }

        public void GetInputValues(QPerson person, QPersonAddress address)
        {
            person.CandidateIsActivelySeeking = IsActivelySeeking.ValueAsBoolean;
            address.City = AddressCity.Text;
            person.CandidateIsWillingToRelocate = IsWillingToRelocate.ValueAsBoolean;
            person.CandidateLinkedInUrl = LinkedInUrl.Text;

            person.OccupationStandardIdentifier = OccupationIdentifier.ValueAsGuid;
            person.MembershipStatusItemIdentifier = AccountStatusId.ValueAsGuid;

            person.JobsApproved = JobsApprovedOn;
            person.JobsApprovedBy = JobsApprovedBy;
        }
    }
}