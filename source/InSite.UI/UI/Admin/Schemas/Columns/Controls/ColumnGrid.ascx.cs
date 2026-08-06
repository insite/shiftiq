using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Metadata.Columns.Controls
{
    public partial class ColumnGrid : SearchResultsGridViewController<VTableColumnFilter>
    {
        #region Properties

        protected override bool IsFinder => false;

        #endregion

        #region Public methods

        public void LoadData(string schemaName, string tableName)
        {
            var filter = new VTableColumnFilter { SchemaName = schemaName, TableName = tableName };

            Search(filter);
        }

        #endregion

        #region Search results

        protected override int SelectCount(VTableColumnFilter filter)
        {
            filter.IsExactComparison = true;

            return VTableColumnSearch.Count(filter);
        }

        protected override IListSource SelectData(VTableColumnFilter filter)
        {
            filter.IsExactComparison = true;

            return VTableColumnSearch.Select(filter);
        }

        #endregion
    }
}