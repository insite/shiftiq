using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Utilities.Constraints.Forms
{
    public partial class Read : AdminBasePage
    {
        #region Properties

        private string OriginalConstraintName => Request["constraintName"];

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
            {
                LoadData();
            }
        }

        #endregion

        #region LoadData

        private void LoadData()
        {
            if (OriginalConstraintName.IsEmpty())
            {
                RedirectToSearch();
                return;
            }

            try
            {
                var filter = new VForeignKeyFilter
                {
                    UniqueName = OriginalConstraintName
                };

                var keys = VForeignKeySearch.Select(filter).GetList();
                if (keys.IsEmpty())
                    RedirectToSearch();

                var key = (VForeignKey)keys[0];

                UniqueName.Text = key.UniqueName;
                ChildSchemaName.Text = key.ForeignSchemaName;
                ChildTableName.Text = key.ForeignTableName;
                ChildColumnName.Text = key.ForeignColumnName;
                MasterSchemaName.Text = key.PrimarySchemaName;
                MasterTableName.Text = key.PrimaryTableName;
                IsEnforced.SelectedValue = key.IsEnforced.ToString().ToLower();
            }
            catch (Exception ex)
            {
                Status.AddMessage(AlertType.Error, ex.Message);
            }
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/database/constraints/search", true);

        #endregion
    }
}