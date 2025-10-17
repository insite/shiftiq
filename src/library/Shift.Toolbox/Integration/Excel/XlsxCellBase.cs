using System;

using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Shift.Toolbox
{
    public abstract class XlsxCellBase
    {
        #region Properties

        public int ColumnIndex { get; }
        public int RowIndex { get; }
        public int ColumnSpan { get; }
        public int RowSpan { get; }

        public XlsxCellStyle Style { get; set; }

        public string ExcelCode => ColumnSpan > 1 || RowSpan > 1
            ? $"{GetColumnCode(ColumnIndex)}{RowIndex + 1:f0}:{GetColumnCode(ColumnIndex + ColumnSpan - 1)}{RowIndex + RowSpan:f0}"
            : $"{GetColumnCode(ColumnIndex)}{RowIndex + 1:f0}";

        #endregion

        #region Construction

        protected XlsxCellBase(int colIndex, int rowIndex, int colSpan, int rowSpan)
        {
            if (colIndex < 0)
                throw new IndexOutOfRangeException("Invalid column index: " + colIndex);

            if (rowIndex < 0)
                throw new IndexOutOfRangeException("Invalid row index: " + rowIndex);

            if (colSpan < 1)
                throw new IndexOutOfRangeException("Invalid column span: " + colSpan);

            if (rowSpan < 1)
                throw new IndexOutOfRangeException("Invalid row span: " + rowSpan);

            ColumnIndex = colIndex;
            RowIndex = rowIndex;
            ColumnSpan = colSpan;
            RowSpan = rowSpan;
        }

        #endregion

        #region Methods

        internal void Apply(ExcelRange cell)
        {
            if (ColumnSpan > 1 || RowSpan > 1)
                cell.Merge = true;

            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            Style?.Apply(cell);

            ApplyInternal(cell);
        }

        protected abstract void ApplyInternal(ExcelRange cell);

        public static string GetColumnCode(int index)
        {
            const int numBase = 26;

            var name = string.Empty;

            while (index >= 0)
            {
                name = (char)('A' + index % numBase) + name;
                index = index / numBase - 1;
            }

            return name;
        }

        #endregion
    }
}
