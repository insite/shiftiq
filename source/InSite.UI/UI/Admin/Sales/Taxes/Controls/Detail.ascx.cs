using System.Web.UI;

using InSite.Application.Invoices.Read;

namespace InSite.UI.Admin.Sales.Taxes.Controls
{
    public partial class Detail : UserControl
    {
        public void SetInputValues(TTax tax)
        {
            TaxName.Text = tax.TaxName;
            RegionCode.Value = tax.RegionCode;
            TaxPercent.ValueAsDecimal = tax.TaxRate * 100m;
        }

        public void GetInputValues(TTax tax)
        {
            tax.TaxName = TaxName.Text;
            tax.RegionCode = RegionCode.Value;
            tax.TaxRate = TaxPercent.ValueAsDecimal.Value / 100m;
        }
    }
}