using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Utilities.Tables.Forms
{
    public partial class Read : AdminBasePage
    {
        #region Properties

        private string OriginalSchemaName => Request["schemaName"];

        private string OriginalTableName => Request["tableName"];

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
                var table = OriginalSchemaName.HasValue() && OriginalTableName.HasValue() ?
                    VTableSearch.Select(OriginalSchemaName, OriginalTableName) : null;

                if (table == null || table.Rows.Count == 0)
                    RedirectToSearch();

                var row = table.Rows[0];

                SchemaTableTitle.Text = $"{row["SchemaName"]}.{row["TableName"]}";

                ColumnCount.Text = string.Format("{0:n0}", row["ColumnCount"]);
                RowCount.Text = string.Format("{0:n0}", row["RowCount"]);

                ColumnsGrid.LoadData(OriginalSchemaName, OriginalTableName);
                DependenciesGrid.LoadDataByForeignTable(OriginalSchemaName, OriginalTableName);
                ReferencesGrid.LoadDataByTable(OriginalSchemaName, OriginalTableName);

                NoDependenciesPanel.Visible = !DependenciesGrid.HasRows;
                NoReferencesPanel.Visible = !ReferencesGrid.HasRows;

                ColumnCountTitle.Text = ColumnsGrid.RowCount.ToString();
                ForeignKeyCountTitle.Text = ReferencesGrid.RowCount.ToString();
            }
            catch (Exception ex)
            {
                Status.AddMessage(AlertType.Error, ex.Message);
            }
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/database/tables/search", true);

        #endregion
    }
}