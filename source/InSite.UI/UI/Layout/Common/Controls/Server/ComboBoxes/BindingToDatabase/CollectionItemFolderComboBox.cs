using System;
using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CollectionItemFolderComboBox : ComboBox
    {
        public Guid? OrganizationIdentifier
        {
            get => (Guid?)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        public Guid? CollectionIdentifier
        {
            get => (Guid?)ViewState[nameof(CollectionIdentifier)];
            set => ViewState[nameof(CollectionIdentifier)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            if (CollectionIdentifier.HasValue)
            {
                var values = TCollectionItemCache
                    .Query(new TCollectionItemFilter
                    {
                        OrganizationIdentifier = OrganizationIdentifier,
                        CollectionIdentifier = CollectionIdentifier.Value
                    })
                    .Where(x => x.ItemFolder != null)
                    .OrderBy(x => x.ItemFolder)
                    .Select(x => x.ItemFolder)
                    .Distinct()
                    .ToArray();

                foreach (var value in values)
                    list.Add(value);
            }

            return list;
        }
    }
}