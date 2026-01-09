using OfficeOpenXml;

namespace Shift.Toolbox
{
    public class XlsxRow
    {
        #region Properties

        public double? Height { get; set; }

        #endregion

        #region Methods

        internal void Apply(ExcelRow sheetRow)
        {
            if (Height.HasValue)
                sheetRow.Height = Height.Value;
        }

        #endregion
    }
}
