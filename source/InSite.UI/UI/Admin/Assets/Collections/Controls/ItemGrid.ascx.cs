using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Events;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Utilities.Collections.Controls
{
    public partial class ItemGrid : SearchResultsGridViewController<NullFilter>
    {
        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Classes

        private class DataItem
        {
            public VOrganization Organization { get; set; }
            public TCollection Collection { get; set; }
            public TCollectionSearch.ReferenceInfo[] References { get; internal set; }
            public TCollectionItem Item { get; set; }
            public int ItemNumber { get; internal set; }

            public string ItemColorBoxHtml
            {
                get
                {
                    var indicator = Item.ItemColor.ToEnumNullable<Indicator>();
                    return indicator.HasValue
                        ? $"<i class='fas fa-square text-{indicator.GetContextualClass()} me-2'></i>"
                        : null;
                }
            }

            public string ItemColorName
            {
                get
                {
                    var indicator = Item.ItemColor.ToEnumNullable<Indicator>();
                    return indicator?.GetDescription();
                }
            }
        }

        #endregion

        #region Properties

        protected override int DefaultPageSize => 100;

        protected override bool IsFinder => false;

        protected Guid? CollectionIdentifier
        {
            get => (Guid?)ViewState[nameof(CollectionIdentifier)];
            set => ViewState[nameof(CollectionIdentifier)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UpdatePanel.Request += UpdatePanel_Request;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(JavaScriptHelper),
                "register_reorder",
                string.Format(@"
inSite.common.gridReorderHelper.registerReorder({{
    id:'collectionItems',
    selector:'#{0} > tbody',
    items:'> tr',
    updatePanelId:'{1}',
    updateItemStyle:collectionItemGrid.reorder.updateItemStyle,
    onStart:collectionItemGrid.reorder.onStart,
}});
", Grid.ClientID, UpdatePanel.ClientID),
                true);
        }

        #endregion

        #region Event handlers

        private void UpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (string.IsNullOrEmpty(e.Value))
                return;

            var arguments = e.Value.Split('&');

            if (arguments[0] == "cancel-reorder")
            {

            }
            else if (arguments[0] == "save-reorder")
            {
                var changes = JavaScriptHelper.GridReorder.Parse(arguments[1]);
                var items = TCollectionItemSearch.Select(new TCollectionItemFilter
                {
                    CollectionIdentifier = CollectionIdentifier.Value,
                    OrderBy = "ItemNumber"
                });

                foreach (var move in changes)
                {
                    var itemNum = Grid.GetDataKey<int>(move.Source.ItemIndex, "ItemNumber");

                    var item = items.FirstOrDefault(x => x.ItemNumber == itemNum);
                    if (item != null)
                        item.ItemSequence = move.Destination.ItemIndex + 1;
                }

                var sequence = 1;
                foreach (var item in items.OrderBy(x => x.ItemSequence).ThenBy(x => x.ItemName))
                    item.ItemSequence = sequence++;

                TCollectionItemStore.Update(items);

                RefreshGrid();
            }
        }

        #endregion

        #region Public methods

        public void LoadData(Guid collectionId)
        {
            CollectionIdentifier = collectionId;

            Search(new NullFilter());

            AddButton.NavigateUrl = $"/ui/admin/assets/collections/create-item?collection={collectionId}";
        }

        #endregion

        #region Search results

        protected override int SelectCount(NullFilter filter)
        {
            var count = CollectionIdentifier.HasValue
                ? TCollectionItemSearch.Count(new TCollectionItemFilter { CollectionIdentifier = CollectionIdentifier.Value })
                : 0;

            ReorderButton.Visible = count > 1;

            return count;
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            var collection = TCollectionSearch.Select(CollectionIdentifier.Value);
            var references = TCollectionSearch.ParseCollectionReferences(collection.CollectionReferences);

            return TCollectionItemSearch
                .Select(
                    new TCollectionItemFilter
                    {
                        CollectionIdentifier = CollectionIdentifier.Value,
                        OrderBy = "ItemSequence,Organization.OrganizationCode"
                    },
                    x => x.Organization)
                .Select(x => new DataItem
                {
                    Organization = x.Organization,
                    Collection = collection,
                    References = references,
                    Item = x,
                    ItemNumber = x.ItemNumber
                })
                .ToList()
                .ToSearchResult();
        }

        #endregion

        #region Helper methods

        protected string GetReferenceCount()
        {
            var dataItem = (DataItem)Page.GetDataItem();

            try
            {
                var count = TCollectionItemSearch.GetReferenceCount(dataItem.Collection, dataItem.Item);

                return count.HasValue ? count.Value.ToString() : string.Empty;
            }
            catch (ApplicationError apperr)
            {
                OnAlert(AlertType.Error, apperr.Message);
            }

            return "<span class='text-danger'>ERROR</span>";
        }

        protected string GetItemDescription()
        {
            var dataItem = (DataItem)Page.GetDataItem();

            return !string.IsNullOrEmpty(dataItem.Item.ItemDescription)
                ? dataItem.Item.ItemDescription.Replace("\n", "<br>")
                : string.Empty;
        }

        #endregion
    }
}