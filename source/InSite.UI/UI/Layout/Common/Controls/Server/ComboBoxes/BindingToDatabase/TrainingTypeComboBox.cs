using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class TrainingTypeComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var items = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CollectionName = CollectionName.Records_Logbooks_Entry_TrainingType
            });

            if (items.Count == 0)
                items = TCollectionItemCache.Select(new TCollectionItemFilter
                {
                    OrganizationIdentifier = OrganizationIdentifiers.Global,
                    CollectionName = CollectionName.Records_Logbooks_Entry_TrainingType
                });

            var list = new ListItemArray { TotalCount = items.Count };
            foreach (var item in items)
                list.Add(new ListItem { Value = item.ItemName, Text = item.ItemName });

            return list;
        }
    }
}