using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shift.Common
{
    public class DifferenceCollection : ICollection<Difference>
    {
        #region Properties

        public bool IsEmpty => _list.Count == 0;

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public Difference this[string name] => _dict.ContainsKey(name) ? _dict[name] : null;

        #endregion

        #region Fields

        private List<Difference> _list = new List<Difference>();
        private Dictionary<string, Difference> _dict = new Dictionary<string, Difference>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region ICollection

        public void Add(Difference item)
        {
            if (item == null) return;
            _list.Add(item);
            _dict.Add(item.PropertyName, item);
        }

        public void Add(DifferenceCollection collection)
        {
            foreach (var item in collection)
            {
                if (_dict.ContainsKey(item.PropertyName))
                    throw new ApplicationError($"The property already exists in collection: {item.PropertyName}");

                _list.Add(item);
                _dict.Add(item.PropertyName, item);
            }
        }

        public void Clear()
        {
            _list.Clear();
            _dict.Clear();
        }

        public bool Contains(Difference item) => _list.Contains(item);

        public bool Contains(string name) => _dict.ContainsKey(name);

        public void CopyTo(Difference[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(Difference item)
        {
            if (item == null)
                return false;

            var isRemoved = _list.Remove(item);

            if (isRemoved)
                _dict.Remove(item.PropertyName);

            return isRemoved;
        }

        public IEnumerator<Difference> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Methods: convert to string

        public override string ToString() => ToString(null, null);

        public string ToString(ICollection<string> exclusions, IDictionary<string, Action<Difference, StringBuilder>> fieldFormat)
        {
            var sb = new StringBuilder();

            WriteString(sb, exclusions, fieldFormat);

            return sb.ToString();
        }

        public void WriteString(StringBuilder sb, ICollection<string> exclusions = null, IDictionary<string, Action<Difference, StringBuilder>> fieldFormat = null)
        {
            if (Count == 0)
                return;

            var hasExclusions = exclusions.IsNotEmpty();
            var hasFormats = fieldFormat.IsNotEmpty();

            foreach (var diff in this)
            {
                if (hasExclusions && exclusions.Contains(diff.PropertyName))
                    continue;

                if (hasFormats && fieldFormat.TryGetValue(diff.PropertyName, out var format))
                    format(diff, sb);
                else
                    sb.Append("- ")
                          .Append(diff.PropertyName)
                          .Append(" changed from *").Append(GetStringValue(diff.ValueBefore))
                          .Append("* to **").Append(GetStringValue(diff.ValueAfter)).Append("**")
                          .AppendLine();
            }
        }

        private static string GetStringValue(object value)
        {
            if (ValueConverter.IsNull(value)) return "(blank)";

            if (value is string strValue) return strValue;

            if (value is bool boolValue) return boolValue ? bool.TrueString : bool.FalseString;

            if (value is DateTime dateValue) return $"{dateValue:MMM d, yyyy}";

            if (value is DateTimeOffset dateOffsetValue) return $"{dateOffsetValue:MMM d, yyyy}";

            return value.ToString();
        }

        #endregion
    }
}