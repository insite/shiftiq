using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class FindCertificate : BaseFindEntity<FindCertificate.DataFilter>
    {
        #region Classes

        public class DataFilter : Filter
        {
            public ProfileFilter Filter { get; set; }
            public string Keyword { get; set; }
        }

        #endregion

        #region Properties

        public ProfileFilter Filter => (ProfileFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new ProfileFilter()));

        #endregion

        protected override string GetEntityName() => "Certificate";

        protected override DataFilter GetFilter(string keyword)
        {
            var result = new DataFilter
            {
                Filter = Filter.Clone(),
                Keyword = keyword,
            };

            result.Filter.ProfileNumber = "Certificate";

            return result;
        }

        protected override int Count(DataFilter filter)
        {
            return ProfileRepository.CountForSelector(filter.Filter, filter.Keyword);
        }

        protected override DataItem[] Select(DataFilter filter)
        {
            filter.Filter.Paging = filter.Paging;

            return ProfileRepository.SelectForSelector(filter.Filter, filter.Keyword)
                .Rows.Cast<DataRow>()
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ProfileRepository.SelectForSelector(ids)
                .Rows.Cast<DataRow>()
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(DataRow x) => new DataItem
        {
            Value = (Guid)x["Value"],
            Text = (string)x["Text"],
        };
    }
}