using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class GridViewPager : UserControl
    {
        #region Constants

        private const int PageGroupSize = 10;

        #endregion

        #region Classes

        public class Template : ITemplate
        {
            private Page _page;

            public Template(Page page)
            {
                _page = page ?? throw new ArgumentNullException(nameof(page));
            }

            public void InstantiateIn(Control container)
            {
                var pager = (GridViewPager)_page.LoadControl("~/UI/Layout/Common/Controls/GridViewPager.ascx");
                pager.ID = "Pager";

                container.Controls.Add(pager);
                container.DataBinding += Container_DataBinding;
            }

            private static void Container_DataBinding(object sender, EventArgs e)
            {
                var container = (Control)sender;
                var grid = (GridView)container.NamingContainer.NamingContainer;
                var pager = (GridViewPager)container.FindControl("Pager");

                pager.LoadData(grid);
            }
        }

        private class DataItem
        {
            public bool IsActive { get; set; }
            public int Page { get; set; }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemCommand += Repeater_ItemCommand;
        }

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            RaiseBubbleEvent(
                this,
                new GridViewCommandEventArgs(this, e));
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            if (args is CommandEventArgs cmdArgs)
            {
                RaiseBubbleEvent(
                    this,
                    new GridViewCommandEventArgs(this, cmdArgs));

                return true;
            }

            return base.OnBubbleEvent(source, args);
        }

        private void LoadData(GridView grid)
        {
            {
                var curNum = grid.PageIndex + 1;
                var start = (int)Math.Floor((decimal)grid.PageIndex / PageGroupSize) * PageGroupSize + 1;
                var end = Number.CheckRange(start + PageGroupSize - 1, 0, grid.PageCount);
                var data = new List<DataItem>();

                PrevPageContainer.Attributes["class"] = start > PageGroupSize ? "page-item" : "page-item disabled";
                PrevPage.CommandArgument = (start - 1).ToString();

                NextPageContainer.Attributes["class"] = grid.PageCount > end ? "page-item" : "page-item disabled";
                NextPage.CommandArgument = (end + 1).ToString();

                for (var n = start; n <= end; n++)
                    data.Add(new DataItem
                    {
                        Page = n,
                        IsActive = n == curNum
                    });

                Repeater.DataSource = data;
                Repeater.DataBind();
            }

            {
                var format = "Page <strong>{0:n0}</strong> of <strong>{1:n0}</strong> - Items <strong>{2:n0}</strong> to <strong>{3:n0}</strong> of <strong>{4:n0}</strong>";
                format = ((IHasTranslator)Page).Translator.Translate(nameof(GridViewPager) + ".Format", format);

                var startIndex = grid.PageIndex * grid.PageSize;
                var itemsCount = grid.AllowCustomPaging ? grid.VirtualItemCount : grid.Rows.Count;
                Info.Text = string.Format(
                    format,
                    grid.PageIndex + 1,
                    grid.PageCount,
                    startIndex + 1,
                    Number.CheckRange(startIndex + grid.PageSize, 1, itemsCount),
                    itemsCount
                );
            }
        }
    }
}