using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Cmds.Controls.User
{
    public partial class PendingApprovalUserGrid : BaseUserControl
    {
        private class PendingApprovalUser
        {
            public class Group
            {
                public string OrganizationCode { get; set; }
                public string GroupType { get; set; }
                public string Name { get; set; }
            }

            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public Group[] Groups { get; set; }
        }

        public void LoadData()
        {
            var roles = MembershipSearch.Select(
                x => x.User.AccessGrantedToCmds
                  && x.User.UtcArchived == null
                  && x.User.Persons.Any(y => y.OrganizationIdentifier == x.Group.OrganizationIdentifier && y.UserAccessGranted == null),
                x => x.User,
                x => x.Group,
                x => x.Group.Organization
            );

            var users = roles.GroupBy(x => x.User.UserIdentifier)
                .Select(x => new PendingApprovalUser
                {
                    UserIdentifier = x.Key,
                    FullName = x.First().User.FullName,
                    Email = x.First().User.Email,
                    Groups = x.Select(y => new PendingApprovalUser.Group
                    {
                        OrganizationCode = y.Group.Organization.OrganizationCode,
                        GroupType = y.Group.GroupType,
                        Name = y.Group.GroupName
                    }).ToArray()
                })
                .ToList();

            var nonBlogSubscribers = users
                .Where(x => x.Groups.Length != 1 || x.Groups[0].Name != "CMDS Blog Subscribers")
                .OrderBy(x => x.FullName)
                .ToList();

            var hasData = nonBlogSubscribers.Count > 0;

            Repeater.Visible = hasData;
            NoDataMessage.Visible = !hasData;

            Repeater.DataSource = nonBlogSubscribers;
            Repeater.DataBind();
        }

        protected static string GetGroupList(object o)
        {
            var user = (PendingApprovalUser)o;

            var groups = user.Groups
                .Where(x =>
                    string.Equals(x.OrganizationCode, "cmds", StringComparison.OrdinalIgnoreCase)
                    && x.GroupType == GroupTypes.Role
                    && (x.Name.StartsWith("CMDS") || x.Name.StartsWith("Skills Passport"))
                )
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToList();

            return string.Join(", ", groups);
        }
    }
}