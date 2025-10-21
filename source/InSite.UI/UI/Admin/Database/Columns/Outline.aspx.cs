using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Databases.Columns
{
    public partial class Outline : AdminBasePage
    {
        #region Properties

        private string OriginalSchemaName => Request["schemaName"];

        private string OriginalTableName => Request["tableName"];

        private string OriginalColumnName => Request["columnName"];

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        #endregion

        #region LoadData

        private void LoadData()
        {
            try
            {
                var column = OriginalSchemaName.HasValue() && OriginalTableName.HasValue() && OriginalColumnName.HasValue() ?
                    VTableColumnSearch.Select(OriginalSchemaName, OriginalTableName, OriginalColumnName) : null;

                if (column == null)
                    RedirectToSearch();

                SchemaTableColumn.Text = $"{column.SchemaName}.{column.TableName}.{column.ColumnName}";

                DataType.Text = column.DataType;

                MaximumLength.Text = column.MaximumLength.ToString();
                // MaximumLengthField.Visible = column.MaximumLength.HasValue;

                IsRequired.Text = column.IsRequired == true ? "Required" : "Not Required";
                NonNullValues.Text = $"{column.NonNullCount:n0} ({column.NonNullPercent:n1}%)";
                DistinctValues.Text = $"{column.DistinctCount:n0}";
            }
            catch (Exception ex)
            {
                Status.AddMessage(AlertType.Error, ex.Message);
            }
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/database/columns/search", true);

        #endregion
    }
}