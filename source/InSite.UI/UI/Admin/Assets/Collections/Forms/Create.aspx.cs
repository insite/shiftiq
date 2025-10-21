using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Utilities.Collections.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/assets/collections/edit";
        private const string SearchUrl = "/ui/admin/assets/collections/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);

            Details.SetDefaultInputValues();

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var entity = new TCollection { CollectionIdentifier = UniqueIdentifier.Create() };

            Details.GetInputValues(entity);

            if (TCollectionSearch.Exists(new TCollectionFilter { CollectionName = entity.CollectionName }))
            {
                CreatorStatus.AddMessage(AlertType.Error, "The system already contains a collection having the same key. Please give each process a unique number.");
                return;
            }

            TCollectionStore.Insert(entity);

            HttpResponseHelper.Redirect(EditUrl + $"?collection={entity.CollectionIdentifier}");
        }
    }
}