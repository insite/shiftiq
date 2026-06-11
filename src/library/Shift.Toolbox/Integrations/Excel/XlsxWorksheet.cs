using OfficeOpenXml;

namespace Shift.Toolbox
{
    public class XlsxWorksheet
    {
        #region Properties

        public string Name { get; }

        public XlsxColumnCollection Columns { get; } = new XlsxColumnCollection();

        public XlsxRowCollection Rows { get; } = new XlsxRowCollection();

        public XlsxCellCollection Cells { get; } = new XlsxCellCollection();

        public bool WrapText { get; set; }

        #endregion

        #region Construction

        public XlsxWorksheet(string name)
        {
            Name = name;
        }

        #endregion

        #region Methods

        public byte[] GetBytes() => GetBytes(this);

        public static byte[] GetBytes(params XlsxWorksheet[] xlsxSheets)
        {
            using (var excel = new ExcelPackage())
            {
                foreach (var xlsxSheet in xlsxSheets)
                {
                    var sheet = excel.Workbook.Worksheets.Add(xlsxSheet.Name);
                    sheet.Cells.Style.WrapText = xlsxSheet.WrapText;

                    xlsxSheet.Columns.Apply(sheet);
                    xlsxSheet.Rows.Apply(sheet);
                    xlsxSheet.Cells.Apply(sheet);
                }

                return excel.GetAsByteArray();
            }
        }

        #endregion
    }
}
