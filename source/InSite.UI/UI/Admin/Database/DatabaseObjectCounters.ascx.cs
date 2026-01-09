using System;

using InSite.Persistence;

namespace InSite.UI.Admin.Databases
{
    public partial class DatabaseObjectCounters : System.Web.UI.UserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
                BindModelToControls();
        }

        protected void BindModelToControls()
        {
            var tableFilter = new VTableFilter();
            var tableColumnFilter = new VTableColumnFilter();
            var foreignKeyFilter = new VForeignKeyFilter();

            var tableCount = VTableSearch.Count(tableFilter);
            var columnCount = VTableColumnSearch.Count(tableColumnFilter);
            var constraintCount = VForeignKeySearch.Count(foreignKeyFilter);

            TableCount.Text = $@"{tableCount:n0}";
            ColumnCount.Text = $@"{columnCount:n0}";
            ConstraintCount.Text = $@"{constraintCount:n0}";

            var entityFilter = new TEntityFilter();
            var entityCount = TEntitySearch.Count(entityFilter);
            EntityCount.Text = $@"{entityCount:n0}";
        }
    }
}