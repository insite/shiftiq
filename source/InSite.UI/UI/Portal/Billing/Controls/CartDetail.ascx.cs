using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Billing.Models;

using Shift.Common;

using static InSite.UI.Portal.Billing.Models.PriceSelectionModel;

namespace InSite.UI.Portal.Billing.Controls
{
    public partial class CartDetail : Common.Web.UI.BaseUserControl
    {
        #region Events

        public event EventHandler StateChanged;
        private void OnStateChanged() => StateChanged?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        public bool AllowCheckout
        {
            get => (bool)(ViewState[nameof(AllowCheckout)] ?? false);
            private set => ViewState[nameof(AllowCheckout)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CartRepeater.ItemCommand += CartRepeater_ItemCommand;
            CartRepeater.ItemDataBound += CartRepeater_ItemDataBound;
        }

        #endregion

        #region UI Event Handling

        protected void CartRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var cart = CartStorage.Get();
            bool isPackage = cart.Mode == PriceSelectionMode.Package;

            var qtyWrap = e.Item.FindControl("QtyWrap") as HtmlGenericControl;
            var priceWrap = e.Item.FindControl("PriceWrap") as HtmlGenericControl;

            if (qtyWrap != null)
            {
                var cls = qtyWrap.Attributes["class"] ?? "";
                cls = cls.Replace(" ms-auto", "").Replace(" me-3", "");

                if (isPackage)
                    qtyWrap.Attributes["class"] = cls + " ms-auto";
                else
                    qtyWrap.Attributes["class"] = cls + " me-3";

            }

            if (priceWrap != null)
                priceWrap.Visible = !isPackage;
        }

        protected void CartRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "step", StringComparison.OrdinalIgnoreCase))
                return;

            var parts = Convert.ToString(e.CommandArgument).Split('|');
            if (parts.Length != 2) return;

            if (!Guid.TryParse(parts[0], out var productId)) return;
            var plus = parts[1] == "+1";

            var cart = CartStorage.Get();

            if (plus)
            {
                cart.Add(productId, 1, out var actuallyAdded);
            }
            else
            {
                var current = cart.Items.ContainsKey(productId) ? cart.Items[productId] : 0;
                var next = Math.Max(0, current - 1);
                if (next == 0) cart.Items.Remove(productId); else cart.Items[productId] = next;
            }

            LoadData(cart);
        }

        protected void QtyInput_TextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            var item = (RepeaterItem)tb.NamingContainer;
            var hid = (HiddenField)item.FindControl("ProductId");

            if (!Guid.TryParse(hid.Value, out var productId))
                return;

            int desired = 0;
            int.TryParse((tb.Text ?? "0").Trim(), out desired);
            desired = Math.Max(0, desired);

            var cart = CartStorage.Get();

            if (cart.Mode == PriceSelectionMode.Package && (cart.PackageQuantity ?? 0) > 0)
            {
                var capacity = cart.PackageQuantity.Value;
                var current = cart.Items.ContainsKey(productId) ? cart.Items[productId] : 0;
                var otherSum = cart.TotalSelected - current;
                var maxForThis = Math.Max(0, capacity - otherSum);

                desired = Math.Min(desired, maxForThis);
            }

            if (desired == 0) cart.Items.Remove(productId);
            else cart.Items[productId] = desired;

            LoadData(cart);
        }

        #endregion

        #region Data operations

        internal void LoadData(CartState cart)
        {
            SetupModeVisibility(cart);

            if (HandleEmptyCart(cart))
                return;

            switch (cart.Mode)
            {
                case PriceSelectionMode.ALaCarte:
                    BindAlaCarte(cart);
                    break;

                case PriceSelectionMode.Package:
                    BindPackage(cart);
                    break;

                case PriceSelectionMode.Subscribe:
                    BindSubscribe(cart);
                    break;

                default:
                    EmptyState.Visible = cart.TotalSelected == 0;
                    SetFinalPricing(string.Empty, string.Empty, string.Empty);
                    AllowCheckout = false;
                    break;
            }

            OnStateChanged();
            UpdatePanel();
        }

        private void BindAlaCarte(CartState cart)
        {
            BindListItems(cart, isPackage: false);

            var subtotal = CalculateSubtotal(cart);
            SetFinalPricing(Currency(subtotal), string.Empty, Currency(subtotal));

            SetCartHeader(string.Empty);
            AllowCheckout = cart.TotalSelected > 0;
        }

        private void BindPackage(CartState cart)
        {
            BindListItems(cart, isPackage: true);

            var package = cart.PackageProductId.HasValue
                ? ServiceLocator.InvoiceSearch.GetProduct(cart.PackageProductId.Value)
                : null;

            var packageName = package?.ProductName ?? "Package";
            var packagePrice = package?.ProductPrice ?? 0m;
            var packSize = package?.ProductQuantity;

            SetCartHeader($"{packageName} ({packSize}-pack) - {Currency(packagePrice)}");

            SetFinalPricing(Currency(packagePrice), string.Empty, Currency(packagePrice));

            AllowCheckout = cart.TotalSelected > 0
                         && cart.Remaining == 0
                         && (cart.PackageQuantity ?? 0) > 0;
        }

        private void BindSubscribe(CartState cart)
        {
            BindListItems(cart, isPackage: false);

            var subtotal = CalculateSubtotal(cart);

            SetCartHeader("Subscribe & Choose Later");

            SetFinalPricing(Currency(subtotal), string.Empty, Currency(subtotal));
            AllowCheckout = cart.TotalSelected > 0;
        }

        private bool HandleEmptyCart(CartState cart)
        {
            if (cart.Items.Count != 0) return false;

            EmptyState.Visible = true;
            CartRepeater.DataSource = null;
            CartRepeater.DataBind();

            SetFinalPricing(Currency(0m), string.Empty, Currency(0m));
            AllowCheckout = false;
            OnStateChanged();
            UpdatePanel();
            return true;
        }

        private void BindListItems(CartState cart, bool isPackage)
        {
            var lines = new List<CartProductItem>(cart.Items.Count);
            var addPackSuffix = (cart.Mode == PriceSelectionMode.Subscribe);

            foreach (var kv in cart.Items)
            {
                var pid = kv.Key;
                var qty = kv.Value;

                var product = ServiceLocator.InvoiceSearch.GetProduct(pid);
                var unit = product?.ProductPrice ?? 0m;
                var unitForDisplay = isPackage ? 0m : unit;
                var name = product?.ProductName ?? "(Unknown)";
                var summary = product?.ProductSummary;

                var packQty = product?.ProductQuantity ?? 0;
                if (addPackSuffix && packQty > 0)
                    name = $"{name} ({packQty}-pack)";

                lines.Add(new CartProductItem
                {
                    ProductId = pid,
                    Name = name,
                    Url = "/",
                    Qty = qty,
                    UnitPrice = unitForDisplay,
                    Summary = summary
                });
            }

            CartRepeater.DataSource = lines.OrderBy(l => l.Name).ToList();
            CartRepeater.DataBind();
        }

        #endregion

        #region Helper Functions

        private void SetupModeVisibility(CartState cart)
        {
            var showList = cart.Mode == PriceSelectionMode.ALaCarte
                        || cart.Mode == PriceSelectionMode.Package
                        || cart.Mode == PriceSelectionMode.Subscribe;

            CartBlock.Visible = showList;
            ModeNotSupported.Visible = !showList;
            EmptyState.Visible = false;
        }

        private decimal CalculateSubtotal(CartState cart)
        {
            decimal subtotal = 0m;

            foreach (var kv in cart.Items)
            {
                var product = ServiceLocator.InvoiceSearch.GetProduct(kv.Key);
                var unit = product?.ProductPrice ?? 0m;
                subtotal += unit * kv.Value;
            }

            return subtotal;
        }

        private static string Currency(decimal amount) => string.Format("{0:C}", amount);

        private void SetCartHeader(string text)
        {
            CartHeader.Visible = text.IsNotEmpty();
            CartHeader.InnerText = text;
        }

        private void SetFinalPricing(string subtotal, string tax, string total)
        {
            SubtotalLit.Text = subtotal;
            TaxLit.Text = tax;
            TotalLit.Text = total;
        }

        private void UpdatePanel()
        {
            if (Page.Master is PortalMaster pm)
                pm.UpdateHeaderCartBadge();
        }

        #endregion
    }
}