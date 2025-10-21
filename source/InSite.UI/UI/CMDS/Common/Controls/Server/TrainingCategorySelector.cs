using System.Data;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class TrainingCategorySelector : ComboBox
    {
        public VAchievementCategoryFilter ListFilter => (VAchievementCategoryFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new VAchievementCategoryFilter()));

        protected override ListItemArray CreateDataSource()
        {
            ListFilter.OrganizationCode = CurrentSessionState.Identity.Organization.Code;

            var list = new ListItemArray();

            var data = VAchievementCategorySearch.SelectForSelector(ListFilter);
            foreach (DataRow row in data.Rows)
                list.Add(row["Value"].ToString(), row["Text"].ToString());

            return list;

        }
    }
}