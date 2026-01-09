using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class EditMembership : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private const string EditUrl = "/ui/admin/contacts/people/edit";
        private const string SearchUrl = "/ui/admin/contacts/people/search";
        private string ReturnUrl => !string.IsNullOrEmpty(Request.QueryString["returnURL"]) ? Request.QueryString["returnURL"] : "/";

        private Guid? GroupIdentifier => Guid.TryParse(Request["from"], out var value) ? value : (Guid?)null;

        private Guid? UserIdentifier => Guid.TryParse(Request["to"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            var membership = MembershipSearch.SelectFirst(
                x => x.GroupIdentifier == GroupIdentifier && x.UserIdentifier == UserIdentifier,
                x => x.Group);

            if (membership == null || membership.Group.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier.Value, Organization.Key, x => x.User);

            PageHelper.AutoBindHeader(Page, qualifier: person.User.FullName);

            GroupDetail.BindGroup(membership.Group);
            PersonInfo.BindPerson(person, User.TimeZone);

            GroupType.Text = membership.Group.GroupLabel ?? membership.Group.GroupType;
            PersonName1.Text = PersonName2.Text = person.User.FirstName;

            AssignedOn.Value = membership.Assigned;
            MembershipFunction.Value = membership.MembershipType;
            MembershipExpiry.Value = membership.MembershipExpiry;

            CancelButton.NavigateUrl = GetReturnUrl();
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var membership = MembershipSearch.Select(GroupIdentifier.Value, UserIdentifier.Value);

            if (AssignedOn.Value != null)
                membership.Assigned = AssignedOn.Value.Value;

            membership.MembershipType = MembershipFunction.Value;
            membership.MembershipExpiry = MembershipExpiry.Value;

            MembershipHelper.Save(membership, true, true, true);

            HttpResponseHelper.Redirect(GetReturnUrl());
        }

        protected override string GetReturnUrl()
        {
            var url = ReturnUrl;
            if (!(string.IsNullOrEmpty(url) || url == "/"))
                return url;

            return EditUrl + $"?contact={UserIdentifier}&panel=groups";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent != null && string.Equals(parent.Name, GetParentAction(), StringComparison.OrdinalIgnoreCase)
                ? GetParentActionParameters()
                : null;
        }

        private string GetParentActionParameters()
        {
            if (ReturnUrl == "/" || !ReturnUrl.StartsWith("/"))
                return null;

            var index = ReturnUrl.IndexOf('?');

            return index > 0 && index < ReturnUrl.Length
                ? ReturnUrl.Substring(index + 1)
                : null;
        }

        public new IWebRoute GetParent()
        {
            var parentAction = GetParentAction();

            return !string.IsNullOrEmpty(parentAction)
                ? WebRoute.GetWebRoute(parentAction)
                : null;
        }

        private string GetParentAction()
        {
            if (ReturnUrl == "/" || !ReturnUrl.StartsWith("/"))
                return null;

            var index = ReturnUrl.IndexOf('?');

            return index > 0
                ? ReturnUrl.Substring(1, index - 1)
                : ReturnUrl.Substring(1);
        }
    }
}