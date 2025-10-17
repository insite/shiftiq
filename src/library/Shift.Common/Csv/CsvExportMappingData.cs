using System;
using System.ComponentModel;
using System.Linq;

namespace Shift.Common
{
    [Serializable]
    internal class CsvExportMappingData
    {
        #region Properties

        public string ColumnName
        {
            get;
            private set;
        }

        #endregion

        #region Fields

        private PropertyDescriptor[] _properties;
        private string _formatString;
        private Func<object[], string> _formatFunction;

        #endregion

        #region Constructor

        public CsvExportMappingData(PropertyDescriptor property, string name, string format)
            : this(new[] { property }, name, format)
        {

        }

        public CsvExportMappingData(PropertyDescriptor[] properties, string name, string format)
            : this(properties, name)
        {
            if (format != null)
            {
                _formatString = format;
                _formatFunction = FormatByString;
            }
        }

        public CsvExportMappingData(PropertyDescriptor property, string name, Func<object[], string> format)
            : this(new[] { property }, name, format)
        {

        }

        public CsvExportMappingData(PropertyDescriptor[] properties, string name, Func<object[], string> format)
            : this(properties, name)
        {
            _formatFunction = format;
        }

        public CsvExportMappingData(PropertyDescriptor property, string name)
            : this(new[] { property }, name)
        {

        }

        public CsvExportMappingData(PropertyDescriptor[] properties, string name)
        {
            if (properties.IsNotEmpty())
            {
                for (var i = 0; i < properties.Length; i++)
                {
                    if (properties[i] == null)
                        throw new ArgumentNullException($"{nameof(properties)}[{i}]");
                }
            }
            else
                throw new ArgumentNullException(nameof(properties));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            ColumnName = name;

            _properties = properties;
            _formatString = "{0}";
            _formatFunction = FormatByString;

            if (properties.Length == 1)
            {
                var p = properties[0];
                if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?))
                    _formatFunction = FormatBoolean;
                else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                    _formatFunction = FormatDateTime;
            }
        }

        #endregion

        #region Public methods

        public string Format(object obj)
        {
            if (_formatFunction == null)
                throw new ApplicationException("Format is not defined");

            var values = _properties.Select(x => x.GetValue(obj)).ToArray();
            var result = _formatFunction(values);

            result = result?
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\"", "\"\"");

            return result;
        }

        #endregion

        #region Helper methods

        private static string FormatBoolean(object[] values)
        {
            if (values == null || values[0] == null)
                return "No";
            return (bool)values[0] ? "Yes" : "No";
        }

        private static string FormatDateTime(object[] values)
        {
            return string.Format("{0:MMM dd, yyyy}", values);
        }

        private string FormatByString(object[] values)
        {
            return string.Format(_formatString, values);
        }

        #endregion
    }
}
