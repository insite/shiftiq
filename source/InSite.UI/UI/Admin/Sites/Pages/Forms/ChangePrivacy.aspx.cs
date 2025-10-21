using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

using IHasParentLinkParameters = Shift.Common.IHasParentLinkParameters;
using IWebRoute = Shift.Common.IWebRoute;

namespace InSite.Admin.Sites.Pages
{
    public partial class ChangePrivacy : AdminBasePage, IHasParentLinkParameters
    {
        private Guid PageId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterGroupType.AutoPostBack = true;
            FilterGroupType.ValueChanged += (x, y) => ApplyFilter();
            FilterGroupButton.Click += (x, y) => ApplyFilter();

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var page = ServiceLocator.PageSearch.GetPage(PageId);

                if (page == null || page.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/sites/pages/search");
                    return;
                }

                PageHelper.AutoBindHeader(this, null, page.Title);

                PageDetails.BindPage(page);

                if (!Page.IsPostBack)
                {
                    FilterGroupType.Value = GroupTypes.Role;
                    ApplyFilter();
                }

                CancelButton.NavigateUrl = $"/ui/admin/sites/pages/outline?id={PageId}&panel=setup&tab=privacy";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            SaveGroups();

            HttpResponseHelper.Redirect($"/ui/admin/sites/pages/outline?id={PageId}&panel=setup&tab=privacy");
        }

        public void SaveGroups()
        {
            var grants = new List<Guid>();
            var revokes = new List<Guid>();

            foreach (ListItem item in FilterGroupList.Items)
            {
                var group = Guid.Parse(item.Value);
                if (item.Selected)
                    grants.Add(group);
                else
                    revokes.Add(group);
            }

            TGroupPermissionStore.Update(DateTimeOffset.UtcNow, User.UserIdentifier, PageId, "Web Page", grants, revokes);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={PageId}&panel=setup&tab=privacy"
                : null;
        }

        private void ApplyFilter()
        {
            var filterGroupName = FilterGroupName.Text;

            var organizations = new List<Guid> { Organization.Identifier };
            if (Organization.ParentOrganizationIdentifier.HasValue)
                organizations.Add(Organization.ParentOrganizationIdentifier.Value);

            var filter = new QGroupFilter
            {
                OrganizationIdentifiers = organizations.ToArray(),
                GroupType = FilterGroupType.Value,
                GroupNameLike = filterGroupName
            };

            var groups = ServiceLocator.GroupSearch.GetGroups(filter);

            FilterGroupList.Items.Clear();
            foreach (var group in groups)
            {
                var isExists = TGroupPermissionSearch.Exists(x => x.GroupIdentifier == group.GroupIdentifier && x.ObjectIdentifier == PageId);

                FilterGroupList.Items.Add(new ListItem
                {
                    Value = group.GroupIdentifier.ToString(),
                    Text = group.GroupName,
                    Selected = isExists
                });
            }
        }
    }
}