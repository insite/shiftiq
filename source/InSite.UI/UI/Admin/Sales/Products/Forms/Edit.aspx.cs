using System;

using InSite.Application.Invoices.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Invoices.Products.Forms
{
    public partial class Edit : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        #region Properties

        protected string SearchUrl => "/ui/admin/sales/products/search";

        private Guid? ProductIdentifier => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;
        string DefaultParameters => $"id={ProductIdentifier}";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"{DefaultParameters}" : GetParentLinkParameters(parent, null);

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        #endregion

        #region Initialization and Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(EditorStatus, StatusType.Saved);

            Open();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProductDetails.Updated += Product_Updated;
            SaveButton.Click += SaveButton_Click;
            PublishButton.Click += (s, a) => Publish();
            UnpublishButton.Click += (s, a) => Unpublish();
        }

        #endregion

        #region Button Actions

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = SelectEntity();

            ProductDetails.GetInputValues(entity);

            entity.ModifiedBy = User.Identifier;

            ServiceLocator.InvoiceStore.UpdateProduct(entity);

            HttpResponseHelper.Redirect($"/ui/admin/sales/products/edit?id={entity.ProductIdentifier}&status=saved");
        }

        private void Publish()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.InvoiceStore.UpdateProduct<TProduct>(ProductIdentifier.Value,
                (x => x.Published, DateTimeOffset.UtcNow),
                (x => x.PublishedBy, CurrentSessionState.Identity.User.Identifier)
            );

            HttpResponseHelper.Redirect(Request.RawUrl);
        }

        private void Unpublish()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.InvoiceStore.UpdateProduct<TProduct>(ProductIdentifier.Value,
                (x => x.Published, null),
                (x => x.PublishedBy, CurrentSessionState.Identity.User.Identifier)
            );

            HttpResponseHelper.Redirect(Request.RawUrl);
        }

        #endregion

        protected void Open()
        {
            var entity = SelectEntity();
            if (entity == null || entity.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, null, entity.ProductName);

            PublishButton.Visible = !entity.Published.HasValue;
            UnpublishButton.Visible = !PublishButton.Visible;

            if (!entity.Published.HasValue && entity.PublishedBy.HasValue)
                PublishedByLiteral.Text = "Unpublished By";

            CreatedBy.Text = GetUserFromId(entity.CreatedBy, entity.OrganizationIdentifier);
            ModifiedBy.Text = GetUserFromId(entity.ModifiedBy, entity.OrganizationIdentifier);
            PublishedBy.Text = GetUserFromId(entity.PublishedBy, entity.OrganizationIdentifier);
            PublishedOn.Text = entity.Published.HasValue ? entity.Published.Value.ToString("R") : null;

            ProductDetails.SetInputValues(entity);
            PublishButton.Enabled = SetEnableState();

            CancelButton.NavigateUrl = GetParentUrl("");
        }


        private TProduct SelectEntity() =>
            ServiceLocator.InvoiceSearch.GetProduct(ProductIdentifier.Value);

        private string GetUserFromId(Guid? userId, Guid orgId)
            => userId.HasValue ? ServiceLocator.UserSearch.GetUser(userId.Value)?.FullName : "Unprovided";

        private bool SetEnableState()
            => ProductDetails.ObjectIdentifier.HasValue;

        private void Product_Updated(object sender, EventArgs e)
            => PublishButton.Enabled = SetEnableState();
    }
}