using System.Linq;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class GenderComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var items = TCollectionItemCache
                .Query(new TCollectionItemFilter
                {
                    OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                    CollectionName = CollectionName.Contacts_People_Gender_Type
                })
                .OrderBy(x => x.ItemNumber)
                .ToList();

            if (items.Count > 0)
            {
                foreach (var item in items)
                    list.Add(item.ItemName);
            }
            else
            {
                list.Add(GenderType.Female.ToString());
                list.Add(GenderType.Male.ToString());
                list.Add(GenderType.Other.ToString());
                list.Add(GenderType.Unspecified.ToString());
            }

            return list;
        }
    }
}
