using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class EditMembership : AdminBasePage, IHasParentLinkParameters
    {
        private const string EditUrl = "/ui/admin/contacts/people/edit";
        private const string SearchUrl = "/ui/admin/contacts/people/search";

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
            var url = base.GetReturnUrl();
            if (!string.IsNullOrEmpty(url))
                return url;

            return EditUrl + $"?contact={UserIdentifier}&panel=groups";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={UserIdentifier}"
                : null;
        }
    }
}