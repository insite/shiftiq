using System.Collections.Generic;

using OfficeOpenXml;

namespace Shift.Toolbox
{
    public class XlsxCellRichText : XlsxCellBase
    {
        #region Classes

        public class TextItem
        {
            public bool IsBold { get; set; }
            public string Text { get; set; }
        }

        #endregion

        #region Properties

        public IReadOnlyList<TextItem> Items => _items;

        #endregion

        #region Fields

        private readonly List<TextItem> _items;

        #endregion

        #region Construction

        public XlsxCellRichText(int colIndex, int rowIndex, int colSpan = 1, int rowSpan = 1)
            : base(colIndex, rowIndex, colSpan, rowSpan)
        {
            _items = new List<TextItem>();
        }

        #endregion

        public TextItem AddText(string value, bool isBold = false)
        {
            var result = new TextItem
            {
                Text = value,
                IsBold = isBold
            };

            _items.Add(result);

            return result;
        }

        protected override void ApplyInternal(ExcelRange cell)
        {
            foreach (var value in _items)
            {
                var richText = cell.RichText.Add(value.Text);
                richText.Bold = value.IsBold;
            }
        }
    }
}
