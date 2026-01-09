using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Utilities.Constraints.Controls
{
    public partial class ConstraintGrid : SearchResultsGridViewController<VForeignKeyFilter>
    {
        #region Properties

        protected override bool IsFinder => false;

        #endregion

        #region Public methods

        public bool LoadDataByTable(string schemaName, string tableName)
        {
            Grid.Columns.FindByHeaderText("Foreign Schema").Visible = false;
            Grid.Columns.FindByHeaderText("Foreign Table").Visible = false;

            var filter = new VForeignKeyFilter { SchemaName = schemaName, TableName = tableName };

            Search(filter);

            return HasRows;
        }

        public bool LoadDataByForeignTable(string foreignSchemaName, string foreignTableName)
        {
            Grid.Columns.FindByHeaderText("Primary Schema").Visible = false;
            Grid.Columns.FindByHeaderText("Primary Table").Visible = false;

            var filter = new VForeignKeyFilter
            {
                PrimarySchemaName = foreignSchemaName,
                PrimaryTableName = foreignTableName,
                IsExactComparison = true
            };
            Search(filter);

            return HasRows;
        }

        #endregion

        #region Search results

        protected override int SelectCount(VForeignKeyFilter filter)
        {
            return VForeignKeySearch.Count(filter);
        }

        protected override IListSource SelectData(VForeignKeyFilter filter)
        {
            return VForeignKeySearch.Select(filter);
        }

        #endregion
    }
}