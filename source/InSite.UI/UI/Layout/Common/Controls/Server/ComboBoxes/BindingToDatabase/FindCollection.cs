using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Common.Web.UI
{
    public class FindCollection : BaseFindEntity<FindCollection.CustomFilter>
    {
        [Serializable]
        public class CustomFilter : Filter
        {
            public string Keyword { get; set; }

            public CustomFilter Clone()
                => (CustomFilter)MemberwiseClone();
        }

        private DataItem[] AllItems
        {
            get
            {
                var items = (DataItem[])ViewState[nameof(AllItems)];
                if (items == null)
                {
                    ViewState[nameof(AllItems)] = items = TCollectionSearch.Bind(
                        x => new DataItem
                        {
                            Value = x.CollectionIdentifier,
                            Text = x.CollectionName
                        },
                        new TCollectionFilter
                        {
                            OrderBy = "Text"
                        });
                }
                return items;
            }
        }

        public CustomFilter Filter
            => (CustomFilter)(ViewState[nameof(Filter)] ?? (ViewState[nameof(Filter)] = new CustomFilter()));

        protected override int Count(CustomFilter filter)
        {
            return CreateQuery(filter).Count();
        }

        protected override string GetEntityName() => "Collection";

        protected override CustomFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();
            filter.Keyword = keyword;
            return filter;
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return AllItems.Where(x => ids.Contains(x.Value));
        }

        protected override DataItem[] Select(CustomFilter filter)
        {
            return CreateQuery(filter).ApplyPaging(filter).ToArray();
        }

        private IQueryable<DataItem> CreateQuery(CustomFilter filter)
        {
            var query = AllItems.AsQueryable();

            if (filter.Keyword.IsNotEmpty())
                query = query.Where(x => (x.Text ?? "").IndexOf(filter.Keyword, StringComparison.OrdinalIgnoreCase) >= 0);

            return query;
        }
    }
}