using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Utilities.Constraints.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VForeignKeyFilter>
    {
        #region Properties

        public override VForeignKeyFilter Filter
        {
            get
            {
                var filter = new VForeignKeyFilter()
                {
                    SchemaName = SchemaName.Text,
                    TableName = TableName.Text,
                    ColumnName = ColumnName.Text,
                    PrimaryTableName = ForeignTableName.Text,
                    EnforcedInclusion = EnforcedInclusion.Value.ToEnum(InclusionType.Include)
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                SchemaName.Text = value.SchemaName;
                TableName.Text = value.TableName;
                ColumnName.Text = value.ColumnName;
                ForeignTableName.Text = value.PrimaryTableName;
                EnforcedInclusion.Value = value.EnforcedInclusion?.GetName(InclusionType.Include);
            }
        }

        #endregion

        #region Helper methods

        public override void Clear()
        {
            SchemaName.Text = null;
            TableName.Text = null;
            ColumnName.Text = null;
            ForeignTableName.Text = null;
            EnforcedInclusion.ClearSelection();
        }

        #endregion
    }
}