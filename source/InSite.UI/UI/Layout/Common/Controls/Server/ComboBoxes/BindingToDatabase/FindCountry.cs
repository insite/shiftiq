using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FindCountry : BaseFindEntity<FindCountry.DataFilter>
    {
        #region Classes

        [Serializable]
        public class DataFilter : Filter
        {
            public string Keyword { get; set; }
            public Guid[] Ids { get; set; }
            public DataFilter Clone()
            {
                return (DataFilter)MemberwiseClone();
            }
        }

        #endregion

        #region Properties

        public DataFilter Filter => (DataFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new DataFilter()));

        public bool CanadaFirst
        {
            get => ViewState[nameof(CanadaFirst)] as bool? ?? false;
            set => ViewState[nameof(CanadaFirst)] = value;
        }

        #endregion

        protected override string GetEntityName() => "Countries";

        protected override DataFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.Keyword = keyword;

            return filter;
        }

        protected override int Count(DataFilter filter)
        {
            var count = ServiceLocator.CountrySearch.Count(filter.Keyword);
            return count;
        }

        protected override DataItem[] Select(DataFilter filter)
        {
            var data = ServiceLocator.CountrySearch.Select(filter.Keyword, filter.Paging, false, true, CanadaFirst)
                .Select(x => new DataItem
                {
                    Text = x.Text,
                    Value = new Guid(x.Value)
                }).ToList();

            if (filter.Ids != null && filter.Ids.Length > 0)
                data = data.Where(x => filter.Ids.Any(y => x.Value == y)).ToList();

            return data.ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            var filter = new DataFilter() { Ids = ids };

            return Select(filter);
        }

    }
}