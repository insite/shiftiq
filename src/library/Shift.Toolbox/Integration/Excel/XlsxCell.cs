using OfficeOpenXml;

namespace Shift.Toolbox
{
    public class XlsxCell : XlsxCellBase
    {
        #region Properties

        public string Format { get; set; }

        public bool WrapText { get; set; }

        public object Value { get; set; }

        #endregion

        #region Construction

        public XlsxCell(int col, int row, int colSpan = 1, int rowSpan = 1)
            : base(col, row, colSpan, rowSpan)
        {

        }

        #endregion

        protected override void ApplyInternal(ExcelRange cell)
        {
            if (!string.IsNullOrEmpty(Format))
                cell.Style.Numberformat.Format = Format;

            cell.Style.WrapText = WrapText;

            cell.Value = Value;
        }
    }
}