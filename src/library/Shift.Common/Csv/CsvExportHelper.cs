using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Shift.Common
{
    public class CsvExportHelper
    {
        #region Fields

        private IEnumerable<object> _data;
        private PropertyDescriptorCollection _properties;
        private CsvExportMappingCollection _mappings;

        #endregion

        #region Constructor

        public CsvExportHelper(IListSource source) => Init(source);

        public CsvExportHelper(IEnumerable source) => Init(source);

        #endregion

        #region Initialization

        private void Init(IListSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Init(source.GetList());
        }

        public void Init(IEnumerable source)
        {
            var data = source?.Cast<object>();
            if (data == null || !data.Any())
                throw new ArgumentException("The data source is empty");

            _properties = TypeDescriptor.GetProperties(data.First());
            if (_properties.IsEmpty())
                throw new ArgumentException("The data source has no properties.");

            _data = data;
            _mappings = new CsvExportMappingCollection();
        }

        #endregion

        #region Public methods

        public void AddMapping(string property, string name)
        {
            _mappings.Add(new CsvExportMappingData(GetPropertyDescriptor(property), name));
        }

        public void AddMapping(string property, string name, string format)
        {
            _mappings.Add(new CsvExportMappingData(GetPropertyDescriptor(property), name, format));
        }

        public void AddMapping(string[] properties, string name, string format)
        {
            _mappings.Add(new CsvExportMappingData(GetPropertyDescriptor(properties), name, format));
        }

        public void AddMapping(string property, string name, Func<object[], string> format)
        {
            _mappings.Add(new CsvExportMappingData(GetPropertyDescriptor(property), name, format));
        }

        public void AddMapping(string[] properties, string name, Func<object[], string> format)
        {
            _mappings.Add(new CsvExportMappingData(GetPropertyDescriptor(properties), name, format));
        }

        public string GetString()
        {
            if (_mappings.Count == 0)
                throw new InvalidOperationException("No mappings defined for this export.");

            var csv = new StringBuilder();

            csv.Append(GetHeaderLine(_mappings));

            foreach (var item in _data)
            {
                csv.AppendLine();

                csv.Append(GetDataLine(item, _mappings));
            }

            return csv.ToString();
        }

        public byte[] GetBytes(Encoding encoding)
        {
            var str = GetString();
            return encoding.GetBytes(str);
        }

        #endregion

        #region Helper methods

        private PropertyDescriptor[] GetPropertyDescriptor(string[] names)
        {
            return names.Select(GetPropertyDescriptor).ToArray();
        }

        private PropertyDescriptor GetPropertyDescriptor(string name)
        {
            var propertyDescriptor = _properties.Find(name, true);
            if (propertyDescriptor == null)
                throw new InvalidOperationException($"Invalid property name: {name}");

            return propertyDescriptor;
        }

        private static string GetDataLine(object item, CsvExportMappingCollection mappings)
        {
            var line = new StringBuilder();

            foreach (var mapping in mappings)
                RenderColumnValue(line, mapping.Format(item));

            return line.ToString();
        }

        private static string GetHeaderLine(CsvExportMappingCollection mappings)
        {
            var header = new StringBuilder();

            foreach (var mapping in mappings)
                RenderColumnValue(header, mapping.ColumnName);

            return header.ToString();
        }

        private static void RenderColumnValue(StringBuilder csv, string value)
        {
            if (csv.Length > 0)
                csv.Append(",");

            csv.AppendFormat("\"{0}\"", value);
        }

        #endregion
    }
}
