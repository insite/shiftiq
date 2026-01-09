using System.Text;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class PersonDetail : BaseUserControl
    {
        private const string None = "None";

        public void SetModel(QUser user, QPerson person)
        {
            PersonCode.Text = person.PersonCode.IfNullOrEmpty(None);
            FullName.Text = person.FullName.IfNullOrEmpty(user.FullName);
            Email.Text = user.Email;
            Birthdate.Text = person.Birthdate.HasValue ? $"{person.Birthdate:MMM d, yyyy}" : None;
            Gender.Text = person.Gender.IfNullOrEmpty(None);
            EmergencyContact.Text = GetEmergencyContact(person);

            Phone.Text = person.Phone.IfNullOrEmpty("None");
            PhoneHome.Text = person.PhoneHome.IfNullOrEmpty("None");
            PhoneWork.Text = person.PhoneWork.IfNullOrEmpty("None");
            PhoneMobile.Text = person.User.PhoneMobile.IfNullOrEmpty("None");
            PhoneOther.Text = person.PhoneOther.IfNullOrEmpty("None");
        }

        private static string GetEmergencyContact(QPerson person)
        {
            var result = new StringBuilder();

            if (!string.IsNullOrEmpty(person.EmergencyContactName))
                result.Append(person.EmergencyContactName);

            if (!string.IsNullOrEmpty(person.EmergencyContactPhone))
            {
                if (result.Length > 0)
                    result.AppendLine("<br>");

                result.AppendLine(person.EmergencyContactPhone);
            }

            if (result.Length == 0)
                return None;

            if (!string.IsNullOrEmpty(person.EmergencyContactRelationship))
            {
                result.AppendLine("<br>");
                result.AppendLine(person.EmergencyContactRelationship);
            }

            return result.ToString();
        }
    }
}