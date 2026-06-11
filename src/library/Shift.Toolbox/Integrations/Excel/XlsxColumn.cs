using OfficeOpenXml;

namespace Shift.Toolbox
{
    public class XlsxColumn
    {
        #region Properties

        public double? Width { get; set; }

        #endregion

        #region Methods

        internal void Apply(ExcelColumn sheetColumn)
        {
            if (Width.HasValue)
                sheetColumn.Width = Width.Value;
        }

        #endregion
    }
}
