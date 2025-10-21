using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class FindCompany : BaseFindEntity<CompanyFilter>
    {
        #region Properties

        public CompanyFilter Filter => (CompanyFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new CompanyFilter()));

        #endregion

        protected override string GetEntityName() => "Organization";

        protected override CompanyFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.Name = keyword;

            return filter;
        }

        protected override int Count(CompanyFilter filter)
        {
            return ContactRepository3.CountCompaniesForSelector(filter);
        }

        protected override DataItem[] Select(CompanyFilter filter)
        {
            return ContactRepository3.SelectCompaniesForSelector(filter)
                .Rows.Cast<DataRow>()
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ContactRepository3.SelectCompaniesForSelector(ids)
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