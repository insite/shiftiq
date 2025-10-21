using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Contract;

namespace InSite.Cmds.Actions.Contact.Category
{
    public partial class Create : AdminBasePage
    {
        private const string SearchUrl = "/ui/cmds/admin/organizations/search";

        private Guid? OrganizationIdentifier => Guid.TryParse(Request["organizationID"], out var value) ? value : (Guid?)null;

        private string OrgUrl => string.Format("/ui/cmds/admin/organizations/edit?id={0}", OrganizationIdentifier);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var organization = OrganizationIdentifier.HasValue ? OrganizationSearch.Select(OrganizationIdentifier.Value) : null;
                if (organization == null)
                    HttpResponseHelper.Redirect(SearchUrl, true);

                PageHelper.BindHeader(this, new BreadcrumbItem[]
                {
                    new BreadcrumbItem("Organizations", SearchUrl),
                    new BreadcrumbItem("Edit", OrgUrl),
                    new BreadcrumbItem("Category", null)
                }, null, organization.CompanyName);

                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void Save()
        {
            var organization = OrganizationSearch.Select(OrganizationIdentifier.Value);
            var category = new VAchievementCategory { OrganizationIdentifier = organization.OrganizationIdentifier };
            GetInputValues(category);

            category.CategoryIdentifier = UniqueIdentifier.Create();

            TAchievementCategoryStore.Insert(category);

            HttpResponseHelper.Redirect(OrgUrl);
        }

        private void GetInputValues(VAchievementCategory category)
        {
            Details.GetInputValues(category);
        }
    }
}