
using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class MessageTypeComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            return new ListItemArray
            {
                "Alert",
                "Invitation",
                "Newsletter",
                "Notification"
            };
        }
    }
}