using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

using Shift.Constant;

namespace Shift.Common
{
    public sealed class DataRowHelper
    {
        private readonly DataRow _row;

        public DataRowHelper(DataRow row)
        {
            _row = row;
        }

        public bool GetBoolean(string columnName)
        {
            object value = GetObject(columnName);

            switch (value)
            {
                case null:
                    return false;
                case bool b:
                    return b;
                case int i:
                    return i == 1;
                case string text:
                    return ValueConverter.ToBoolean(text);
            }

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public bool? GetBooleanNullable(string columnName)
        {
            object value = GetObject(columnName);

            return value == null ? (bool?)null : GetBoolean(columnName);
        }

        public byte GetByte(string columnName)
        {
            object value = GetObject(columnName);

            if (value is byte)
                return (byte)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public byte? GetByteNullable(string columnName)
        {
            object value = GetObject(columnName);

            if (value == null)
                return null;

            if (value is byte)
                return (byte)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public byte[] GetByteArray(string columnName)
        {
            object value = GetObject(columnName);

            if (value == null)
                return null;

            if (value is byte[] bytes)
                return bytes;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public DateTime GetDateTime(string columnName)
        {
            object value = GetObject(columnName);

            if (value is DateTime)
                return (DateTime)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public DateTime? GetDateTimeNullable(string columnName)
        {
            object value = GetObject(columnName);

            if (value == null)
                return null;

            if (value is DateTime)
                return (DateTime)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public decimal GetDecimal(string columnName)
        {
            object value = GetObject(columnName);

            if (value is byte)
                return (byte)value;

            if (value is short)
                return (short)value;

            if (value is int)
                return (int)value;

            if (value is decimal)
                return (decimal)value;

            if (value is double)
                return Convert.ToDecimal(value, Cultures.Default);

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public decimal? GetDecimalNullable(string columnName)
        {
            object value = GetObject(columnName);

            if (value == null)
                return null;

            if (value is byte)
                return (byte)value;

            if (value is short)
                return (short)value;

            if (value is int)
                return (int)value;

            if (value is decimal)
                return (decimal)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public double GetDouble(string columnName)
        {
            object value = GetObject(columnName);

            if (value is byte)
                return (byte)value;

            if (value is short)
                return (short)value;

            if (value is int)
                return (int)value;

            if (value is decimal)
                return Convert.ToDouble(value, Cultures.Default);

            if (value is double)
                return (double)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public double? GetDoubleNullable(string columnName)
        {
            object value = GetObject(columnName);

            if (value == null)
                return null;

            if (value is byte)
                return (byte)value;

            if (value is short)
                return (short)value;

            if (value is double)
                return (double)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public Guid GetGuid(string columnName)
        {
            object value = GetObject(columnName);

            if (value is Guid)
                return (Guid)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public short GetInt16(string columnName)
        {
            object value = GetObject(columnName);

            if (value is byte)
                return (byte)value;

            if (value is short)
                return (short)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public short? GetInt16Nullable(string columnName)
        {
            object value = GetObject(columnName);

            if (value == null)
                return null;

            if (value is byte)
                return (byte)value;

            if (value is short)
                return (short)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public int GetInt32(string columnName)
        {
            object value = GetObject(columnName);

            if (value is byte)
                return (byte)value;

            if (value is short)
                return (short)value;

            if (value is int)
                return (int)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public int? GetInt32Nullable(string columnName)
        {
            object value = GetObject(columnName);

            if (value == null)
                return null;

            if (value is byte)
                return (byte)value;

            if (value is short)
                return (short)value;

            if (value is int)
                return (int)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public long GetInt64(string columnName)
        {
            object value = GetObject(columnName);

            if (value is byte)
                return (byte)value;

            if (value is short)
                return (short)value;

            if (value is int)
                return (int)value;

            if (value is long)
                return (long)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        public long? GetInt64Nullable(string columnName)
        {
            object value = GetObject(columnName);

            if (value == null)
                return null;

            if (value is byte)
                return (byte)value;

            if (value is short)
                return (short)value;

            if (value is int)
                return (int)value;

            if (value is long)
                return (long)value;

            throw ApplicationError.Create(ErrorMessage.DataTypeMismatch, columnName, value.GetType().Name);
        }

        private object GetObject(string columnName)
        {
            if (!_row.Table.Columns.Contains(columnName))
            {
                throw ApplicationError.Create(ErrorMessage.ColumnNotFound, columnName);
            }

            if (_row.IsNull(columnName))
                return null;

            return _row[columnName];
        }

        public string GetString(string columnName)
        {
            object value = GetObject(columnName);

            if (value == null)
                return null;

            var result = ((string)value).Trim();
            if (result.Length == 0)
                return null;

            return result;
        }

        public bool IsNull(string columnName)
        {
            if (!_row.Table.Columns.Contains(columnName))
                throw ApplicationError.Create(ErrorMessage.ColumnNotFound, columnName);

            return _row.IsNull(columnName);
        }
    }
}