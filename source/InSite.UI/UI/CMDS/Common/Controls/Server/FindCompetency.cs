using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class FindCompetency : BaseFindEntity<FindCompetency.DataFilter>
    {
        #region Classes

        public class DataFilter : Filter
        {
            public CompetencyFilter Filter { get; set; }
            public string Keyword { get; set; }
        }

        #endregion

        #region Properties

        public CompetencyFilter Filter => (CompetencyFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new CompetencyFilter()));

        #endregion

        protected override string GetEntityName() => "Competency";

        protected override DataFilter GetFilter(string keyword) => new DataFilter
        {
            Filter = Filter.Clone(),
            Keyword = keyword
        };

        protected override int Count(DataFilter filter)
        {
            return CompetencyRepository.SelectCountForSelector(filter.Filter, filter.Keyword);
        }

        protected override DataItem[] Select(DataFilter filter)
        {
            filter.Filter.Paging = filter.Paging;

            return CompetencyRepository
                .SelectForSelector(filter.Filter, filter.Keyword)
                .Rows.Cast<DataRow>()
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return CompetencyRepository
                .SelectForSelector(ids)
                .Rows.Cast<DataRow>()
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(DataRow x)
        {
            var text = x["Text"];
            if (text == DBNull.Value || text == null)
                text = "???";

            return new DataItem
            {
                Value = (Guid)x["Value"],
                Text = $"{text}"
            };
        }
    }
}
