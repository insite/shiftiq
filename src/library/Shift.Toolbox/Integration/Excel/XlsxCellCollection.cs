using System;
using System.Collections.Generic;

using OfficeOpenXml;

namespace Shift.Toolbox
{
    public class XlsxCellCollection
    {
        #region Classes

        public struct XlsxCellKey
        {
            #region Properties

            public int Column { get; }
            public int Row { get; }

            #endregion

            #region Construction

            public XlsxCellKey(int column, int row)
            {
                if (column < 0)
                    throw new IndexOutOfRangeException("Invalid column number: " + column);

                if (row < 0)
                    throw new IndexOutOfRangeException("Invalid row number: " + row);

                Column = column;
                Row = row;
            }

            #endregion

            #region Methods

            public override int GetHashCode()
            {
                var hash = 17;

                hash = hash * 23 + Column.GetHashCode();
                hash = hash * 23 + Row.GetHashCode();

                return hash;
            }

            public override bool Equals(object obj) => obj is XlsxCellKey key && key.Column == Column && key.Row == Row;

            #endregion
        }

        #endregion

        #region Properties

        public XlsxCellBase this[int col, int row]
        {
            get
            {
                var key = new XlsxCellKey(col, row);

                return _cellsDict.ContainsKey(key) ? _cellsDict[key] : null;
            }
        }

        #endregion

        #region Fields

        private int _maxRow;
        private int _maxCol;
        private readonly List<XlsxCellBase> _cellList = new List<XlsxCellBase>();
        private readonly Dictionary<XlsxCellKey, XlsxCellBase> _cellsDict = new Dictionary<XlsxCellKey, XlsxCellBase>();

        #endregion

        #region Methods

        public void Add(XlsxCellBase cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));

            var keys = new List<XlsxCellKey>();
            var colEndIndex = cell.ColumnIndex + cell.ColumnSpan - 1;
            var rowEndIndex = cell.RowIndex + cell.RowSpan - 1;

            for (var col = cell.ColumnIndex; col <= colEndIndex; col++)
            {
                for (var row = cell.RowIndex; row <= rowEndIndex; row++)
                {
                    var key = new XlsxCellKey(col, row);

                    if (_cellsDict.ContainsKey(key))
                        throw new InvalidOperationException($"Cell already exists: {cell.ExcelCode}");

                    keys.Add(key);
                }
            }

            foreach (var key in keys)
                _cellsDict.Add(key, cell);

            _cellList.Add(cell);

            if (_maxCol < colEndIndex)
                _maxCol = colEndIndex;

            if (_maxRow < rowEndIndex)
                _maxRow = rowEndIndex;
        }

        public IEnumerable<XlsxCellBase> GetRow(int col)
        {
            for (var row = 1; row <= _maxRow; row++)
            {
                var key = new XlsxCellKey(col, row);

                yield return _cellsDict.ContainsKey(key) ? _cellsDict[key] : null;
            }
        }

        public IEnumerable<XlsxCellBase> GetColumn(int row)
        {
            for (var col = 1; col <= _maxCol; col++)
            {
                var key = new XlsxCellKey(col, row);

                yield return _cellsDict.ContainsKey(key) ? _cellsDict[key] : null;
            }
        }

        internal void Apply(ExcelWorksheet sheet)
        {
            foreach (var cell in _cellList)
            {
                var sheetCell = sheet.Cells[cell.RowIndex + 1, cell.ColumnIndex + 1, cell.RowIndex + cell.RowSpan, cell.ColumnIndex + cell.ColumnSpan];

                cell.Apply(sheetCell);
            }
        }

        #endregion
    }
}
