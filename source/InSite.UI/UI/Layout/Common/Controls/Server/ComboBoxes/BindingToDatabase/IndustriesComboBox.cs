using System.Linq;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class IndustriesComboBox : ComboBox
    {
        public bool DistinctValuesOnly
        {
            get => (bool)(ViewState[nameof(DistinctValuesOnly)] ?? false);
            set => ViewState[nameof(DistinctValuesOnly)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            if (DistinctValuesOnly)
            {
                var list = new ListItemArray();

                var values = TCollectionItemCache
                     .Query(new TCollectionItemFilter { CollectionName = CollectionName.Contacts_Settings_Industries_Name })
                     .Where(x => x.ItemName != null)
                     .OrderBy(x => x.ItemName)
                     .Select(x => x.ItemName)
                     .Distinct()
                     .ToArray();

                foreach (var value in values)
                    list.Add(value);

                return list;
            }
            else
            {
                var data = TCollectionItemCache
                    .Query(new TCollectionItemFilter
                    {
                        OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                        CollectionName = CollectionName.Contacts_Settings_Industries_Name
                    })
                    .Select(x => new ListItem { Value = x.ItemName, Text = x.ItemName })
                    .OrderBy(x => x.Text)
                    .ToList();

                return new ListItemArray(data);
            }
        }
    }
}