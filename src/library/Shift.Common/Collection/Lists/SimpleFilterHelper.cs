using System.Collections.Generic;
using System.Text;

namespace Shift.Common
{
    public static class SimpleFilterHelper
    {
        #region FilterItem class

        public class FilterItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        #endregion

        #region Public methods

        public static FilterItem[] Parse(string filter, ICollection<string> allowedNames, string defaultName)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return null;

            filter = filter.Trim();

            var index = 0;
            var result = new List<FilterItem>();

            while (index < filter.Length)
            {
                var item = ParseItem(filter, allowedNames, defaultName, ref index);

                if (item == null)
                    return new[] { new FilterItem { Name = defaultName, Value = filter } };

                if (!string.IsNullOrEmpty(item.Value))
                    result.Add(item);
            }

            return result.ToArray();
        }

        public static string ConvertToFilter(IEnumerable<FilterItem> items)
        {
            StringBuilder filter = new StringBuilder();

            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.Value))
                    continue;

                string value = item.Value;

                if (value.Contains(" ") || value.Contains("\""))
                    value = "\"" + value.Replace("\"", "\"\"") + "\"";

                if (filter.Length > 0)
                    filter.Append(" ");

                filter.Append(item.Name + ":" + value);
            }

            return filter.ToString();
        }

        #endregion

        #region Helper methods

        private static FilterItem ParseItem(string filter, ICollection<string> allowedNames, string defaultName, ref int index)
        {
            var nameStart = index;
            var nameEnd = filter.IndexOf(':', nameStart + 1);

            if (nameEnd < 0)
            {
                var item = new FilterItem { Name = defaultName, Value = filter.Substring(index) };
                index = filter.Length;
                return item;
            }

            var name = filter.Substring(nameStart, nameEnd - nameStart).Trim();
            if (string.IsNullOrEmpty(name) || !allowedNames.Contains(name))
                return null;

            var valueStart = nameEnd + 1;
            int valueEnd;
            string value;

            if (filter.Length == valueStart)
            {
                valueEnd = valueStart;
                value = null;
            }
            else
            {
                char separateChar;

                if (filter[valueStart] == '"')
                {
                    separateChar = '"';
                    valueStart++;
                }
                else
                    separateChar = ' ';

                valueEnd = valueStart + 1;

                while (valueEnd < filter.Length && filter[valueEnd] != separateChar)
                    valueEnd++;

                value = valueStart == valueEnd || valueStart >= filter.Length
                    ? null
                    : filter.Substring(valueStart, valueEnd - valueStart);
            }

            index = valueEnd + 1;

            while (index < filter.Length && filter[index] == ' ')
                index++;

            return new FilterItem { Name = name, Value = value };
        }

        #endregion
    }
}