using System;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Utilities.Constraints.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VForeignKeyFilter>
    {
        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        #endregion

        #region Methods (event handling)

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var row = (VForeignKey)e.Row.DataItem;
            var isEnforced = row.IsEnforced;

            var enforced = (HtmlControl)e.Row.FindControl("Enforced");
            var notEnforced = (HtmlControl)e.Row.FindControl("NotEnforced");

            enforced.Visible = isEnforced;
            notEnforced.Visible = !isEnforced;
        }

        #endregion

        #region Search Results

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