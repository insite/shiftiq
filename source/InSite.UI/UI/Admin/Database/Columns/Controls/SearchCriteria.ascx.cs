using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Metadata.Columns.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VTableColumnFilter>
    {
        #region Properties

        public override VTableColumnFilter Filter
        {
            get
            {
                var filter = new VTableColumnFilter
                {
                    SchemaName = SchemaName.Text,
                    TableName = TableName.Text,
                    ColumnName = ColumnName.Text,
                    IsRequired = IsRequired.ValueAsBoolean,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                SchemaName.Text = value.SchemaName;
                TableName.Text = value.TableName;
                ColumnName.Text = value.ColumnName;
                IsRequired.ValueAsBoolean = value.IsRequired;
            }
        }

        #endregion

        #region Helper methods

        public override void Clear()
        {
            SchemaName.Text = null;
            TableName.Text = null;
            ColumnName.Text = null;
            IsRequired.ClearSelection();
        }

        #endregion
    }
}