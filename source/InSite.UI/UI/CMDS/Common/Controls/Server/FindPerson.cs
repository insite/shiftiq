using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class FindPerson : BaseFindEntity<FindPerson.DataFilter>
    {
        #region Classes

        public class DataFilter : Filter
        {
            public CmdsPersonFilter Filter { get; set; }
            public string Keyword { get; set; }
        }

        #endregion

        #region Properties

        public CmdsPersonFilter Filter => (CmdsPersonFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new CmdsPersonFilter { NameFilterType = "Similar" }));

        #endregion

        protected override string GetEntityName() => "Person";

        protected override DataFilter GetFilter(string keyword)
        {
            return new DataFilter
            {
                Filter = Filter.Clone(),
                Keyword = keyword,
            };
        }

        protected override int Count(DataFilter filter)
        {
            return ContactRepository3.SelectCountForPersonSelector(filter.Filter, filter.Keyword);
        }

        protected override DataItem[] Select(DataFilter filter)
        {
            filter.Filter.Paging = filter.Paging;

            return ContactRepository3
                .SelectForPersonSelector(filter.Filter, filter.Keyword)
                .Rows.Cast<DataRow>()
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ContactRepository3
                .SelectForPersonSelector(ids)
                .Rows.Cast<DataRow>()
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(DataRow x) => new DataItem
        {
            Value = (Guid)x["Value"],
            Text = (string)x["Text"],
        };

        public DataItem[] GetDataItems() => Select(GetFilter((string)null));

        public int GetCount() => Count(GetFilter((string)null));

    }
}
