using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class DocumentSubTypeComboBox : ComboBox
    {
        public Guid OrganizationIdentifier
        {
            get => (Guid?)ViewState[nameof(OrganizationIdentifier)] ?? CurrentSessionState.Identity.Organization.Identifier;
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        public string DocumentType
        {
            get => (string)ViewState[nameof(DocumentType)];
            set => ViewState[nameof(DocumentType)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            if (string.IsNullOrEmpty(DocumentType))
                return list;

            var items = GetItems();

            var filter = $"{DocumentType}:";

            var values = items
                .Where(x => x.ItemName.StartsWith(filter))
                .OrderBy(x => x.ItemSequence)
                .Select(x => x.ItemName.Substring(filter.Length).Trim())
                .ToList();

            foreach (var value in values)
                list.Add(value);

            return list;
        }

        private List<TCollectionItem> GetItems()
        {
            var filter = new TCollectionItemFilter
            {
                OrganizationIdentifier = OrganizationIdentifier,
                CollectionName = CollectionName.Assets_Files_Document_SubType
            };

            var items = TCollectionItemCache.Select(filter);

            if (items.Count == 0 && OrganizationIdentifier != OrganizationIdentifiers.Global)
            {
                filter.OrganizationIdentifier = OrganizationIdentifiers.Global;
                items = TCollectionItemCache.Select(filter);
            }

            return items;
        }
    }
}