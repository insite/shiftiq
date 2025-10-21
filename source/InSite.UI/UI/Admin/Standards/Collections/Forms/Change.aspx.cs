using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Admin.Standards.Collections.Forms
{
    public partial class Change : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/standards/collections/search";

        private Guid StandardIdentifier => Guid.TryParse(Request["asset"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                HttpResponseHelper.Redirect(SearchUrl);

            if (IsPostBack)
                return;

            Open();


            CancelButton.NavigateUrl = $"/ui/admin/standards/collections/outline?asset={StandardIdentifier}";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = ServiceLocator.StandardSearch.GetStandard(StandardIdentifier);
            entity.ContentTitle = ContentTitle.Text;
            entity.StandardLabel = StandardLabel.Text;

            StandardStore.Update(entity);

            HttpResponseHelper.Redirect($"/ui/admin/standards/collections/outline?asset={StandardIdentifier}");
        }

        private void Open()
        {
            var entity = StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardIdentifier);
            if (entity.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.BindHeader(this, new BreadcrumbItem[]
            {
                new BreadcrumbItem("Standards", "/ui/admin/standards/home"),
                new BreadcrumbItem("Collections", "/ui/admin/standards/collections/search"),
                new BreadcrumbItem("Outline", $"/ui/admin/standards/collections/outline?asset={StandardIdentifier}"),
                new BreadcrumbItem("Change", null, null, "active"),
            }, null, entity.ContentTitle);

            ContentTitle.Text = entity.ContentTitle;
            StandardLabel.Text = entity.StandardLabel;
        }
    }
}