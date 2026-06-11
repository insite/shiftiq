using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;
using Shift.Constant;

namespace Shift.Toolbox
{
    public class XlsxExportHelper
    {
        #region Constant

        public const string HeaderStyleName = "Header";

        #endregion

        #region Fields

        private TimeZoneInfo _timeZone;

        private List<XlsxExportMappingInfo> _mappings;
        private bool _isRebuildProcessor = true;

        private XlsxExportSourceProcessor _sourceProcessor;

        #endregion

        #region Constructor

        public XlsxExportHelper() : this(TimeZoneInfo.Utc)
        {

        }

        public XlsxExportHelper(TimeZoneInfo timeZone)
        {
            _timeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
            _mappings = new List<XlsxExportMappingInfo>();
        }

        #endregion

        #region Initialization

        private void BuildMapping(IEnumerable source)
        {
            if (!_isRebuildProcessor)
                return;

            if (_mappings.Count == 0)
                throw new InvalidOperationException("No mappings defined for this export.");

            var dataSample = source?.Cast<object>().FirstOrDefault();
            if (dataSample == null)
                throw new ArgumentException("The data source is empty");

            _sourceProcessor = XlsxExportSourceProcessor.Build(this, dataSample, _mappings);
            _isRebuildProcessor = false;
        }

        #endregion

        #region Public methods

        public void Map(string property, string name) =>
            Map(property, name, null, null, null);

        public void Map(string property, string name, string format) =>
            Map(property, name, format, null, null);

        public void Map(string property, string name, double width) =>
            Map(property, name, null, width, null);

        public void Map(string property, string name, HorizontalAlignment alignment) =>
            Map(property, name, null, null, alignment);

        public void Map(string property, string name, double width, HorizontalAlignment alignment) =>
            Map(property, name, null, width, alignment);

        public void Map(string property, string name, string format, double? width, HorizontalAlignment? alignment)
        {
            var item = new XlsxExportMappingInfo(property, name, format, width, alignment);

            _mappings.Add(item);

            _isRebuildProcessor = true;
        }

        public void RebuildMapping()
        {
            _isRebuildProcessor = true;
        }

        public byte[] GetXlsxBytes(Action<ExcelPackage> build)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                using (var excel = new ExcelPackage(stream))
                {
                    var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                    defaultStyle.Font.Name = "Calibri";
                    defaultStyle.Font.Size = 11;
                    defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;

                    var headerStyle = excel.Workbook.Styles.CreateNamedStyle(HeaderStyleName);
                    headerStyle.Style.Font.Bold = true;
                    headerStyle.Style.QuotePrefix = true;
                    headerStyle.Style.Numberformat.Format = "";

                    build(excel);

                    excel.Save();

                    result = new byte[stream.Length];

                    stream.Position = 0;
                    stream.Read(result, 0, result.Length);
                }
            }

            return result;
        }

        public byte[] GetXlsxBytes(IListSource source, string sheetName, bool renderHeader = true, bool autoColumnWidth = true)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return GetXlsxBytes(source.GetList(), sheetName, renderHeader, autoColumnWidth);
        }

        public byte[] GetXlsxBytes(IEnumerable source, string sheetName, bool renderHeader = true, bool autoColumnWidth = true)
        {
            return GetXlsxBytes(excel =>
            {
                var sheet = excel.Workbook.Worksheets.Add(sheetName);
                sheet.Cells.Style.WrapText = true;

                ApplyColumnFormatting(sheet, source, 1);

                if (renderHeader)
                {
                    InsertHeader(sheet, 1, 1, false);
                    InsertData(sheet, source, 2, 1, false);
                }
                else
                {
                    InsertData(sheet, source, 1, 1, false);
                }

                ApplyColumnWidth(sheet, 1, autoColumnWidth);
            });
        }

        public void InsertHeader(ExcelWorksheet sheet, int startRow, int startColumn, bool applyCellsFormatting = true)
        {
            sheet.Cells[startRow, startColumn].LoadFromArrays(new[]
            {
                _mappings.Select(x => x.ColumnTitle).ToArray()
            });
            sheet.Cells[startRow, startColumn, startRow, startColumn + _mappings.Count - 1].StyleName = HeaderStyleName;

            if (!applyCellsFormatting)
                return;

            for (var i = 0; i < _mappings.Count; i++)
            {
                var map = _mappings[i];
                var cells = sheet.Cells[startRow, startColumn + i];

                if (map.HorizontalAlignment.HasValue)
                    cells.Style.HorizontalAlignment = GetExcelAlign(map.HorizontalAlignment.Value);
            }
        }

        public int InsertData(ExcelWorksheet sheet, IEnumerable source, int startRow, int startColumn, bool applyCellsFormatting = true)
        {
            BuildMapping(source);

            var dataItems = _sourceProcessor.Map(source);
            if (dataItems.Length == 0)
                throw new ArgumentException("The data source is empty");

            sheet.Cells[startRow, startColumn].LoadFromArrays(dataItems);

            if (applyCellsFormatting)
            {
                for (var i = 0; i < _mappings.Count; i++)
                {
                    var map = _mappings[i];
                    var type = _sourceProcessor.GetColumnType(i);
                    var cells = sheet.Cells[startRow, startColumn + i, startRow + dataItems.Length - 1, startColumn + i];

                    var format = map.ColumnFormat ?? GetColumnDefaultFormat(type);
                    if (format.IsNotEmpty())
                        cells.Style.Numberformat.Format = format;

                    if (map.HorizontalAlignment.HasValue)
                        cells.Style.HorizontalAlignment = GetExcelAlign(map.HorizontalAlignment.Value);

                    if (type == typeof(string))
                        cells.Style.QuotePrefix = true;
                }
            }

            return dataItems.Length;
        }

        public void ApplyColumnFormatting(ExcelWorksheet sheet, IEnumerable source, int startColumn)
        {
            BuildMapping(source);

            for (var i = 0; i < _mappings.Count; i++)
            {
                var map = _mappings[i];
                var type = _sourceProcessor.GetColumnType(i);
                var col = sheet.Column(startColumn + i);

                var format = map.ColumnFormat ?? GetColumnDefaultFormat(type);
                if (format.IsNotEmpty())
                    col.Style.Numberformat.Format = format;

                if (map.HorizontalAlignment.HasValue)
                    col.Style.HorizontalAlignment = GetExcelAlign(map.HorizontalAlignment.Value);

                if (type == typeof(string))
                    col.Style.QuotePrefix = true;
            }
        }

        public void ApplyColumnWidth(ExcelWorksheet sheet, int startColumn, bool autoFit)
        {
            for (var i = 0; i < _mappings.Count; i++)
            {
                var map = _mappings[i];
                var col = sheet.Column(startColumn + i);

                if (map.ColumnWidth.HasValue)
                    col.Width = map.ColumnWidth.Value;
                else if (autoFit)
                    col.AutoFit();
            }
        }

        #endregion

        #region Helper methods

        private object GetExportDate(DateTimeOffset? value)
        {
            return value.HasValue
                ? TimeZoneInfo.ConvertTime(value.Value, _timeZone).DateTime
                : (object)null;
        }

        private object GetRowValue(object rowValue)
        {
            if (rowValue == DBNull.Value || rowValue == null)
                return null;

            if (rowValue is DateTimeOffset dto)
                return GetExportDate(dto);

            return rowValue;
        }

        private static string GetColumnDefaultFormat(Type type)
        {
            if (type == typeof(DateTimeOffset) || type == typeof(DateTime))
                return "yyyy-mm-dd";

            return null;
        }

        private static ExcelHorizontalAlignment GetExcelAlign(HorizontalAlignment align)
        {
            switch (align)
            {
                case HorizontalAlignment.Left:
                    return ExcelHorizontalAlignment.Left;
                case HorizontalAlignment.Right:
                    return ExcelHorizontalAlignment.Right;
                case HorizontalAlignment.Center:
                    return ExcelHorizontalAlignment.Center;
                default:
                    throw new ArgumentException(align.ToString());
            }
        }

        #endregion
    }
}