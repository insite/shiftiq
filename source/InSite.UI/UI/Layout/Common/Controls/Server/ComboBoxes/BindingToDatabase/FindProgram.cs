using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Persistence;

namespace InSite.Common.Web.UI
{
    public class FindProgram : BaseFindEntity<TProgramFilter>
    {
        #region Properties

        public TProgramFilter Filter => (TProgramFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new TProgramFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        #endregion

        protected override string GetEntityName() => "Program";

        protected override TProgramFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.ProgramName = keyword;

            return filter;
        }

        protected override int Count(TProgramFilter filter)
        {
            return ProgramSearch.CountPrograms(filter);
        }

        protected override DataItem[] Select(TProgramFilter filter)
        {
            filter.OrderBy = nameof(TProgram.ProgramName);

            return ProgramSearch
                .GetPrograms(filter)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ProgramSearch
                .GetPrograms(ids)
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(TProgram x) => new DataItem
        {
            Value = x.ProgramIdentifier,
            Text = x.ProgramName,
        };
    }
}
