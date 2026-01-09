using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CollectionItemComboBox : ComboBox
    {
        public TCollectionItemFilter ListFilter
            => (TCollectionItemFilter)(ViewState[nameof(ListFilter)] ?? (ViewState[nameof(ListFilter)] = new TCollectionItemFilter()));

        protected override ListItemArray CreateDataSource()
        {
            if (!ListFilter.OrganizationIdentifier.HasValue || ListFilter.CollectionName.IsEmpty())
                return new ListItemArray();

            var items = TCollectionItemCache.Select(ListFilter)
                .Select(x => new ListItem
                {
                    Text = x.ItemName.ToString(),
                    Value = x.ItemIdentifier.ToString(),
                })
                .ToList();

            return new ListItemArray(items);
        }
    }
}