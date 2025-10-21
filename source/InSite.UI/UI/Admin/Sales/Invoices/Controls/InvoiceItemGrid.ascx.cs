using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Invoices.Write;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Invoices.Controls
{
    public partial class InvoiceItemGrid : SearchResultsGridViewController<NullFilter>
    {
        #region Events

        public event EventHandler Refreshed;

        private void OnRefreshed() =>
            Refreshed?.Invoke(this, EventArgs.Empty);

        #endregion

        protected override bool IsFinder => false;

        private Guid InvoiceIdentifier
        {
            get => (Guid)ViewState[nameof(InvoiceIdentifier)];
            set => ViewState[nameof(InvoiceIdentifier)] = value;
        }

        private bool ShowZeroTaxRate
        {
            get => (bool?)ViewState[nameof(ShowZeroTaxRate)] ?? false;
            set => ViewState[nameof(ShowZeroTaxRate)] = value;
        }

        private bool ShowTaxColumn
        {
            get => (bool?)ViewState[nameof(ShowTaxColumn)] ?? false;
            set => ViewState[nameof(ShowTaxColumn)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDeleting += Grid_DeleteCommand;
        }

        private void Grid_DeleteCommand(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var itemId = grid.GetDataKey<Guid>(e);

            ServiceLocator.SendCommand(new RemoveInvoiceItem(InvoiceIdentifier, itemId));

            TotalAmount.Text = ServiceLocator.InvoiceSearch.GetInvoiceTotalAmount(InvoiceIdentifier).ToString("c2");

            SetTaxRateColumnVisibility();

            Search(new NullFilter());

            OnRefreshed();
        }

        public int LoadData(Guid invoiceIdentifier, bool controlsVisible = true)
        {
            InvoiceIdentifier = invoiceIdentifier;

            TotalAmount.Text = ServiceLocator.InvoiceSearch.GetInvoiceTotalAmount(invoiceIdentifier).ToString("c2");

            AddItemLink.NavigateUrl = $"/ui/admin/sales/invoices/items/add?invoice={InvoiceIdentifier}";

            SetTaxRateColumnVisibility();

            Search(new NullFilter());

            if (!controlsVisible)
                SetControlsVisibility();

            return RowCount;
        }

        private void SetTaxRateColumnVisibility()
        {
            var itemsForCheck = ServiceLocator.InvoiceSearch.GetInvoiceItems(InvoiceIdentifier);
            var showTaxRate = itemsForCheck.Any(x => x.TaxRate.HasValue && x.TaxRate.Value > 0m);
            var showTax = AnyLineHasPositiveTax();
            SetTaxColumnVisibility(showTax);
        }

        private void SetControlsVisibility()
        {
            if (!IsPostBack)
            {
                GridView gridView = this.Grid;

                int columnIndex = gridView.Columns.Count - 1;

                if (gridView.Columns[columnIndex] is System.Web.UI.WebControls.TemplateField)
                {
                    System.Web.UI.WebControls.TemplateField templateField = (System.Web.UI.WebControls.TemplateField)gridView.Columns[columnIndex];
                    templateField.Visible = false;
                }
            }
        }

        protected override int SelectCount(NullFilter filter)
        {
            return ServiceLocator.InvoiceSearch.CountInvoiceItems(InvoiceIdentifier);
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            return ServiceLocator.InvoiceSearch
                .GetInvoiceItems(InvoiceIdentifier)
                .ToSearchResult();
        }

        protected string GetProductName(Guid productIdentifier)
        {
            var product = ServiceLocator.InvoiceSearch.GetProduct(productIdentifier);

            return product?.ProductName;
        }

        protected string GetAmount(decimal price, int quantity)
            => GetAmount(price, quantity, null);

        protected string GetAmount(decimal price, int quantity, object taxRateObj)
        {
            decimal baseAmount = price * quantity;
            decimal rate = 0m;

            if (taxRateObj != null && taxRateObj != DBNull.Value)
                decimal.TryParse(taxRateObj.ToString(), out rate);

            var tax = Math.Round(baseAmount * rate, 2, MidpointRounding.AwayFromZero);
            var total = baseAmount + tax;

            return total.ToString("c2");
        }

        private void SetTaxColumnVisibility(bool visible)
        {
            for (int i = 0; i < Grid.Columns.Count; i++)
            {
                if (string.Equals(Grid.Columns[i].HeaderText, "Tax Total", StringComparison.OrdinalIgnoreCase))
                {
                    Grid.Columns[i].Visible = visible;
                    break;
                }
            }
            ShowTaxColumn = visible;
        }

        protected string FormatTax(object rateObj, object priceObj, object qtyObj)
        {
            decimal rate = 0m;
            decimal price = 0m;
            int qty = 0;

            if (rateObj != null && rateObj != DBNull.Value)
                decimal.TryParse(rateObj.ToString(), out rate);

            if (priceObj != null && priceObj != DBNull.Value)
                decimal.TryParse(priceObj.ToString(), out price);

            if (qtyObj != null && qtyObj != DBNull.Value)
                int.TryParse(qtyObj.ToString(), out qty);

            var taxAmount = Math.Round(price * qty * rate, 2, MidpointRounding.AwayFromZero);

            var pctText = (rate > 0m ? (rate * 100m).ToString("0.##") : "0") + "%";
            return $"{taxAmount:c2} ({pctText})";
        }

        private bool AnyLineHasPositiveTax()
        {
            var items = ServiceLocator.InvoiceSearch.GetInvoiceItems(InvoiceIdentifier);
            return items.Any(x => x.TaxRate.HasValue && x.TaxRate.Value > 0m);
        }
    }
}