using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant.CMDS;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class ValidForUnitSelector : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add(ValidForUnits.Years);
            list.Add(ValidForUnits.Months);

            return list;
        }
    }
}
