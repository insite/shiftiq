using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class CompetencyPrioritySelector : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add("Critical");
            list.Add("Non-Critical");

            return list;
        }
    }
}
