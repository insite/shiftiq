using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Domain.Surveys.Forms;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Controls
{
    [ParseChildren(true), PersistChildren(false)]
    public class ReportCorrelationMatrix : System.Web.UI.Control, INamingContainer
    {
        #region Delegates

        public delegate void CellEventHandler(object sender, CellEventArgs e);

        #endregion

        #region Classes

        public class Cell : System.Web.UI.Control, IDataItemContainer
        {
            #region Properties

            public object DataItem { get; private set; }

            int IDataItemContainer.DataItemIndex
            {
                get { return -1; }
            }

            int IDataItemContainer.DisplayIndex
            {
                get { return -1; }
            }

            #endregion

            #region Fields

            private string _valueField;
            private ITemplate _template;

            #endregion

            #region Construction

            public Cell()
            {
                DataItem = null;
                _valueField = null;
                _template = null;
            }

            internal Cell(ITemplate template)
                : this()
            {
                _template = template;

                Controls.Add(new LiteralControl());

                if (_template != null)
                    _template.InstantiateIn(this);
            }

            internal Cell(ResponseAnalysisCorrelationItem dataRow, string valueField, ITemplate template)
                : this(template)
            {
                DataItem = dataRow;
                _valueField = valueField;
            }

            #endregion

            #region Event handlers

            protected override void OnDataBinding(EventArgs e)
            {
                if (_template == null && !string.IsNullOrEmpty(_valueField))
                {
                    var row = DataItem;
                    if (row != null)
                    {
                        var value = DataBinder.Eval(row, _valueField);

                        ((LiteralControl)Controls[0]).Text = value?.ToString();
                    }
                }

                base.OnDataBinding(e);
            }

            #endregion
        }

        public class CellEventArgs : EventArgs
        {
            #region Properties

            public Cell Cell { get; private set; }

            #endregion

            #region Construction

            public CellEventArgs(Cell cell)
            {
                Cell = cell;
            }

            #endregion
        }

        #endregion

        #region Events

        public event CellEventHandler CellCreated;
        private void OnCellCreated(CellEventArgs args) => CellCreated?.Invoke(this, args);

        public event CellEventHandler CellDataBound;
        private void OnCellDataBound(CellEventArgs args) => CellDataBound?.Invoke(this, args);

        #endregion

        #region Properties

        public string RowField
        {
            get { return (string)ViewState[nameof(RowField)]; }
            set { ViewState[nameof(RowField)] = value; }
        }

        public string ColumnField
        {
            get { return (string)ViewState[nameof(ColumnField)]; }
            set { ViewState[nameof(ColumnField)] = value; }
        }

        public string ValueField
        {
            get { return (string)ViewState[nameof(ValueField)]; }
            set { ViewState[nameof(ValueField)] = value; }
        }

        public Unit Width
        {
            get { return ViewState[nameof(Width)] == null ? Unit.Empty : (Unit)ViewState[nameof(Width)]; }
            set { ViewState[nameof(Width)] = value; }
        }

        public string CssClass
        {
            get { return (string)ViewState[nameof(CssClass)]; }
            set { ViewState[nameof(CssClass)] = value; }
        }

        public bool HasTotalCell
        {
            get { return ViewState[nameof(HasTotalCell)] != null && (bool)ViewState[nameof(HasTotalCell)]; }
            set { ViewState[nameof(HasTotalCell)] = value; }
        }

        public string ColumnHeader
        {
            get { return (string)ViewState[nameof(ColumnHeader)]; }
            set { ViewState[nameof(ColumnHeader)] = value; }
        }

        public string RowHeader
        {
            get { return (string)ViewState[nameof(RowHeader)]; }
            set { ViewState[nameof(RowHeader)] = value; }
        }

        public bool ShowRowLabels
        {
            get { return ViewState[nameof(ShowRowLabels)] == null || (bool)ViewState[nameof(ShowRowLabels)]; }
            set { ViewState[nameof(ShowRowLabels)] = value; }
        }

        public bool ShowColumnLabels
        {
            get { return ViewState[nameof(ShowColumnLabels)] == null || (bool)ViewState[nameof(ShowColumnLabels)]; }
            set { ViewState[nameof(ShowColumnLabels)] = value; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(Cell))]
        public virtual ITemplate CellTemplate
        {
            get;
            set;
        }

        private IList<string> Rows
        {
            get { return (IList<string>)ViewState[nameof(Rows)]; }
            set { ViewState[nameof(Rows)] = value; }
        }

        private IList<string> Columns
        {
            get { return (IList<string>)ViewState[nameof(Columns)]; }
            set { ViewState[nameof(Columns)] = value; }
        }

        #endregion

        #region Fields

        private Cell[,] _cells;

        #endregion

        #region Initialization

        protected override void CreateChildControls()
        {
            RecreateChildControls(null);
        }

        private void RecreateChildControls(ResponseAnalysisCorrelationItem[,] data)
        {
            Controls.Clear();

            if (Rows.IsEmpty() || Columns.IsEmpty())
                return;

            var isDataBinding = data != null;

            _cells = new Cell[Columns.Count, Rows.Count];

            for (var x = 0; x < Columns.Count; x++)
            {
                for (var y = 0; y < Rows.Count; y++)
                {
                    var cell = isDataBinding ? new Cell(data[x, y], ValueField, CellTemplate) : new Cell(CellTemplate);
                    var eventArgs = new CellEventArgs(cell);

                    OnCellCreated(eventArgs);

                    Controls.Add(cell);

                    if (isDataBinding)
                    {
                        cell.DataBind();

                        OnCellDataBound(eventArgs);
                    }

                    _cells[x, y] = cell;
                }
            }
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            base.OnLoad(e);
        }

        public void LoadData(IEnumerable<ResponseAnalysisCorrelationItem> data)
        {
            if (data == null || !data.Any())
                return;

            ValidateColumn(RowField, "RowsField");
            ValidateColumn(ColumnField, "ColumnsField");

            if (CellTemplate == null)
                ValidateColumn(ValueField, "ValuesField");

            Rows = new List<string>();
            Columns = new List<string>();

            var rowsData = DistinctValues(data, RowField);
            foreach (object rowValue in rowsData)
                Rows.Add(rowValue?.ToString());

            var columnsData = DistinctValues(data, ColumnField);
            foreach (object columnValue in columnsData)
                Columns.Add(columnValue?.ToString());

            var matrixData = new ResponseAnalysisCorrelationItem[Columns.Count, Rows.Count];
            foreach (var item in data)
            {
                var rowValue = DataBinder.Eval(item, RowField);
                var columnValue = DataBinder.Eval(item, ColumnField);

                var rowIndex = rowsData.IndexOf(rowValue);
                var columnIndex = columnsData.IndexOf(columnValue);

                matrixData[columnIndex, rowIndex] = item;
            }

            RecreateChildControls(matrixData);
        }

        public void Clear()
        {
            if (Rows != null)
                Rows.Clear();

            if (Columns != null)
                Columns.Clear();

            RecreateChildControls(null);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            if (Columns.IsEmpty() || Rows.IsEmpty() || _cells == null)
                return;

            if (ShowRowLabels)
            {
                writer.Write("<label class='matrix-y-axis'>{0}</label>", RowHeader);
            }

            writer.Write("<table id='{0}' border='0' cellspacing='0' cellpadding='0' class='{1}'>", ClientID, (!string.IsNullOrEmpty(CssClass) ? CssClass.Trim() : "matrix"));

            writer.Write("<colgroup>");
            if (ShowRowLabels)
            {
                for (var i = 0; i < Columns.Count + 1; i++)
                    writer.Write("<col/>");
            }
            else
            {
                for (var i = 0; i < Columns.Count; i++)
                    writer.Write("<col/>");

            }
            writer.Write("</colgroup>");

            if (ShowColumnLabels)
            {
                writer.Write("<thead><tr>");

                if (!string.IsNullOrEmpty(ColumnHeader))
                {
                    if (ShowRowLabels)
                        writer.Write("<th></th>");

                    writer.Write("<th class='matrix-x-axis' colspan='{0}'>{1}</th></tr><tr>", Columns.Count, ColumnHeader);
                }

                if (ShowRowLabels)
                {
                    writer.Write("<th></th>");
                }

                for (int i = 0; i < Columns.Count; i++)
                {
                    writer.Write("<th class='column'>");
                    writer.Write((i + 1).ToString());
                    writer.Write("</th>");
                }

                writer.Write("</tr></thead>");
            }

            writer.Write("<tbody>");

            for (int y = 0; y < Rows.Count; y++)
            {
                string rowClass = y % 2 == 0 ? "tr-row" : "alt-row";
                if (HasTotalCell && y == Rows.Count - 1)
                    rowClass += " total";

                writer.Write("<tr class='{0}'>", rowClass);

                if (ShowRowLabels)
                {
                    writer.Write("<th class='th-row'>");
                    writer.Write((y + 1).ToString());
                    writer.Write("</th>");
                }

                for (int x = 0; x < Columns.Count; x++)
                {
                    string cellClass = "value";

                    if (x % 2 == 1)
                        cellClass += " alt-col";

                    if (HasTotalCell && x == Columns.Count - 1)
                        cellClass += " total";

                    if (ShowRowLabels || ShowColumnLabels) writer.Write("<td class='{0}' title='x: {1}&#13;y: {2}'>", cellClass, Columns[x], Rows[y]);
                    else writer.Write("<td class='{0}'>", cellClass);

                    _cells[x, y].RenderControl(writer);

                    writer.Write("</td>");
                }

                writer.Write("</tr>");
            }

            writer.Write("</tbody></table>");
        }

        #endregion

        #region Helpers

        private static IList<object> DistinctValues(IEnumerable<ResponseAnalysisCorrelationItem> t, string columnName)
        {
            var distinctValues = new HashSet<object>();

            foreach (var item in t)
            {
                var value = DataBinder.Eval(item, columnName);

                distinctValues.Add(value);
            }

            return distinctValues.ToList();
        }

        private static void ValidateColumn(string columnName, string fieldName)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException(string.Format("{0} field is null.", fieldName));

            if (typeof(ResponseAnalysisCorrelationItem).GetProperty(columnName) == null)
                throw new ArgumentException(string.Format("Column '{0}' does not belong to the table.", columnName));
        }

        #endregion
    }
}