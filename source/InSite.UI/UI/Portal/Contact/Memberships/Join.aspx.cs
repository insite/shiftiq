using System;
using System.Web.UI;

using InSite.Persistence;

namespace InSite.UI.Portal.Contact.Memberships
{
    public partial class Join : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var group = Page.Request.QueryString["group"];
            var redirect = Page.Request.QueryString["redirect"];
            var identity = CurrentSessionState.Identity;

            if (identity == null)
                return;

            if (identity.User != null)
            {
                var user = identity.User.UserIdentifier;

                TestOutput.Text = string.Format("Add user {0} to group {1} and redirect to {2}", user, group, redirect);

                Guid g;
                if (Guid.TryParse(group, out g))
                    AddUserToGroup(user, g);
            }

            if (redirect != null)
                Common.Web.HttpResponseHelper.Redirect(redirect);
        }

        public static void AddUserToGroup(Guid user, Guid group)
        {
            var g = ServiceLocator.GroupSearch.GetGroup(group);
            var m = MembershipSearch.SelectFirst(x => x.User.UserIdentifier == user && x.Group.GroupIdentifier == group);

            if (m == null && g.OrganizationIdentifier == CurrentSessionState.Identity.Organization.OrganizationIdentifier)
            {
                if (!g.AllowJoinGroupUsingLink)
                    Common.Web.HttpResponseHelper.SendHttp403();

                m = new Membership
                {
                    Assigned = DateTimeOffset.UtcNow,
                    UserIdentifier = user,
                    GroupIdentifier = g.GroupIdentifier
                };

                Web.Data.MembershipHelper.Save(m);
                CurrentSessionState.Identity.Groups.Add(GroupAdapter.CreateGroupPacket(g));
            }
        }
    }
}