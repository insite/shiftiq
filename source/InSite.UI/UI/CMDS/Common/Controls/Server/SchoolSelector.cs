using System.Data;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class SchoolSelector : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var data = ContactRepository3.SelectForSchoolSelector();
            foreach (DataRow row in data.Rows)
                list.Add(row["Value"].ToString(), row["Text"].ToString());

            return list;
        }
    }
}
