using OfficeOpenXml.Style;

using Shift.Constant;

namespace Shift.Toolbox
{
    public class XlsxBorderItem
    {
        public XlsxBorderStyle Style { get; set; } = XlsxBorderStyle.Hair;
        public System.Drawing.Color Color { get; set; } = System.Drawing.Color.Silver;

        internal void Apply(ExcelBorderItem item)
        {
            item.Style = (ExcelBorderStyle)Style;

            if(Style != XlsxBorderStyle.None)
                item.Color.SetColor(Color);
        }
    }
}
