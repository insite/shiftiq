using System;
using System.Web.UI;

using InSite.Application.Payments.Read;

namespace InSite.Admin.Payments.Discounts.Controls
{
    public partial class Details : UserControl
    {
        private string Code => Request["code"];

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                DiscountCode.Enabled = Code == null;
            }
        }

        public void SetInputValues(TDiscount entity)
        {
            DiscountCode.Text = entity.DiscountCode;
            DiscountPercent.ValueAsDecimal = entity.DiscountPercent;
            DiscountDescription.Text = entity.DiscountDescription;
        }

        public void GetInputValues(TDiscount entity)
        {
            entity.DiscountCode = DiscountCode.Text;
            entity.DiscountPercent = DiscountPercent.ValueAsDecimal ?? decimal.Zero;
            entity.DiscountDescription = DiscountDescription.Text;
        }
    }
}