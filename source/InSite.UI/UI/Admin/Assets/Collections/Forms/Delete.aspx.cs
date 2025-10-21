using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assets.Collections.Forms
{
    public partial class Delete : AdminBasePage
    {
        private Guid? CollectionIdentifier => Guid.TryParse(Request["collection"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;

            CancelButton.NavigateUrl = "/ui/admin/assets/collections/search";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var entity = CollectionIdentifier.HasValue ? TCollectionSearch.Select(CollectionIdentifier.Value) : null;
            if (entity == null)
                HttpResponseHelper.Redirect("/ui/admin/assets/collections/search");

            PageHelper.AutoBindHeader(this, null, entity.CollectionName);

            Detail.BindCollection(entity);

            var itemsCount = TCollectionItemSearch.Count(new TCollectionItemFilter
            {
                CollectionIdentifier = CollectionIdentifier.Value
            });

            ItemsCount.Text = $"{itemsCount:n0}";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            TCollectionStore.Delete(CollectionIdentifier.Value);

            HttpResponseHelper.Redirect("/ui/admin/assets/collections/search");
        }
    }
}