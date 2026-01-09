using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Utilities.Collections.Forms
{
    public partial class CreateItem : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? CollectionIdentifier => Guid.TryParse(Request["collection"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CollectionIdentifier.HasValue)
                RedirectToSearch();

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);

            ItemDetail.SetDefaultInputValues(CollectionIdentifier.Value);

            SaveButton.Enabled = CanEdit;
        }

        private void Save()
        {
            var item = new TCollectionItem();

            ItemDetail.GetInputValues(item);

            item.CollectionIdentifier = CollectionIdentifier.Value;
            item.ItemIdentifier = UniqueIdentifier.Create();
            item.ItemSequence = TCollectionItemSearch.GetNextSequence(CollectionIdentifier.Value, item.OrganizationIdentifier.Value);

            TCollectionItemStore.Insert(item);

            TCollectionItemCache.Refresh();

            RedirectToParent();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (CanEdit && Page.IsValid)
                Save();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/assets/collections/edit?collection={CollectionIdentifier.Value}&panel=step", true);

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assets/collections/search", true);

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"collection={CollectionIdentifier.Value}"
                : null;
        }

        #endregion
    }
}