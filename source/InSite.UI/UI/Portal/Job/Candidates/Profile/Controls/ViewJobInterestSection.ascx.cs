using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class ViewJobInterestSection : BaseUserControl
    {

        public void BindModelToControls(Person person)
        {
            var isActivelySeeking = person.CandidateIsActivelySeeking == true;
            ActivelySeeking.Text = isActivelySeeking ? "Yes" : "No";
            HomeAddressCity.Text = person.HomeAddress?.City;
            if (HomeAddressCity.Text.HasNoValue())
                HomeAddressCity.Text = EmptyValue();

            RelocateInformation.Visible = true;

            string relocateResponse = "Not Sure";
            if (person.CandidateIsWillingToRelocate.HasValue)
                if (person.CandidateIsWillingToRelocate.Value)
                    relocateResponse = "Yes";
                else
                    relocateResponse = "No";
            IsWillingToRelocate.Text = relocateResponse;

            LinkedInUrl.Text = person.CandidateLinkedInUrl.HasValue() ? $"<i class='fab fa-fw fa-linkedin'></i>{person.User.FullName}" : EmptyValue();
            LinkedInUrl.NavigateUrl = person.CandidateLinkedInUrl.HasValue() ? person.CandidateLinkedInUrl : "#";

            var occupation = StandardSearch.BindFirst(x => x.ContentTitle, x => x.StandardIdentifier == person.OccupationStandardIdentifier);
            if (occupation != null)
                Occupation.Text = occupation.ToString();
            else
                Occupation.Text = EmptyValue();

            if (Organization.Identifier == Shift.Constant.OrganizationIdentifiers.BCPVPA)
                AccountStatusLabel.InnerText = $"{Organization.Code.ToUpper()} Membership Status";

            AccountStatus.Text = $"{person.MembershipStatusItemKey} {person.AccountStatusName}";
            if (AccountStatus.Text.HasNoValue())
                AccountStatus.Text = EmptyValue();

        }

        private string EmptyValue()
        {
            return "<p class=\"fw-light text-secondary\">Information not disclosed</p>";
        }
    }
}