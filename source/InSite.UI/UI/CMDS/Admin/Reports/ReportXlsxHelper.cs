using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;

using InSite.Common.Web;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;
using Shift.Toolbox;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public static class ReportXlsxHelper
    {
        public static void ExportToXlsx(string name, DataTable data)
        {
            if (data.Rows.Count == 0)
                return;

            var bytes = GetXlsxBytes(name, data);

            ExportToXlsx(name, bytes);
        }

        public static void ExportToXlsx(string name, IList data)
        {
            if (data.Count == 0)
                return;

            var bytes = GetXlsxBytes(name, data);

            ExportToXlsx(name, bytes);
        }

        public static void Export(XlsxWorksheet sheet) =>
            ExportToXlsx(sheet.Name, sheet.GetBytes());

        public static void Export(string name, ExcelPackage package) =>
            ExportToXlsx(name, package.GetAsByteArray());

        public static void ExportToXlsx(string name, byte[] bytes, string fileName = null)
        {
            if (bytes == null)
                return;

            // var filename = string.Format("{0}-{1:yyyyMMdd}-{1:HHmmss}", StringHelper.ReplaceNonAlphanumericCharacters(name, '-', false), DateTime.UtcNow);
            var filename = fileName ?? StringHelper.Sanitize(name, '-', false);

            HttpContext.Current.Response.SendFile(filename, "xlsx", bytes);
        }

        private static byte[] GetXlsxBytes(string name, IList data)
        {
            var properties = TypeDescriptor.GetProperties(data[0]);
            if (properties.Count == 0)
                return null;

            var lastProperty = properties[properties.Count - 1];
            var helper = new XlsxExportHelper();

            foreach (PropertyDescriptor property in properties)
            {
                var align = XlsxCellStyle.GetAlign(property.PropertyType);
                var format = XlsxCellStyle.GetFormat(property.PropertyType);

                helper.Map(property.Name, property.Name, format, 30, align);
            }

            return helper.GetXlsxBytes(data, name);
        }

        private static byte[] GetXlsxBytes(string name, DataTable data)
        {
            var helper = new XlsxExportHelper();

            foreach (DataColumn column in data.Columns)
            {
                var align = XlsxCellStyle.GetAlign(column.DataType);
                var format = XlsxCellStyle.GetFormat(column.DataType);

                helper.Map(column.ColumnName, column.ColumnName, format, 30, align);
            }

            return helper.GetXlsxBytes(data, name);
        }

        #region Helper methods

        public static void Set(this ExcelBorderItem border, ExcelBorderStyle style, Color color)
        {
            border.Style = style;
            border.Color.SetColor(color);
        }

        #endregion
    }
}