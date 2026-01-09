using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Utilities.Collections.Forms
{
    public partial class EditItem : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? CollectionIdentifier
        {
            get => (Guid?)ViewState[nameof(CollectionIdentifier)];
            set => ViewState[nameof(CollectionIdentifier)] = value;
        }

        private Guid? ItemIdentifier
        {
            get => (Guid?)ViewState[nameof(ItemIdentifier)];
            set => ViewState[nameof(ItemIdentifier)] = value;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            if (!Guid.TryParse(Request["item"], out var itemId))
                RedirectToSearch();

            var entity = TCollectionItemSearch.Select(itemId);
            if (entity == null)
                RedirectToSearch();

            PageHelper.AutoBindHeader(Page, null, entity.ItemName);

            CollectionIdentifier = entity.CollectionIdentifier;
            ItemIdentifier = entity.ItemIdentifier;

            ItemDetail.SetInputValues(entity);
        }

        private void Save()
        {
            var item = TCollectionItemSearch.Select(ItemIdentifier.Value);

            ItemDetail.GetInputValues(item);

            TCollectionItemStore.Update(item);

            TCollectionItemCache.Refresh();

            RedirectToParent();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                Save();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/assets/collections/edit?collection={CollectionIdentifier}", true);

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect("/ui/admin/assets/collections/search");

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"collection={CollectionIdentifier}"
                : null;
        }
    }
}
