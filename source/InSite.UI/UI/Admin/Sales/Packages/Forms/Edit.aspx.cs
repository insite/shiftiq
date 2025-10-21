using System;
using System.Web;

using InSite.Application.Invoices.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Sales.Packages.Forms
{
    public partial class Edit : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/sales/packages/edit";

        public static string GetNavigateUrl(Guid packageId, string status = null)
        {
            var url = NavigateUrl + "?package=" + packageId;

            if (status.IsNotEmpty())
                url += "&status=" + HttpUtility.UrlEncode(status);

            return url;
        }

        public static void Redirect(Guid quizId, string status = null) =>
            HttpResponseHelper.Redirect(GetNavigateUrl(quizId, status));

        private Guid PackageIdentifier => Guid.TryParse(Request["package"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PublishButton.Click += (s, a) => Publish();
            UnpublishButton.Click += (s, a) => Unpublish();
            SaveButton.Click += (s, a) => Save();
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            Open();

            CancelButton.NavigateUrl = Search.NavigateUrl;
        }

        protected void Open()
        {
            var entity = SelectEntity();
            if (entity == null || entity.OrganizationIdentifier != Organization.OrganizationIdentifier || entity.ProductType != Create.ProductType)
                Search.Redirect();

            PublishButton.Visible = !entity.Published.HasValue;
            UnpublishButton.Visible = !PublishButton.Visible;

            PageHelper.AutoBindHeader(Page, null, entity.ProductName);

            var createdBy = GetUserFromId(entity.CreatedBy);
            CreatedBy.Text = createdBy;
            ModifiedBy.Text = entity.CreatedBy == entity.ModifiedBy ? createdBy : GetUserFromId(entity.ModifiedBy);

            PackageDetails.SetInputValues(entity);
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var entity = SelectEntity();

            PackageDetails.GetInputValues(entity);

            entity.ModifiedBy = User.Identifier;

            ServiceLocator.InvoiceStore.UpdateProduct(entity);

            Search.Redirect();
        }

        private void Publish()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.InvoiceStore.UpdateProduct<TProduct>(PackageIdentifier,
                (x => x.Published, DateTimeOffset.UtcNow),
                (x => x.PublishedBy, CurrentSessionState.Identity.User.Identifier)
            );

            HttpResponseHelper.Redirect(Request.RawUrl);
        }

        private void Unpublish()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.InvoiceStore.UpdateProduct<TProduct>(PackageIdentifier,
                (x => x.Published, null),
                (x => x.PublishedBy, CurrentSessionState.Identity.User.Identifier)
            );

            HttpResponseHelper.Redirect(Request.RawUrl);
        }

        private TProduct SelectEntity() => ServiceLocator.InvoiceSearch.GetProduct(PackageIdentifier);

        private string GetUserFromId(Guid? userId)
            => userId.HasValue ? ServiceLocator.UserSearch.GetUser(userId.Value)?.FullName : "Unprovided";
    }
}