using System.ComponentModel;
using System.Data;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TProgramFilter>
    {
        protected override int SelectCount(TProgramFilter filter)
        {
            return ProgramSearch1.Count(filter);
        }

        protected override IListSource SelectData(TProgramFilter filter)
        {
            var table = ProgramSearch1.Select(filter);
            PrepareDataTableForDisplay(table);
            return table;
        }

        private void PrepareDataTableForDisplay(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                var markdown = row["ProgramDescription"] as string;
                if (markdown != null)
                    row["ProgramDescription"] = Markdown.ToHtml(markdown);
            }
        }
    }
}