using System;

using Shift.Common;
using Shift.Constant;

namespace Shift.Toolbox
{
    internal class XlsxExportMappingInfo
    {
        #region Properties

        public string PropertyName { get; }

        public string ColumnTitle { get; }

        public string ColumnFormat { get; set; }

        public double? ColumnWidth { get; set; }

        public HorizontalAlignment? HorizontalAlignment { get; set; }

        #endregion

        #region Constructor

        public XlsxExportMappingInfo(string property, string title, string format, double? width, HorizontalAlignment? horizontalAlignment)
        {
            if (property.IsEmpty())
                throw new ArgumentNullException(nameof(property));

            PropertyName = property;
            ColumnTitle = title;
            ColumnFormat = format;
            ColumnWidth = width;
            HorizontalAlignment = horizontalAlignment;
        }

        #endregion
    }
}
