using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Common.Web.UI
{
    public class FindCollectionItem : BaseFindEntity<FindCollectionItem.CustomFilter>
    {
        [Serializable]
        public class CustomFilter : Filter
        {
            public string CollectionName { get; set; }
            public Guid OrganizationIdentifier { get; set; } = CurrentSessionState.Identity.Organization.Identifier;
            public string Keyword { get; set; }

            public CustomFilter Clone()
                => (CustomFilter)MemberwiseClone();
        }

        public CustomFilter Filter
            => (CustomFilter)(ViewState[nameof(Filter)] ?? (ViewState[nameof(Filter)] = new CustomFilter()));

        public string CollectionName
        {
            get => (string)(ViewState[nameof(CollectionName)] ?? (ViewState[nameof(CollectionName)] = "Collection"));
            set => ViewState[nameof(CollectionName)] = value;
        }

        protected override int Count(CustomFilter filter)
        {
            if (Filter.CollectionName.IsEmpty())
                return 0;

            return TCollectionItemCache.Count(new TCollectionItemFilter
            {
                OrganizationIdentifier = Filter.OrganizationIdentifier,
                CollectionName = Filter.CollectionName
            });
        }

        protected override string GetEntityName() => CollectionName;

        protected override CustomFilter GetFilter(string keyword)
            => Filter.Clone();

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ids.Select(x =>
            {
                var item = TCollectionItemCache.Select(x);
                return item == null
                    ? null
                    : new DataItem
                    {
                        Value = item.ItemIdentifier,
                        Text = item.ItemName
                    };
            });
        }

        protected override DataItem[] Select(CustomFilter filter)
        {
            return TCollectionItemCache
                .Select(new TCollectionItemFilter
                {
                    OrganizationIdentifier = Filter.OrganizationIdentifier,
                    CollectionName = Filter.CollectionName
                })
                .ApplyPaging(filter)
                .Select(x => new DataItem
                {
                    Value = x.ItemIdentifier,
                    Text = x.ItemName
                })
                .ToArray();
        }
    }
}