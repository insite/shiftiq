using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class PersonAddressTypeMultiComboBox : MultiComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add("Billing");
            list.Add("Home");
            list.Add("Shipping");
            list.Add("Work");

            return list;
        }
    }
}