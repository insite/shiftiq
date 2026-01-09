using System.Linq;
using System.Text;

using InSite.Application.Contacts.Read;
using InSite.Persistence;
using InSite.UI.Admin.Events.Classes.Controls;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class BCPVPAMembershipSummaryReport : System.Web.UI.UserControl
    {
        public void LoadReport(QPerson person)
        {
            var user = person.User;

            BCPVPAMemberNumber.Text = person.PersonCode.HasValue() ? person.PersonCode : "None";
            BCPVPAFullName.Text = user.FullName;
            BCPVPAHonorific.Text = user.Honorific.HasValue() ? user.Honorific + " " : "";

            BCPVPAEmail.Text = user.Email.HasValue() ? user.Email.ToLower() : null;
            BCPVPALoginName.Text = user.Email;
            BCPVPALoginName.Visible = !(BCPVPAEmail.Text == BCPVPALoginName.Text);

            if (person.HomeAddress != null)
                BCPVPAHomeAddress.Text = ClassVenueAddressInfo.GetAddress(person.HomeAddress);
            else
                BCPVPAHomeAddress.Text = "None";

            var phones = new StringBuilder();

            if (person.Phone.HasValue())
                phones.AppendLine($"<div>Preferred: {person.Phone}</div>");
            if (person.PhoneHome.HasValue())
                phones.AppendLine($"<div>Home: {person.PhoneHome}</div>");
            if (person.PhoneWork.HasValue())
                phones.AppendLine($"<div>Work: {person.PhoneWork}</div>");
            if (user.PhoneMobile.HasValue())
                phones.AppendLine($"<div>Cell: {user.PhoneMobile}</div>");
            if (person.PhoneOther.HasValue())
                phones.AppendLine($"<div>Other: {person.PhoneOther}</div>");

            if (phones.Length > 0)
                BCPVPAPhoneNumbers.Text = phones.ToString();
            else
                BCPVPAPhoneNumbers.Text = "None";

            if (person.EmployerGroup != null)
            {
                var pN = new StringBuilder();

                if (person.EmployerGroup.GroupPhone.HasValue())
                    pN.AppendLine($"<div>Phone: {person.EmployerGroup?.GroupPhone}</div>");
                if (person.EmployerGroup.GroupFax.HasValue())
                    pN.AppendLine($"<div>Fax: {person.EmployerGroup?.GroupFax}</div>");

                if (pN.Length > 0)
                    BCPVPASchoolPhoneNumbers.Text = pN.ToString();
                else
                    BCPVPASchoolPhoneNumbers.Text = "None";

                BCPVPASchoolNumber.Text = person.EmployerGroup.GroupCode.HasValue() ? "#" + person.EmployerGroup.GroupCode : "";
                BCPVPASchoolName.Text = person.EmployerGroup.GroupName.HasValue() ? person.EmployerGroup.GroupName : "None";

                var address = ServiceLocator.GroupSearch.GetAddress(person.EmployerGroupIdentifier.Value, AddressType.Shipping);
                if (address != null)
                    BCPVPASchoolAddress.Text = ClassVenueAddressInfo.GetAddress(address);
            }
            else
            {
                BCPVPASchoolName.Text = "None";
                BCPVPASchoolPhoneNumbers.Text = "None";
            }
            BCPVPASchoolDisctrict.Text = person.EmployerGroup?.Parent?.GroupName;
            BCPVPAShippingPreference.Text = person.ShippingPreference.HasValue() ? person.ShippingPreference : "None";

            var membershipStatus = TCollectionItemCache.GetName(person.MembershipStatusItemIdentifier);
            if (membershipStatus.IsNotEmpty())
            {
                BCPVPAMembershipStatus.Text = membershipStatus;
                if (person.MemberStartDate.HasValue)
                    BCPVPAMembershipStatus.Text = BCPVPAMembershipStatus.Text + $" from {person.MemberStartDate:MMM d, yyyy}";
                if (person.MemberEndDate.HasValue)
                    BCPVPAMembershipStatus.Text = BCPVPAMembershipStatus.Text + $" till {person.MemberEndDate:MMM d, yyyy}";
            }
            BCPVPAPosition.Text = person.JobTitle.HasValue() ? person.JobTitle : "None";

            BindRoles(user);
        }

        private void BindRoles(QUser user)
        {
            var roles = MembershipSearch
                .Select(x => x.UserIdentifier == user.UserIdentifier, x => x.Group)
                .Where(x => x.Group.GroupType == GroupTypes.Team && x.Group.GroupLabel == "Role")
                .OrderBy(x => x.Group.GroupType)
                .ThenBy(x => x.Group.GroupName)
                .ToList();

            ParticipationsRepeater.DataSource = roles;
            ParticipationsRepeater.DataBind();
        }
    }
}