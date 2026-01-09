using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using InSite.Application.Contacts.Read;

using Shift.Common;

using AspLiteral = System.Web.UI.WebControls.Literal;

namespace InSite.UI.Admin.Contacts.People.Controls
{
    public partial class PersonInfo : UserControl
    {
        public void BindPerson(QPerson person, TimeZoneInfo tz, string customPersonUrl = null) =>
            BindPerson(person, person?.User, tz, customPersonUrl);

        public void BindPerson(QPerson person, QUser user, TimeZoneInfo tz, string customPersonUrl = null)
        {
            BindPerson(person, user, tz,
                UserName, UserLink,
                Email, ContactCode, Birthdate,
                Employer);

            if (customPersonUrl.IsNotEmpty())
                UserLink.HRef = customPersonUrl;
        }

        public static void BindPerson(QPerson person, QUser user, TimeZoneInfo tz,
            AspLiteral userName = null, HtmlAnchor userLink = null,
            AspLiteral email = null, AspLiteral personCode = null, AspLiteral birthdate = null,
            AspLiteral employer = null)
        {
            if (userName != null)
            {
                userName.Text = user?.FullName;
                userName.Visible = person == null || userLink == null;
            }

            if (userLink != null)
            {
                userLink.InnerText = user?.FullName;
                userLink.HRef = $"/ui/admin/contacts/people/edit?contact={user?.UserIdentifier}";
                userLink.Visible = person != null;
            }

            if (email != null)
                email.Text = $"<a href='mailto:{user?.Email}'>{user?.Email}</a>";

            if (personCode != null)
                personCode.Text = person?.PersonCode ?? "None";

            birthdate.Text = person?.Birthdate != null ? GetLocalTime(person.Birthdate, tz) : "N/A";

            if (employer != null)
            {
                var employerGroup = person?.EmployerGroupIdentifier != null
                    ? ServiceLocator.GroupSearch.GetGroup(person.EmployerGroupIdentifier.Value)
                    : null;

                employer.Text = employerGroup != null
                    ? $"<a href=\"/ui/admin/contacts/groups/edit?contact={employerGroup.GroupIdentifier}\">{employerGroup.GroupName}</a>"
                    : "None";
            }
        }

        private static string GetLocalTime(DateTimeOffset? item, TimeZoneInfo tz)
        {
            return item.FormatDateOnly(tz, nullValue: "None");
        }
    }
}