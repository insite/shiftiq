using System;

using InSite.UI.Portal.Billing.Models;

using Shift.Constant;

using static InSite.UI.Portal.Billing.Models.PriceSelectionModel;

namespace InSite.UI.Portal.Billing.Controls
{
    public partial class CatalogDetail : Common.Web.UI.BaseUserControl
    {
        #region Events

        public event EventHandler ClickCheckout;
        public event EventHandler ClickCart;

        #endregion

        #region Properties

        private PriceSelectionMode CurrentMode => CatalogGrid.CurrentMode;

        private Guid? CurrentPackageProductId
        {
            get => (Guid?)ViewState[nameof(CurrentPackageProductId)];
            set => ViewState[nameof(CurrentPackageProductId)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            SubscribeBanner.Visible = (PriceSelector.Mode == PriceSelectionMode.Subscribe);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnLoad(e);

            PriceSelector.SelectionChanged += PriceSelector_SelectionChanged;
            PriceSelector.SelectionChanged += (_, args) => ResetCartForSelection();
            PriceSelector.ClickCart += (s, a) => ClickCart?.Invoke(this, a);
            PriceSelector.ClickCheckout += (s, a) => ClickCheckout?.Invoke(this, a);

            CatalogGrid.AddItem += CatalogGrid_AddItem;
        }

        #endregion

        #region UI Event Handling

        private void PriceSelector_SelectionChanged(object sender, PriceSelectionChangedEventArgs e)
        {
            CurrentPackageProductId = e.PackageProductIdentifier;
            CatalogGrid.LoadData(e.Mode);
        }

        private void CatalogGrid_AddItem(object sender, CatalogGrid.ItemEventArgs e)
        {
            var cart = CartStorage.Get();

            ResetCartForSelection();

            cart.Add(e.ProductId, e.Quantity, out var actuallyAdded);

            var product = ServiceLocator.InvoiceSearch.GetProduct(e.ProductId)
                ?? throw new ArgumentException($"Product doesn't exist: {e.ProductId}");

            CartWarning.AddMessage(AlertType.Success, $"{product.ProductName} was added to cart");

            SyncSelectorUi();
        }

        #endregion

        #region Data operations

        internal void LoadData()
        {
            ResetCartForSelection();
            CurrentPackageProductId = PriceSelector.SelectedProductIdentifier;
            CatalogGrid.LoadData(PriceSelector.Mode);
        }

        #endregion

        #region Cart Functions

        private void ResetCartForSelection()
        {
            var cart = CartStorage.Get();
            var newPkgId = PriceSelector.SelectedProductIdentifier;

            int? pkgQty = null;
            if (CurrentMode == PriceSelectionMode.Package && newPkgId.HasValue)
            {
                var pkg = ServiceLocator.InvoiceSearch.GetProduct(newPkgId.Value);
                pkgQty = pkg?.ProductQuantity ?? 0;
            }

            if (cart.Mode != CurrentMode || cart.PackageProductId != newPkgId || cart.PackageQuantity != pkgQty)
                cart.Reset(CurrentMode, newPkgId, pkgQty);

            SyncSelectorUi();
        }

        private void SyncSelectorUi()
        {
            var cart = CartStorage.Get();

            int selected = cart.TotalSelected;
            int? remaining = cart.Mode == PriceSelectionMode.Package ? cart.Remaining : (int?)null;

            bool canViewCart = selected > 0;
            bool canCheckout = cart.Mode == PriceSelectionMode.Package
                                            ? (selected > 0 && cart.Remaining == 0)
                                            : (selected > 0);

            PriceSelector.ApplyCartUi(selected, remaining, canViewCart, canCheckout);
        }

        #endregion
    }
}