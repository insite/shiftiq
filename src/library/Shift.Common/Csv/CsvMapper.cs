using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    public static class CsvMapper
    {
        [Serializable]
        public abstract class Column
        {
            #region Classes

            [Serializable]
            public class String : Column
            {
                public bool AllowNull { get; }
                public int MaxLength { get; }

                public String(string name, string label, bool required, bool allowNull, int maxLength = -1)
                    : base(name, label, required)
                {
                    AllowNull = allowNull;
                    MaxLength = maxLength;
                }

                public override void Read(Reader data)
                {
                    while (data.NextRow())
                    {
                        var value = data.GetValue().NullIfEmpty();

                        if (value == null)
                        {
                            if (!AllowNull)
                                data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) cannot be empty", data.RowIndex + 1, Label, data.ColIndex + 1);

                            continue;
                        }

                        if (MaxLength > 0 && value.Length > MaxLength)
                        {
                            data.Messages.AddWarning("Row {0}: the string value in column '{1}' ({2}) has exceeded the maximum length of {3}. The value is truncated", data.RowIndex + 1, Label, data.ColIndex + 1, MaxLength);
                            value = value.Substring(0, MaxLength);
                        }

                        data.SetValue(value);
                    }
                }
            }

            [Serializable]
            public class DateTimeOffset : Column
            {
                public bool AllowNull { get; }

                public DateTimeOffset(string name, string label, bool required, bool allowNull)
                    : base(name, label, required)
                {
                    AllowNull = allowNull;
                }

                public override void Read(Reader data)
                {
                    while (data.NextRow())
                    {
                        var value = data.GetValue().NullIfEmpty();

                        if (value == null)
                        {
                            if (!AllowNull)
                                data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) cannot be empty", data.RowIndex + 1, Label, data.ColIndex + 1);

                            continue;
                        }

                        if (!System.DateTimeOffset.TryParse(value, out var date))
                        {
                            data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) is not recognized as a valid date/time", data.RowIndex + 1, Label, data.ColIndex + 1);
                            continue;
                        }

                        data.SetValue(date);
                    }
                }
            }

            [Serializable]
            public class Boolean : Column
            {
                public bool AllowNull { get; }

                public Boolean(string name, string label, bool required, bool allowNull)
                    : base(name, label, required)
                {
                    AllowNull = allowNull;
                }

                public override void Read(Reader data)
                {
                    while (data.NextRow())
                    {
                        var value = data.GetValue().NullIfEmpty();

                        if (value == null)
                        {
                            if (!AllowNull)
                                data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) cannot be empty", data.RowIndex + 1, Label, data.ColIndex + 1);

                            continue;
                        }

                        value = value.ToLower();

                        var isTrue = value == "true" || value == "yes" || value == "y" || value == "1";
                        var isFalse = value == "false" || value == "no" || value == "n" || value == "0";

                        if (!isTrue && !isFalse)
                        {
                            data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) is not recognized as a valid boolean", data.RowIndex + 1, Label, data.ColIndex + 1);
                            continue;
                        }

                        data.SetValue(isTrue);
                    }
                }
            }

            [Serializable]
            public class Integer : Column
            {
                public bool AllowNull { get; }
                public int? MinValue { get; }
                public int? MaxValue { get; }

                public Integer(string name, string label, bool required, bool allowNull, int? minValue = null, int? maxValue = null)
                    : base(name, label, required)
                {
                    AllowNull = allowNull;
                    MinValue = minValue;
                    MaxValue = maxValue;
                }

                public override void Read(Reader data)
                {
                    while (data.NextRow())
                    {
                        var value = data.GetValue().NullIfEmpty();

                        if (value == null)
                        {
                            if (!AllowNull)
                                data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) cannot be empty", data.RowIndex + 1, Label, data.ColIndex + 1);

                            continue;
                        }

                        if (!int.TryParse(value, out var intVal))
                        {
                            data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) is not recognized as a valid integer", data.RowIndex + 1, Label, data.ColIndex + 1);
                            continue;
                        }

                        if (MinValue.HasValue && intVal < MinValue.Value)
                        {
                            data.Messages.AddWarning("Row {0}: the integer value in column '{1}' ({2}) exceeds the minimum value ({3}). The value is set to column minimum value", data.RowIndex + 1, Label, data.ColIndex + 1, MinValue.Value);
                            intVal = MinValue.Value;
                        }

                        if (MaxValue.HasValue && intVal > MaxValue.Value)
                        {
                            data.Messages.AddWarning("Row {0}: the integer value in column '{1}' ({2}) exceeds the maximum value ({3}). The value is set to column maximum value", data.RowIndex + 1, Label, data.ColIndex + 1, MaxValue.Value);
                            intVal = MaxValue.Value;
                        }

                        data.SetValue(intVal);
                    }
                }
            }

            [Serializable]
            public class Decimal : Column
            {
                public bool AllowNull { get; }
                public decimal? MinValue { get; }
                public decimal? MaxValue { get; }

                public Decimal(string name, string label, bool required, bool allowNull, decimal? minValue = null, decimal? maxValue = null)
                    : base(name, label, required)
                {
                    AllowNull = allowNull;
                    MinValue = minValue;
                    MaxValue = maxValue;
                }

                public override void Read(Reader data)
                {
                    while (data.NextRow())
                    {
                        var value = data.GetValue().NullIfEmpty();

                        if (value == null)
                        {
                            if (!AllowNull)
                                data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) cannot be empty", data.RowIndex + 1, Label, data.ColIndex + 1);

                            continue;
                        }

                        if (!decimal.TryParse(value, out var decVal))
                        {
                            data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) is not recognized as a valid decimal", data.RowIndex + 1, Label, data.ColIndex + 1);
                            continue;
                        }

                        if (MinValue.HasValue && decVal < MinValue.Value)
                        {
                            data.Messages.AddWarning("Row {0}: the decimal value in column '{1}' ({2}) exceeds the minimum value ({3}). The value is set to column minimum value", data.RowIndex + 1, Label, data.ColIndex + 1, MinValue.Value);
                            decVal = MinValue.Value;
                        }

                        if (MaxValue.HasValue && decVal > MaxValue.Value)
                        {
                            data.Messages.AddWarning("Row {0}: the decimal value in column '{1}' ({2}) exceeds the maximum value ({3}). The value is set to column maximum value", data.RowIndex + 1, Label, data.ColIndex + 1, MaxValue.Value);
                            decVal = MaxValue.Value;
                        }

                        data.SetValue(decVal);
                    }
                }
            }

            [Serializable]
            public class StringEnum : Column
            {
                public bool AllowNull { get; }
                public Dictionary<string, string> Values { get; }

                public StringEnum(string name, string label, bool required, bool allowNull, IEnumerable<string> values)
                    : base(name, label, required)
                {
                    AllowNull = allowNull;
                    Values = values.ToDictionary(x => StringHelper.RemoveNonAlphanumericCharacters(x), x => x, StringComparer.OrdinalIgnoreCase);
                }

                public override void Read(Reader data)
                {
                    while (data.NextRow())
                    {
                        var value = data.GetValue().NullIfEmpty();

                        if (value == null)
                        {
                            if (!AllowNull)
                                data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) cannot be empty", data.RowIndex + 1, Label, data.ColIndex + 1);

                            continue;
                        }

                        value = StringHelper.RemoveNonAlphanumericCharacters(value);

                        if (!Values.TryGetValue(value, out var validValue))
                        {
                            data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) is not recognized as a valid value", data.RowIndex + 1, Label, data.ColIndex + 1);
                            continue;
                        }

                        data.SetValue(validValue);
                    }
                }
            }

            [Serializable]
            public class LanguageCode : Column
            {
                public LanguageCode(string name, string label, bool required)
                    : base(name, label, required)
                {
                }

                public override void Read(Reader data)
                {
                    while (data.NextRow())
                    {
                        var value = data.GetValue().NullIfEmpty();

                        if (value == null)
                            continue;

                        if (!Language.CodeExists(value))
                        {
                            data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) is not recognized as a valid language code", data.RowIndex + 1, Label, data.ColIndex + 1);
                            continue;
                        }

                        data.SetValue(value);
                    }
                }
            }

            #endregion

            #region Properties

            public string Name { get; }
            public string Label { get; }
            public bool Required { get; }

            #endregion

            #region Construction 

            protected Column(string name, string label, bool required)
            {
                Name = name;
                Label = label;
                Required = required;
            }

            #endregion

            #region Methods

            public abstract void Read(Reader data);

            #endregion
        }

        public interface ICsvReader
        {
            int RowCount { get; }

            int ColumnCount { get; }

            MessageBuilder Messages { get; }

            T GetValue<T>(int row, string column);
        }

        public class Reader
        {
            #region Classes

            public sealed class ColumnData
            {
                public Column Column { get; }

                public int DataIndex { get; }

                public ColumnData(Column column, int dataIndex)
                {
                    Column = column;
                    DataIndex = dataIndex;
                }
            }

            private sealed class Data : ICsvReader
            {
                #region Properties

                public int RowCount => _dataArray.Length;
                public int ColumnCount => _nameMapping.Count;
                public MessageBuilder Messages => _messages;

                #endregion

                #region Fields

                private readonly object[][] _dataArray;
                private readonly MessageBuilder _messages;
                private readonly Dictionary<string, int> _nameMapping;
                private readonly HashSet<string> _validNames;

                #endregion

                #region Construction

                public Data(object[][] data, MessageBuilder messages, ColumnData[] columns, IEnumerable<string> columnNames)
                {
                    _dataArray = data;
                    _messages = messages;
                    _validNames = new HashSet<string>(columnNames, StringComparer.OrdinalIgnoreCase);

                    _nameMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    for (var i = 0; i < columns.Length; i++)
                        _nameMapping.Add(columns[i].Column.Name, i);
                }

                #endregion

                #region Methods

                public T GetValue<T>(int row, string column)
                {
                    if (row < 0 || row >= _dataArray.Length)
                        throw new ArgumentOutOfRangeException(nameof(row));

                    if (_nameMapping.TryGetValue(column, out var colIndex))
                        return (T)_dataArray[row][colIndex];

                    if (_validNames.Contains(column))
                        return default(T);

                    throw new ArgumentOutOfRangeException(nameof(column));
                }

                #endregion
            }

            #endregion

            #region Properties

            public int RowIndex => _inputRowIndex;

            public int ColIndex => _inputColIndex;

            public MessageBuilder Messages { get; }

            #endregion

            #region Fields

            private int _inputRowIndex;
            private int _inputColIndex;
            private int _outputRowIndex;
            private int _outputColIndex;
            private string[][] _input;
            private object[][] _output;

            #endregion

            #region Construction

            private Reader()
            {
                Messages = new MessageBuilder();
            }

            #endregion

            #region Methods

            public static ICsvReader Read(string[][] data, IEnumerable<string> columnNames, ColumnData[] readColumns)
            {
                var result = new Reader
                {
                    _input = data,
                    _output = new object[data.Length - 1][]
                };

                for (var i = 0; i < result._output.Length; i++)
                    result._output[i] = new object[readColumns.Length];

                for (var cIndex = 0; cIndex < readColumns.Length; cIndex++)
                {
                    var column = readColumns[cIndex];

                    result._inputRowIndex = 0;
                    result._inputColIndex = column.DataIndex;
                    result._outputRowIndex = -1;
                    result._outputColIndex = cIndex;

                    column.Column.Read(result);
                }

                return new Data(result._output, result.Messages, readColumns, columnNames);
            }

            public bool NextRow()
            {
                _inputRowIndex++;
                _outputRowIndex++;

                return _inputRowIndex < _input.Length;
            }

            public string GetValue() => _input[_inputRowIndex][_inputColIndex];

            public void SetValue(object value) => _output[_outputRowIndex][_outputColIndex] = value;

            #endregion
        }
    }
}
