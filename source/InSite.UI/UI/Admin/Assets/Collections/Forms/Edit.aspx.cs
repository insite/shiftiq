using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.Admin.Utilities.Collections.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/assets/collections/search";

        private Guid? CollectionIdentifier => Guid.TryParse(Request.QueryString["collection"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ItemGrid.Alert += (s, a) => EditorStatus.AddMessage(a);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var collection = TCollectionSearch.Select(CollectionIdentifier.Value);

            Detail.GetInputValues(collection);

            TCollectionStore.Update(collection);

            EditorStatus.AddMessage(AlertType.Success, "Changes successfully saved");
        }

        private void Open()
        {
            var entity = CollectionIdentifier.HasValue ? TCollectionSearch.Select(CollectionIdentifier.Value) : null;
            if (entity == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, null, entity.CollectionName);

            Detail.SetInputValues(entity);
            ItemGrid.LoadData(entity.CollectionIdentifier);

            ItemsSectionTitle.Text = $"Collection Items <span class='form-text'>({ItemGrid.RowCount:n0})</span>";

            CancelButton.NavigateUrl = SearchUrl;
        }
    }
}
