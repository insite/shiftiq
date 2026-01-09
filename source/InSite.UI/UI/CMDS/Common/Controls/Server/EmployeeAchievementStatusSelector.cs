using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class EmployeeAchievementStatusSelector : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            var data = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = OrganizationIdentifiers.CMDS,
                CollectionName = CollectionName.Achievements_Credentials_Validity_Status
            });

            foreach (var item in data)
                list.Add(item.ItemName, item.ItemName);

            return list;
        }
    }
}
