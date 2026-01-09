using System;

using System.Web.UI;

using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;
using InSite.UI.Portal.Billing.Models;

using static InSite.UI.Portal.Billing.Models.PriceSelectionModel;

namespace InSite.UI.Portal.Billing
{
    public partial class PriceSelector : UserControl
    {
        public Guid? SelectedProductIdentifier
        {
            get => (Guid?)ViewState[nameof(SelectedProductIdentifier)];
            private set => ViewState[nameof(SelectedProductIdentifier)] = value;
        }

        private int? SelectedPackageQuantity
        {
            get => (int?)ViewState[nameof(SelectedPackageQuantity)];
            set => ViewState[nameof(SelectedPackageQuantity)] = value;
        }

        public event EventHandler ClickCheckout;
        public event EventHandler ClickCart;

        public event EventHandler<PriceSelectionChangedEventArgs> SelectionChanged;

        public PriceSelectionMode Mode
        {
            get
            {
                var raw = ProductPriceSelectorComboBox.Value?.Trim();

                if (string.Equals(raw, ProductPriceSelectorComboBox.SubscribeValue, StringComparison.Ordinal))
                    return PriceSelectionMode.Subscribe;

                if (Guid.TryParse(raw, out var g))
                    return g == Guid.Empty ? PriceSelectionMode.ALaCarte : PriceSelectionMode.Package;

                return PriceSelectionMode.ALaCarte;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            UpdateCountersBasedOnSelection();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProductPriceSelectorComboBox.AutoPostBack = true;
            ProductPriceSelectorComboBox.ValueChanged += ProductPriceSelectorComboBox_SelectedIndexChanged;

            CartButton.Click += (s, a) => ClickCart?.Invoke(s, a);
            CheckoutButton.Click += (s, a) => ClickCheckout?.Invoke(s, a);
        }

        private void CartButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CheckoutButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            UpdateCountersBasedOnSelection();
            RaiseSelectionChanged();
        }

        protected void ProductPriceSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCountersBasedOnSelection();
            RaiseSelectionChanged();
        }

        public void ApplyCartUi(int selectedQty, int? remainingQty, bool canViewCart, bool canCheckout)
        {
            SelectedCount.InnerText = $"{selectedQty} selected";
            SelectedCount.Visible = true;

            if (remainingQty.HasValue)
            {
                RemainingCount.InnerText = $"{Math.Max(remainingQty.Value, 0)} remaining";
                RemainingCount.Visible = true;
            }
            else
            {
                RemainingCount.Visible = false;
            }

            EnableButton(CartButton, canViewCart);
            EnableButton(CheckoutButton, canCheckout);
        }

        private void EnableButton(InSite.Common.Web.UI.Button button, bool enabled)
        {
            button.Enabled = enabled;
        }

        private void RaiseSelectionChanged()
        {
            SelectionChanged?.Invoke(
                this,
                new PriceSelectionChangedEventArgs(Mode, SelectedProductIdentifier)
            );
        }

        private void UpdateCountersBasedOnSelection()
        {
            var raw = ProductPriceSelectorComboBox.Value?.Trim();
            SelectedProductIdentifier = null;
            var cart = CartStorage.Get();

            if (string.Equals(raw, ProductPriceSelectorComboBox.SubscribeValue, StringComparison.Ordinal))
            {
                SetFields(false, false, false);
                EnableButton(CartButton, cart.TotalSelected > 0);
                EnableButton(CheckoutButton, cart.TotalSelected > 0);
                return;
            }

            if (Guid.TryParse(raw, out var guid))
            {
                var selected = cart.TotalSelected;

                if (guid == Guid.Empty)
                {
                    SetFields(true, true, false);
                    SelectedProductIdentifier = null;
                    SelectedCount.InnerText = $"{selected} selected";
                    return;
                }

                SetFields(true, true, true);
                SelectedProductIdentifier = guid;
                var product = TryLoadProduct(guid);
                if (product == null)
                    return;

                var qty = product?.ProductQuantity ?? 0;
                var remaining = Math.Max(qty - selected, 0);

                SelectedPackageQuantity = qty;
                RemainingCount.InnerText = $"{remaining} remaining";
                return;
            }

            SetFields(false, false, false);
        }

        private void SetFields(bool counter, bool selected, bool remaining)
        {
            CounterPanel.Visible = counter;
            SelectedCount.Visible = selected;
            RemainingCount.Visible = remaining;

            if (remaining && string.IsNullOrWhiteSpace(RemainingCount.InnerText))
                RemainingCount.InnerText = "0 remaining";
        }

        private TProduct TryLoadProduct(Guid productId)
          => ServiceLocator.InvoiceSearch.GetProduct(productId);
    }
}