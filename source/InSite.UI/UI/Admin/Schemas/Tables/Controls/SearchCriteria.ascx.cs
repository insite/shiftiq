using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Utilities.Tables.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VTableFilter>
    {
        #region Properties

        public override VTableFilter Filter
        {
            get
            {
                var filter = new VTableFilter()
                {
                    SchemaName = SchemaName.Text,
                    TableName = TableName.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                SchemaName.Text = value.SchemaName;
                TableName.Text = value.TableName;
            }
        }

        #endregion

        #region Helper methods

        public override void Clear()
        {
            SchemaName.Text = null;
            TableName.Text = null;
        }

        #endregion
    }
}