using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant.CMDS;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class ValidForUnitSelector2 : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            return new ListItemArray
            {
                { ValidForUnits.Years, ValidForUnits.Years },
                { ValidForUnits.Months, ValidForUnits.Months }
            };
        }
    }
}
