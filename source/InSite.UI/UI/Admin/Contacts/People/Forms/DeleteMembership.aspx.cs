using System;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class DeleteMembership : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private enum ParentType
        {
            Group, Person
        }

        [Serializable]
        private class ScreenData
        {
            public Guid MembershipId { get; }
            public Guid GroupId { get; }
            public Guid UserId { get; }
            public ParentType Parent { get; }

            public ScreenData(Membership entity, ParentType parent)
            {
                MembershipId = entity.MembershipIdentifier;
                GroupId = entity.GroupIdentifier;
                UserId = entity.UserIdentifier;
                Parent = parent;
            }
        }

        #endregion

        private Guid MembershipId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        private ScreenData Data
        {
            get => (ScreenData)ViewState[nameof(Data)];
            set => ViewState[nameof(Data)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Data != null)
                Delete(Data.MembershipId);

            HttpResponseHelper.Redirect(GetReturnUrlInternal());
        }

        private void Delete(Guid membershipId)
        {
            var membership = MembershipSearch.SelectFirst(x => x.MembershipIdentifier == membershipId);
            if (membership == null || membership.UserIdentifier != Data.UserId || membership.GroupIdentifier != Data.GroupId)
                return;

            MembershipStore.Delete(membership.MembershipIdentifier);

            if (membership.MembershipType != MembershipType.Employee)
                return;

            var persons = ServiceLocator.PersonSearch.GetPersons(new QPersonFilter
            {
                EmployerGroupIdentifier = membership.GroupIdentifier,
                UserIdentifier = membership.UserIdentifier
            });

            foreach (var person in persons)
            {
                person.EmployerGroupIdentifier = null;
                PersonStore.Update(person);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var entity = MembershipSearch.SelectFirst(x => x.MembershipIdentifier == MembershipId, x => x.Group, x => x.User);
            if (entity == null || entity.Group.OrganizationIdentifier != Organization.OrganizationIdentifier)
                RedirectToSearch();

            var parent = Request.UrlReferrer?.ToString().Contains("/groups/") == true ? ParentType.Group : ParentType.Person;
            var screenData = Data = new ScreenData(entity, parent);

            BuildBreadcrumbs(entity);

            var _person = UserSearch.Select(screenData.UserId);
            FullName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={_person.UserIdentifier}\">{_person.FullName}</a>";

            AssignedOn.Text = entity.Assigned.Format(User.TimeZone);
            RoleType.Text = entity.MembershipType ?? "None";

            var group = ServiceLocator.GroupSearch.GetGroup(screenData.GroupId);
            GroupName.Text = group.GroupName;
            GroupLink.HRef = $"/ui/admin/contacts/groups/edit?contact={group.GroupIdentifier}";

            var reasonCount = ServiceLocator.MembershipReasonSearch.Count(new QMembershipReasonFilter
            {
                MembershipIdentifier = entity.MembershipIdentifier
            });

            ReasonCount.Text = reasonCount.ToString("n0");

            CancelButton.NavigateUrl = GetReturnUrlInternal();
        }

        private void BuildBreadcrumbs(Membership role)
        {
            BreadcrumbItem parentSearch, parentEditor;

            if (Data.Parent == ParentType.Group)
            {
                parentSearch = new BreadcrumbItem("Groups", "/ui/admin/contacts/groups/search");
                parentEditor = new BreadcrumbItem("Edit Group", $"/ui/admin/contacts/groups/edit?contact={Data.GroupId}");
            }
            else
            {
                parentSearch = new BreadcrumbItem("People", "/ui/admin/contacts/people/search");
                parentEditor = new BreadcrumbItem("Edit Person", $"/ui/admin/contacts/people/edit?contact={Data.UserId}");
            }

            var title = $"<span class='form-text'>Assignment from </span>{role.Group.GroupName}<span class='form-text'> group to </span>{role.User.FullName}<span class='form-text'> user</span>";

            PageHelper.BindHeader(this, new BreadcrumbItem[]
            {
                new BreadcrumbItem("Contacts", "/ui/admin/contacts/home"),
                parentSearch,
                parentEditor,
                new BreadcrumbItem("Delete Membership", null, null, "active"),
            }, null, title);
        }

        private string GetReturnUrlInternal()
        {
            var url = GetReturnUrl();
            if (!string.IsNullOrEmpty(url))
                return url;

            return Data.Parent == ParentType.Group
                ? $"/ui/admin/contacts/groups/edit?contact={Data.GroupId}&panel=people"
                : $"/ui/admin/contacts/people/edit?contact={Data.UserId}&panel=groups";
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect("/ui/admin/contacts/people/search");

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={Data.UserId}"
                : null;
        }

    }
}
