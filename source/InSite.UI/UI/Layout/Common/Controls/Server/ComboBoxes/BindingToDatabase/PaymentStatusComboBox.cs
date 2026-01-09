using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class PaymentStatusComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add("Started");
            list.Add("Aborted");
            list.Add("Completed");

            return list;
        }
    }
}