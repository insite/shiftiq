using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class SelfAssessmentStatusSelector : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            var data = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = OrganizationIdentifiers.CMDS,
                CollectionName = CollectionName.Validations_SelfAssessment_Status
            });

            foreach (var item in data)
                list.Add(item.ItemName, item.ItemName);

            return list;
        }
    }
}
