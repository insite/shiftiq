using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Assets.Collections.Forms
{
    public partial class DeleteItem : AdminBasePage, IHasParentLinkParameters
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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            TCollectionItemStore.Delete(ItemIdentifier.Value);
            RedirectToParent();
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

            PageHelper.AutoBindHeader(this, null, entity.ItemName);

            CollectionIdentifier = entity.CollectionIdentifier;
            ItemIdentifier = entity.ItemIdentifier;

            SetInputValues(entity);
        }

        private void SetInputValues(TCollectionItem item)
        {
            var collection = TCollectionSearch.Select(CollectionIdentifier.Value);
            var organization = item.OrganizationIdentifier.HasValue
                ? OrganizationSearch.Select(item.OrganizationIdentifier.Value)
                : null;

            OrganizationName.Text = organization?.CompanyName ?? "None";
            FolderName.Text = !string.IsNullOrEmpty(item.ItemFolder) ? item.ItemFolder : "None";
            CollectionItemName.Text = item.ItemName;
            Description.Text = !string.IsNullOrEmpty(item.ItemDescription) ? item.ItemDescription.Replace("\n", "<br>") : "None";
            Status.Text = item.ItemIsDisabled ? "Disabled" : "Enabled";
            HTMLColor.Text = item.ItemColor.HasValue() ?
                $"<div class=\"color-example\" style='background-color:{item.ItemColor}'></div> {item.ItemColor}"
                : "None";
            IconExample.Text = !string.IsNullOrEmpty(item.ItemIcon) ? $"<i class='{item.ItemIcon}'></i> {item.ItemIcon}" : "None";
            ItemHours.Text = item.ItemHours.HasValue ? $"{item.ItemHours:n2}" : "None";
            CollectionName.Text = $"<a href=\"{GetParentUrl()}\">{collection?.CollectionName}</a>";
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }

        private string GetParentUrl() => $"/ui/admin/assets/collections/edit?collection={CollectionIdentifier}";

        private void RedirectToParent()
        {
            var url = GetParentUrl() + "&panel=step";
            HttpResponseHelper.Redirect(url, true);
        }

        private void RedirectToSearch()
        {
            HttpResponseHelper.Redirect("/ui/admin/assets/collections/search");
        }

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"collection={CollectionIdentifier}"
                : null;
        }

        #endregion
    }
}