using System;
using System.Web.UI;

using InSite.Domain.Foundations;
using InSite.Persistence;

namespace InSite.UI.Desktops.Custom.TourismHR
{
    public partial class Join : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var group = RouteData.Values["group"];
            var redirect = Page.Request.QueryString["redirect"];
            var identity = CurrentSessionState.Identity;

            JoinGroup(identity, group as string);
            RedirectCaller(redirect);
        }

        private void JoinGroup(ISecurityFramework identity, string group)
        {
            if (identity.User == null || group == null)
                return;

            if (Guid.TryParse(group, out Guid g))
                AddUserToGroup(identity, g);
        }

        private void AddUserToGroup(ISecurityFramework identity, Guid group)
        {
            var userId = identity.User.UserIdentifier;
            var g = ServiceLocator.GroupSearch.GetGroup(group);

            var m = MembershipSearch.SelectFirst(x => x.User.UserIdentifier == userId && x.Group.GroupIdentifier == group);
            if (m != null || g.OrganizationIdentifier != identity.Organization.OrganizationIdentifier)
                return;

            if (!g.AllowJoinGroupUsingLink)
                Common.Web.HttpResponseHelper.SendHttp403();

            var membership = new Membership
            {
                Assigned = DateTimeOffset.UtcNow,
                UserIdentifier = userId,
                GroupIdentifier = g.GroupIdentifier
            };

            Web.Data.MembershipHelper.Save(membership, false, false);
            identity.Groups.Add(GroupAdapter.CreateGroupPacket(g));
        }

        private void RedirectCaller(string redirect)
        {
            var url = redirect;

            if (url == null && Request.UrlReferrer != null)
                if (Request.UrlReferrer.AbsolutePath != Request.Url.AbsolutePath)
                    url = Request.UrlReferrer.ToString();

            if (url == null)
                url = "/";

            Common.Web.HttpResponseHelper.Redirect(url);
        }
    }
}