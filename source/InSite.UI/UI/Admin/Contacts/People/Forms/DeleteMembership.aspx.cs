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
        private Guid? FromKey => Guid.TryParse(Request["from"], out var value) ? value : (Guid?)null;

        private Guid? ToKey => Guid.TryParse(Request["to"], out var value) ? value : (Guid?)null;

        private Guid? FromIdentifier
        {
            get => ViewState[nameof(FromIdentifier)] as Guid?;
            set => ViewState[nameof(FromIdentifier)] = value;
        }

        private Guid? ToIdentifier
        {
            get => ViewState[nameof(ToIdentifier)] as Guid?;
            set => ViewState[nameof(ToIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (FromKey == null || ToKey == null)
            {
                RedirectToSearch();

                return;
            }

            var membership = MembershipSearch.Select(FromKey.Value, ToKey.Value);
            if (membership != null)
            {
                MembershipStore.Delete(membership);

                if (membership.MembershipType == MembershipType.Employee)
                {
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
            }

            HttpResponseHelper.Redirect(GetReturnUrlInternal());
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (FromKey == null || ToKey == null)
            {
                RedirectToSearch();
                return;
            }

            var role = MembershipSearch.SelectFirst(x => x.GroupIdentifier == FromKey && x.UserIdentifier == ToKey, x => x.Group, x => x.User);
            if (role == null || role.Group.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect($"/ui/admin/contacts/people/search", true);

            if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains("/groups/"))
                FromIdentifier = role.Group.GroupIdentifier;
            else
                ToIdentifier = role.User.UserIdentifier;

            BuildBreadcrumbs(role);

            var _person = UserSearch.Select(ToKey.Value);
            FullName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={_person.UserIdentifier}\">{_person.FullName}</a>";

            AssignedOn.Text = role.Assigned.Format(User.TimeZone);
            RoleType.Text = role.MembershipType ?? "None";

            var group = ServiceLocator.GroupSearch.GetGroup(FromKey.Value);
            GroupName.Text = group.GroupName;
            GroupLink.HRef = $"/ui/admin/contacts/groups/edit?contact={group.GroupIdentifier}";

            var reasonCount = ServiceLocator.MembershipReasonSearch.Count(new QMembershipReasonFilter
            {
                MembershipIdentifier = role.MembershipIdentifier
            });

            ReasonCount.Text = reasonCount.ToString("n0");

            CancelButton.NavigateUrl = GetReturnUrlInternal();
        }

        private void BuildBreadcrumbs(Membership role)
        {
            BreadcrumbItem parentSearch, parentEditor;

            if (FromIdentifier.HasValue)
            {
                parentSearch = new BreadcrumbItem("Groups", "/ui/admin/contacts/groups/search");
                parentEditor = new BreadcrumbItem("Edit Group", $"/ui/admin/contacts/groups/edit?contact={FromIdentifier.Value}");
            }
            else
            {
                parentSearch = new BreadcrumbItem("People", "/ui/admin/contacts/people/search");
                parentEditor = new BreadcrumbItem("Edit Person", $"/ui/admin/contacts/people/edit?contact={ToIdentifier.Value}");
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

            return FromIdentifier.HasValue
                ? $"/ui/admin/contacts/groups/edit?contact={FromIdentifier}&panel=people"
                : $"/ui/admin/contacts/people/edit?contact={ToIdentifier}&panel=groups";
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect("/ui/admin/contacts/people/search");

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={ToIdentifier}"
                : null;
        }

    }
}