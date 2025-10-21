using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class GroupAddressTypeMultiComboBox : MultiComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code; 

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add("Billing");
            list.Add("Physical");
            list.Add("Shipping");

            return list;
        }
    }
}