using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Learning.Categories
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        public const string NavigateUrl = "/ui/admin/learning/categories/delete";

        public static string GetNavigateUrl(Guid itemId) => NavigateUrl + "?category=" + itemId;

        public static void Redirect(Guid itemId) => HttpResponseHelper.Redirect(GetNavigateUrl(itemId));

        private Guid? CategoryIdentifier
        {
            get => (Guid?)ViewState[nameof(CategoryIdentifier)];
            set => ViewState[nameof(CategoryIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            TCollectionItemStore.Delete(CategoryIdentifier.Value);

            Search.Redirect();
        }

        private void LoadData()
        {
            if (!Guid.TryParse(Request.QueryString["category"], out var categoryId))
                Search.Redirect();

            CategoryIdentifier = categoryId;

            var entity = TCollectionItemSearch.Select(categoryId);
            if (entity == null || entity.OrganizationIdentifier != Organization.OrganizationIdentifier)
                Search.Redirect();

            PageHelper.AutoBindHeader(this, null, entity.ItemName);

            CategoryFolder.Text = entity.ItemFolder.IfNullOrEmpty("None");
            CategoryName.Text = entity.ItemName;

            CancelButton.NavigateUrl = Edit.GetNavigateUrl(categoryId);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"category={CategoryIdentifier}"
                : null;
        }
    }
}