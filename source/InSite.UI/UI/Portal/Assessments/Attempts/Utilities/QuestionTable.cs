using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Banks;

using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    public class QuestionTable
    {
        #region Classes

        public class CellData
        {
            public string Text { get; }
            public HorizontalAlignment Alignment { get; set; }
            public string CssClass { get; set; }

            public CellData(string text)
            {
                Text = text;
                Alignment = HorizontalAlignment.Left;
                CssClass = string.Empty;
            }

            public CellData(string text, HorizontalAlignment alignment, string cssClass)
            {
                Text = text;
                Alignment = alignment;
                CssClass = cssClass;
            }
        }

        #endregion

        #region Properties

        public int RowsCount => _data.Length;

        public int ColumnsCount { get; private set; }

        #endregion

        #region Fields

        private string[][] _data;
        private OptionColumn[] _layoutColumns;

        #endregion

        #region Construction

        private QuestionTable()
        {
            ColumnsCount = -1;
        }

        #endregion

        #region Initialization

        public static QuestionTable Build(IEnumerable<OptionColumn> layoutColumns, IEnumerable<string> options)
        {
            if (layoutColumns == null && options != null)
            {
                layoutColumns = new List<OptionColumn>();
            }

            var table = new QuestionTable();
            table._layoutColumns = layoutColumns.ToArray();
            table.ColumnsCount = layoutColumns.Count();

            var rowsList = new List<List<string>>();

            foreach (var text in options)
            {
                var cells = !string.IsNullOrEmpty(text) ? new List<string>(text.Split('|')) : new List<string>();
                if (cells.Count > table.ColumnsCount)
                    table.ColumnsCount = cells.Count;

                rowsList.Add(cells);
            }

            var rowsArray = table._data = new string[rowsList.Count][];

            for (var i = 0; i < rowsArray.Length; i++)
            {
                var colsList = rowsList[i];
                var colsArray = rowsArray[i] = new string[table.ColumnsCount];

                var j = 0;

                for (; j < colsList.Count; j++)
                    colsArray[j] = colsList[j].Trim();

                for (; j < table.ColumnsCount; j++)
                    colsArray[j] = string.Empty;
            }

            return table;
        }

        #endregion

        #region Methods

        public CellData[] GetHeader()
        {
            var result = new CellData[ColumnsCount];

            for (var i = 0; i < ColumnsCount; i++)
                result[i] = GetHeaderCell(i);

            return result;
        }

        public CellData GetHeader(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= ColumnsCount)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));

            return GetHeaderCell(columnIndex);
        }

        public CellData[][] GetBody()
        {
            var result = new CellData[RowsCount][];

            for (var x = 0; x < _data.Length; x++)
            {
                var columns = result[x] = new CellData[_data[x].Length];

                for (var y = 0; y < columns.Length; y++)
                {
                    columns[y] = GetBodyCell(x, y);
                }
            }

            return result;
        }

        public CellData GetBody(int rowIndex, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= ColumnsCount)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));

            if (rowIndex < 0 || rowIndex >= _data.Length)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));

            return GetBodyCell(rowIndex, columnIndex);
        }

        private CellData GetHeaderCell(int index)
        {
            if (index >= _layoutColumns.Length)
                return new CellData(string.Empty, HorizontalAlignment.Left, string.Empty);

            var column = _layoutColumns[index];
            return new CellData(column.Content.Title?.Default, column.Alignment, column.CssClass);
        }

        private CellData GetBodyCell(int rowIndex, int columnIndex)
        {
            var text = _data[rowIndex][columnIndex];

            if (columnIndex >= _layoutColumns.Length)
                return new CellData(text, HorizontalAlignment.Left, string.Empty);

            var column = _layoutColumns[columnIndex];
            return new CellData(text, column.Alignment, column.CssClass);
        }

        #endregion
    }
}