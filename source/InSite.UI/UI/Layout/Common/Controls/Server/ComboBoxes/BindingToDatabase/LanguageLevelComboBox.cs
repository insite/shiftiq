using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class LanguageLevelComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var items = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CollectionName = CollectionName.Contacts_Settings_Languages_Proficiency
            });

            if (items.Count == 0)
                items = TCollectionItemCache.Select(new TCollectionItemFilter
                {
                    OrganizationIdentifier = OrganizationIdentifiers.Global,
                    CollectionName = CollectionName.Contacts_Settings_Languages_Proficiency
                });

            var list = new ListItemArray { TotalCount = items.Count };
            foreach (var item in items)
                list.Add(new ListItem { Value = item.ItemSequence.ToString(), Text = item.ItemName });

            return list;
        }
    }
}