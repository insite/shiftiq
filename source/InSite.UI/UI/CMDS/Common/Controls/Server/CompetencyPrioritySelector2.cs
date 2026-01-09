using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class CompetencyPrioritySelector2 : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            return new ListItemArray
            {
                { "Critical", "Critical" },
                { "Non-Critical", "Non-Critical" }
            };
        }
    }
}
