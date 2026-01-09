using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionLayoutInput : BaseUserControl
    {
        #region UI Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LayoutType.AutoPostBack = true;
            LayoutType.ValueChanged += OnLayoutTypeChanged;

            TableColumnsRepeater.ItemDataBound += OnTableColumnDataBound;
            TableColumnsRepeater.ItemCommand += OnTableRepeaterCommand;

            AddTableColumnButton.Click += OnTableColumnAdding;
        }

        #endregion

        #region UI Event Handling

        private void OnLayoutTypeChanged(object sender, EventArgs e) => OnLayoutTypeChanged();

        private void OnLayoutTypeChanged()
        {
            TableColumnsContainer.Visible = LayoutType.Value == "Table";
        }

        private void OnTableColumnDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = e.Item;
            var dataItem = (OptionColumn)item.DataItem;

            var name = (ITextBox)item.FindControl("Name");
            name.Text = dataItem.Content.Title?.Default;

            var alignment = (ComboBox)item.FindControl("Alignment");
            alignment.Value = dataItem.Alignment.GetName();

            var style = (ITextBox)item.FindControl("Style");
            style.Text = dataItem.CssClass;
        }

        private void OnTableRepeaterCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var index = int.Parse((string)e.CommandArgument);
                var items = GetLayoutColumns();

                items.RemoveAt(index);

                TableColumnsRepeater.Visible = items.Count > 0;
                TableColumnsRepeater.DataSource = items;
                TableColumnsRepeater.DataBind();
            }
        }

        private void OnTableColumnAdding(object sender, EventArgs e)
        {
            var items = GetLayoutColumns();

            items.Add(new OptionColumn());

            TableColumnsRepeater.Visible = items.Count > 0;
            TableColumnsRepeater.DataSource = items;
            TableColumnsRepeater.DataBind();
        }

        #endregion

        #region Data binding

        public void SetInputValue(OptionLayout layout)
        {
            LayoutType.Value = layout.Type.GetName();

            OnLayoutTypeChanged();

            TableColumnsRepeater.Visible = layout.Columns.IsNotEmpty();
            TableColumnsRepeater.DataSource = layout.Columns;
            TableColumnsRepeater.DataBind();
        }

        private List<OptionColumn> GetLayoutColumns()
        {
            var result = new List<OptionColumn>();

            foreach (RepeaterItem item in TableColumnsRepeater.Items)
            {
                var name = (ITextBox)item.FindControl("Name");
                var alignment = (ComboBox)item.FindControl("Alignment");
                var style = (ITextBox)item.FindControl("Style");

                result.Add(new OptionColumn(
                    name.Text.NullIfEmpty(),
                    alignment.Value.ToEnum<HorizontalAlignment>(),
                    style.Text.NullIfEmpty()));
            }

            return result;
        }

        public OptionLayout GetCurrentValue()
        {
            var layout = new OptionLayout
            {
                Type = LayoutType.Value.ToEnum<OptionLayoutType>(),
                Columns = GetLayoutColumns().ToArray()
            };

            if (layout.Type != OptionLayoutType.Table)
                layout.Columns = null;

            return layout;
        }

        #endregion
    }
}