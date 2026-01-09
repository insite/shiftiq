using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;

namespace InSite.Admin.Records.Periods.Controls
{
    public partial class Detail : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EndGreaterValidator.ServerValidate += EndGreaterValidator_ServerValidate;
        }

        private void EndGreaterValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = PeriodStart.Value < PeriodEnd.Value;
        }

        public void SetInputValues(QPeriod period)
        {
            PeriodName.Text = period.PeriodName;
            PeriodStart.Value = period.PeriodStart.UtcDateTime;
            PeriodEnd.Value = period.PeriodEnd.UtcDateTime;
        }

        public void GetInputValues(QPeriod period)
        {
            period.PeriodName = PeriodName.Text;
            period.PeriodStart = PeriodStart.Value.Value;
            period.PeriodEnd = PeriodEnd.Value.Value;
        }
    }
}