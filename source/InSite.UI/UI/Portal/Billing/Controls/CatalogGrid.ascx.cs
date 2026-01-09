using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Invoices.Read;

using Shift.Common;

using static InSite.UI.Portal.Billing.Models.PriceSelectionModel;

namespace InSite.UI.Portal.Billing.Controls
{
    public partial class CatalogGrid : Common.Web.UI.BaseUserControl
    {
        #region Events

        public class ItemEventArgs : EventArgs
        {
            public Guid ProductId { get; }
            public int Quantity { get; }

            public ItemEventArgs(Guid productId, int quantity)
            {
                ProductId = productId;
                Quantity = quantity;
            }
        }

        public event EventHandler<ItemEventArgs> AddItem;

        private void OnAddItem(Guid productId, int quantity) =>
            AddItem?.Invoke(this, new ItemEventArgs(productId, quantity));

        #endregion

        #region Properties

        public PriceSelectionMode CurrentMode
        {
            get => (PriceSelectionMode)(ViewState[nameof(CurrentMode)] ?? PriceSelectionMode.ALaCarte);
            private set => ViewState[nameof(CurrentMode)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnLoad(e);

            ProductRepeater.ItemDataBound += ProductRepeater_ItemDataBound;
            ProductRepeater.ItemCommand += ProductRepeater_ItemCommand;
        }

        #endregion

        #region UI Event Handling

        protected void ProductRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var product = (TProduct)e.Item.DataItem;
            var defaultTitle = (PlaceHolder)e.Item.FindControl("DefaultTitle");
            var subscribeTitle = (PlaceHolder)e.Item.FindControl("SubscribeTitle");
            var subName = (Literal)e.Item.FindControl("SubName");
            var subPack = (Literal)e.Item.FindControl("SubPack");
            var qtyPh = (PlaceHolder)e.Item.FindControl("QtyContainer");
            var addBtn = e.Item.FindControl("AddButton") as InSite.Common.Web.UI.Button;
            bool isSubscribe = (CurrentMode == PriceSelectionMode.Subscribe);

            if (defaultTitle != null) defaultTitle.Visible = !isSubscribe;
            if (subscribeTitle != null) subscribeTitle.Visible = isSubscribe;
            if (qtyPh != null) qtyPh.Visible = !isSubscribe;

            if (isSubscribe)
                SetSubscribeModeText(product, subName, subPack);

            if (addBtn == null)
                return;

            SetButtonText(addBtn, CurrentMode);
        }

        protected void ProductRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "Add", StringComparison.OrdinalIgnoreCase)) return;

            var productId = Guid.Parse(Convert.ToString(e.CommandArgument));

            var qtyBox = (TextBox)e.Item.FindControl("QtyInput");
            int qty = 0;
            if (qtyBox != null) int.TryParse(qtyBox.Text, out qty);
            if (qty < 0) qty = 0;

            if (CurrentMode == PriceSelectionMode.Subscribe)
                qty = 1;

            OnAddItem(productId, qty);

            if (qtyBox != null) qtyBox.Text = "1";
        }

        #endregion

        #region Data operations

        public void LoadData(PriceSelectionMode mode)
        {
            CurrentMode = mode;

            var orgId = CurrentSessionState.Identity.Organization.OrganizationIdentifier;

            IEnumerable<TProduct> items = Enumerable.Empty<TProduct>();

            switch (CurrentMode)
            {
                case PriceSelectionMode.Subscribe:

                    items = ServiceLocator.InvoiceSearch
                        .GetProducts(new TProductFilter
                        {
                            OrganizationIdentifier = orgId,
                            IsPublished = true,
                            ProductType = "Package"
                        })
                        .OrderBy(x => x.ProductPrice);
                    break;
                default:
                    items = ServiceLocator.InvoiceSearch
                        .GetProducts(new TProductFilter
                        {
                            OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                            IsPublished = true,
                            ProductType = "Online Course"
                        })
                        .OrderBy(x => x.ProductName);
                    break;
            }

            ProductRepeater.DataSource = items.ToList();
            ProductRepeater.DataBind();
        }

        #endregion

        #region Helper Functions

        private static void SetButtonText(Common.Web.UI.Button addBtn, PriceSelectionMode mode)
        {
            switch (mode)
            {
                case PriceSelectionMode.Package:
                    addBtn.Text = "Add to Package";
                    break;
                case PriceSelectionMode.ALaCarte:
                case PriceSelectionMode.Subscribe:
                default:
                    addBtn.Text = "Add to Cart";
                    break;
            }
        }

        private void SetSubscribeModeText(TProduct product, Literal subName, Literal subPack)
        {
            if (subName != null) subName.Text = Server.HtmlEncode(product.ProductName);
            if (subPack != null)
            {
                var price = GetProductPrice(product.ProductPrice);
                subPack.Text = $"({product.ProductQuantity}-pack) - {price}";
            }
        }

        protected string GetProductPrice(object price)
        {
            var _price = (decimal?)price;
            if (!_price.HasValue)
                return "Free";
            return string.Format("{0:C}", _price);
        }

        protected string GetProductImage(object url)
        {
            var _url = (string)url;
            return _url.IfNullOrEmpty("../../Utilities/Api/Images/ProductPlaceholder.jpg");
        }

        #endregion
    }
}