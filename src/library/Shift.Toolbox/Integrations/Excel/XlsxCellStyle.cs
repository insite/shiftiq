using System;
using System.Collections.Generic;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Constant;

using Drawing = System.Drawing;

namespace Shift.Toolbox
{
    public class XlsxCellStyle
    {
        #region Properties

        public HorizontalAlignment? Align { get; set; }

        public XlsxCellVAlign? VAlign { get; set; }

        public bool? IsBold { get; set; }

        public bool? IsUnderline { get; set; }

        public bool? IsItalic { get; set; }

        public bool? WrapText { get; set; }

        public Drawing.Color? BackgroundColor { get; set; }

        public Drawing.Color? FontColor { get; set; }

        public XlsxBorder Border { get; private set; } = new XlsxBorder();

        #endregion

        #region Methods

        internal void Apply(ExcelRange cell)
        {
            if (IsBold.HasValue)
                cell.Style.Font.Bold = IsBold.Value;

            if (IsItalic.HasValue)
                cell.Style.Font.Italic = IsItalic.Value;

            if (IsUnderline.HasValue)
                cell.Style.Font.UnderLine = IsUnderline.Value;

            if (WrapText.HasValue)
                cell.Style.WrapText = WrapText.Value;

            if (VAlign.HasValue)
                cell.Style.VerticalAlignment = GetExcelValue(VAlign.Value);

            if (Align.HasValue)
                cell.Style.HorizontalAlignment = GetExcelValue(Align.Value);

            if (BackgroundColor.HasValue)
            {
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(BackgroundColor.Value);
            }

            if (FontColor.HasValue)
                cell.Style.Font.Color.SetColor(FontColor.Value);

            Border.Apply(cell.Style.Border);
        }

        public XlsxCellStyle Copy()
        {
            return new XlsxCellStyle
            {
                Align = Align,
                VAlign = VAlign,
                IsBold = IsBold,
                IsUnderline = IsUnderline,
                IsItalic = IsItalic,
                WrapText = WrapText,
                BackgroundColor = BackgroundColor,
                FontColor = FontColor,
                Border = Border.Copy()
            };
        }

        #endregion

        #region Helper methods

        private static readonly HashSet<Type> RightAlignType = new HashSet<Type>
        {
            typeof(int),
            typeof(int?),
            typeof(short),
            typeof(short?),
            typeof(decimal),
            typeof(decimal?),
            typeof(double),
            typeof(double?),
            typeof(float),
            typeof(float?)
        };

        private const string DefaultDateFormat = "MMM dd, yyyy";

        private static readonly Dictionary<Type, string> TypeFormats = new Dictionary<Type, string>
        {
            { typeof(DateTime), DefaultDateFormat },
            { typeof(DateTime?), DefaultDateFormat },
            { typeof(DateTimeOffset), DefaultDateFormat },
            { typeof(DateTimeOffset?), DefaultDateFormat }
        };

        public static HorizontalAlignment GetAlign(Type type) =>
            RightAlignType.Contains(type) ? HorizontalAlignment.Right : HorizontalAlignment.Left;

        public static string GetFormat(Type type) => TypeFormats.ContainsKey(type) ? TypeFormats[type] : null;

        public static ExcelHorizontalAlignment GetExcelValue(HorizontalAlignment value)
        {
            switch (value)
            {
                case HorizontalAlignment.Left:
                    return ExcelHorizontalAlignment.Left;
                case HorizontalAlignment.Right:
                    return ExcelHorizontalAlignment.Right;
                case HorizontalAlignment.Center:
                    return ExcelHorizontalAlignment.Center;
                default:
                    throw new ArgumentException(value.ToString());
            }
        }

        public static ExcelVerticalAlignment GetExcelValue(XlsxCellVAlign value) =>
            (ExcelVerticalAlignment)(int)value;

        #endregion
    }
}
