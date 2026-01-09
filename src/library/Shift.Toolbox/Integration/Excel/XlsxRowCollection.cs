using System;
using System.Collections.Generic;

using OfficeOpenXml;

namespace Shift.Toolbox
{
    public class XlsxRowCollection
    {
        #region Properties

        public int Count => _maxIndex + 1;

        public XlsxRow this[int index]
        {
            get
            {
                if (index < 0)
                    throw new IndexOutOfRangeException("Invalid row index: " + index);

                if (_rows.ContainsKey(index))
                    return _rows[index];

                var row = new XlsxRow();

                _rows.Add(index, row);

                if (_maxIndex < index)
                    _maxIndex = index;

                return row;
            }
            set
            {
                if (index < 0)
                    throw new IndexOutOfRangeException("Invalid row index: " + index);

                if (!_rows.ContainsKey(index))
                {
                    var row = new XlsxRow();

                    _rows.Add(index, row);

                    if (_maxIndex < index)
                        _maxIndex = index;
                }
                else
                {
                    _rows[index] = value;
                }
            }
        }

        #endregion

        #region Fields

        private int _maxIndex = -1;
        private readonly Dictionary<int, XlsxRow> _rows = new Dictionary<int, XlsxRow>();

        #endregion

        #region Methods

        internal void Apply(ExcelWorksheet sheet)
        {
            foreach (var kv in _rows)
            {
                var sheetRow = sheet.Row(kv.Key + 1);

                kv.Value.Apply(sheetRow);
            }
        }

        #endregion
    }
}
