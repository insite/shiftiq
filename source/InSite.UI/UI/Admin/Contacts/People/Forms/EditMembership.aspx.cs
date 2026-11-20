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

        private Guid MembershipId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        private Guid? UserId
        {
            get => (Guid?)ViewState[nameof(UserId)];
            set => ViewState[nameof(UserId)] = value;
        }

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

            var entity = MembershipSearch.SelectFirst(x => x.MembershipIdentifier == MembershipId, x => x.Group);
            if (entity == null || entity.Group.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            UserId = entity.UserIdentifier;

            var user = ServiceLocator.UserSearch.GetUser(entity.UserIdentifier);
            var person = ServiceLocator.PersonSearch.GetPerson(entity.UserIdentifier, Organization.OrganizationIdentifier);

            PageHelper.AutoBindHeader(Page, qualifier: user.FullName);

            GroupDetail.BindGroup(entity.Group);
            PersonInfo.BindPerson(person, user, User.TimeZone);

            GroupType.Text = entity.Group.GroupLabel ?? entity.Group.GroupType;
            PersonName1.Text = PersonName2.Text = user.FirstName;

            AssignedOn.Value = entity.Assigned;
            MembershipFunction.Value = entity.MembershipType;
            MembershipExpiry.Value = entity.MembershipExpiry;

            CancelButton.NavigateUrl = GetReturnUrl();
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var entity = MembershipSearch.SelectFirst(x => x.MembershipIdentifier == MembershipId, x => x.Group);
            if (entity != null && entity.UserIdentifier == UserId)
            {
                if (AssignedOn.Value != null)
                    entity.Assigned = AssignedOn.Value.Value;

                entity.MembershipType = MembershipFunction.Value;
                entity.MembershipExpiry = MembershipExpiry.Value;

                MembershipHelper.Save(entity, true, true, true);
            }

            HttpResponseHelper.Redirect(GetReturnUrl());
        }

        protected override string GetReturnUrl()
        {
            var url = ReturnUrl;
            if (!(string.IsNullOrEmpty(url) || url == "/"))
                return url;

            return EditUrl + $"?contact={UserId}&panel=groups";
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
