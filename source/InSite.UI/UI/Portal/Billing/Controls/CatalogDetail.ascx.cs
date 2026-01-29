using System;
using System.Collections.Generic;

using InSite.Common.Web;
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

        private bool ChooseLater => Request.QueryString["chooseLater"] == "1";

        private PriceSelectionMode CurrentMode => CatalogGrid.CurrentMode;

        private Guid? CurrentPackageProductId
        {
            get => (Guid?)ViewState[nameof(CurrentPackageProductId)];
            set => ViewState[nameof(CurrentPackageProductId)] = value;
        }

        private int CreditCount
        {
            get => (int)ViewState[nameof(CreditCount)];
            set => ViewState[nameof(CreditCount)] = value;
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

            SaveButton.Click += SaveButton_Click;
        }

        #endregion

        #region UI Event Handling

        private void PriceSelector_SelectionChanged(object sender, PriceSelectionChangedEventArgs e)
        {
            if (CreditCount > 0)
                return;

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

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var products = CatalogGrid.GetSelectedProducts();
            if (products.Count > 0)
                SaveSelectedProducts(products);

            HttpResponseHelper.Redirect("/ui/portal/management/dashboard/home");
        }

        #endregion

        #region Data operations

        internal void LoadData()
        {
            CreditCount = ChooseLater
                ? ServiceLocator.CourseDistributionSearch.CountCredits(Organization.Identifier, User.Identifier)
                : 0;

            if (CreditCount == 0)
                ResetCartForSelection();

            CurrentPackageProductId = PriceSelector.SelectedProductIdentifier;

            CatalogGrid.LoadData(CreditCount == 0 ? PriceSelector.Mode : PriceSelectionMode.Select);

            PriceSelector.Visible = CreditCount == 0;
            SelectBanner.Visible = CreditCount > 0;
            SaveButton.Visible = CreditCount > 0;

            CreditCountSpan.Text = CreditCount.ToString();
            CreditText.Text = CreditCount == 1 ? "credit" : "credits";
        }

        private void SaveSelectedProducts(Dictionary<Guid, int> products)
        {
            var count = 0;
            foreach (var pair in products)
                count += pair.Value;

            if (count == 0)
                return;

            var distributions = ServiceLocator.CourseDistributionSearch.GetCreditDistributions(Organization.Identifier, User.Identifier, count);
            var distributionIndex = 0;

            foreach (var pair in products)
            {
                var productId = pair.Key;
                var product = ServiceLocator.InvoiceSearch.GetProduct(productId);
                var courseId = product.ObjectIdentifier ?? throw new ArgumentNullException("product.ObjectIdentifier");

                for (int i = 0; i < pair.Value && distributionIndex < distributions.Count; i++)
                {
                    var distribution = distributions[distributionIndex++];

                    ServiceLocator.CourseDistributionStore.ModifySubProduct(distribution.CourseDistributionIdentifier, productId, courseId);
                }

                if (distributionIndex == distributions.Count)
                    break;
            }
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