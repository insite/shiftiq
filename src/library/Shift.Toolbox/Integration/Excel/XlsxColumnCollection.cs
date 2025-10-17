using System;
using System.Collections.Generic;

using OfficeOpenXml;

namespace Shift.Toolbox
{
    public class XlsxColumnCollection
    {
        #region Properties

        public int Count => _maxIndex + 1;

        public XlsxColumn this[int index]
        {
            get
            {
                if (index < 0)
                    throw new IndexOutOfRangeException("Invalid column index: " + index);

                if (_columns.ContainsKey(index))
                    return _columns[index];

                var column = new XlsxColumn();

                _columns.Add(index, column);

                if (_maxIndex < index)
                    _maxIndex = index;

                return column;
            }
            set
            {
                if (index < 0)
                    throw new IndexOutOfRangeException("Invalid column index: " + index);

                if (!_columns.ContainsKey(index))
                {
                    var column = new XlsxColumn();

                    _columns.Add(index, column);

                    if (_maxIndex < index)
                        _maxIndex = index;
                }
                else
                {
                    _columns[index] = value;
                }
            }
        }

        #endregion

        #region Fields

        private int _maxIndex = -1;
        private readonly Dictionary<int, XlsxColumn> _columns = new Dictionary<int, XlsxColumn>();

        #endregion

        #region Methods

        internal void Apply(ExcelWorksheet sheet)
        {
            foreach (var kv in _columns)
            {
                var sheetColumn = sheet.Column(kv.Key + 1);

                kv.Value.Apply(sheetColumn);
            }
        }

        #endregion
    }
}
