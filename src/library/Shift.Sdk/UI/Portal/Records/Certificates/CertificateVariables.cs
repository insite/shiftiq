using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Shift.Sdk.UI
{
    public class CertificateVariables
    {
        #region Properties

        public string this[string name]
        {
            get => _items.TryGetValue(name, out var value) ? value : null;
            set => _items[name] = value;
        }

        public Dictionary<string, string> Items => _items;

        #endregion

        #region Fields

        private static readonly Regex _placeholderRegexPattern = new Regex("(?<Escape>!)?\\{(?<Name>[a-zA-Z0-9\\.]+?)(?::(?<Format>[^}]+?))?}", RegexOptions.Compiled);

        private Dictionary<string, string> _items = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Construction

        public CertificateVariables()
        {

        }

        public CertificateVariables(Dictionary<string, string> items)
        {
            _items = items;
        }

        #endregion

        #region Methods

        public string ReplacePlaceholders(string input) => ReplacePlaceholders(input, false);

        public string ReplacePlaceholdersSafe(string input) => ReplacePlaceholders(input, true);

        private string ReplacePlaceholders(string input, bool isSafe)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return _placeholderRegexPattern.Replace(input, m =>
            {
                var escapeGroup = m.Groups["Escape"];
                if (escapeGroup.Success)
                    return m.Value.Remove(escapeGroup.Index - m.Index, escapeGroup.Length);

                var name = m.Groups["Name"].Value;
                var value = isSafe
                    ? this[name]
                    : _items.ContainsKey(name)
                        ? _items[name]
                        : throw new Exception("Variable not found: " + name);

                if (value == null)
                    return string.Empty;

                var format = m.Groups["Format"].Value;
                if (string.IsNullOrEmpty(format))
                    return value.ToString();

                return string.Format("{0:" + format + "}", value);
            });
        }

        #endregion
    }
}