using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CaseLawyerComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            var users = UserSearch.Bind(
                x => new { x.UserIdentifier, x.FullName },
                new UserFilter
                {
                    MembershipGroupName = "Issue Lawyers",
                    PersonOrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier
                },
                "FullName");

            foreach (var user in users)
                list.Add(user.UserIdentifier.ToString(), user.FullName);

            return list;
        }
    }
}