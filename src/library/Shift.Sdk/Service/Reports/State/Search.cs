using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Reports
{
    public class Search<TFilter> where TFilter : Filter
    {
        public int PageIndex { get; set; }
        public SearchSort Sort { get; set; }
        public DateTimeOffset? LastSearched { get; set; }
        public TFilter Filter { get; set; }
    }

    [Serializable]
    public class SearchSort
    {
        #region Construction

        public SearchSort()
        {
            Mappings = new Dictionary<string, IEnumerable<SearchSortField>>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        [JsonProperty]
        public IReadOnlyList<SearchSortField> Fields { get; private set; }

        [JsonProperty]
        public SortOrder Order { get; private set; }

        [JsonProperty]
        public IDictionary<string, IEnumerable<SearchSortField>> Mappings { get; private set; }

        #region Initialization

        public static SearchSort Create(string expression, string order)
        {
            var searchOrder = SortOrder.None;

            if (order.IsNotEmpty())
            {
                if (string.Equals(order, "Asc", StringComparison.OrdinalIgnoreCase) || string.Equals(order, "Ascending", StringComparison.OrdinalIgnoreCase))
                    searchOrder = SortOrder.Ascending;
                else if (string.Equals(order, "Desc", StringComparison.OrdinalIgnoreCase) || string.Equals(order, "Descending", StringComparison.OrdinalIgnoreCase))
                    searchOrder = SortOrder.Descending;
            }

            return Create(expression, searchOrder);
        }

        public static SearchSort Create(string expression, SortOrder order)
        {
            return new SearchSort
            {
                Fields = ParseExpression(expression),
                Order = order
            };
        }

        private static SearchSortField[] ParseExpression(string expression)
        {
            var fields = new List<SearchSortField>();

            if (!string.IsNullOrEmpty(expression))
            {
                var name = new StringBuilder();
                var order = new StringBuilder();

                var state = 0;
                var prevCh = ' ';

                for (var i = 0; i < expression.Length; i++)
                {
                    var ch = expression[i];

                    if (char.IsLetterOrDigit(ch) || ch == '.' || ch == '_')
                    {
                        if (state == 0)
                            name.Append(ch);
                        else if (state == 1)
                            order.Append(ch);
                        else
                            throw new ArgumentException($"Invalid expression: {expression}");
                    }
                    else if (ch == ' ')
                    {
                        if (prevCh != ' ' && prevCh != ',')
                            state++;
                    }
                    else if (ch == ',')
                    {
                        if (name.Length > 0)
                            fields.Add(new SearchSortField(name.ToString(), order.ToString()));

                        state = 0;
                        name.Clear();
                        order.Clear();
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid expression: {expression}");
                    }

                    prevCh = ch;
                }

                if (name.Length > 0)
                    fields.Add(new SearchSortField(name.ToString(), order.ToString()));
            }

            return fields.ToArray();
        }

        [Serializable]
        public class SearchSortField
        {
            public string Name { get; }
            public SortOrder Order { get; }

            public SearchSortField(string name, string order)
            {
                Name = name;
                Order = StringHelper.Equals(order, "desc")
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }

            [JsonConstructor]
            public SearchSortField(string name, SortOrder order)
            {
                Name = name;
                Order = order;
            }
        }

        #endregion

        #region Methods

        public void AddMapping(string name, string expression)
        {
            Mappings.Add(name, ParseExpression(expression));
        }

        public void RemoveMapping(string name)
        {
            Mappings.Remove(name);
        }

        public string GetExpression(bool compile = true)
        {
            if (Order == SortOrder.None)
                return null;

            var sb = new StringBuilder();

            foreach (var field in EnumerateFields(compile))
            {
                if (field.Order == SortOrder.None)
                    continue;

                if (sb.Length != 0)
                    sb.Append(",");

                sb.Append(field.Name);

                var isDescending = field.Order == SortOrder.Descending;
                if (compile && Order == SortOrder.Descending)
                    isDescending = !isDescending;

                if (isDescending)
                    sb.Append(" DESC");
            }

            return sb.Length == 0 ? null : sb.ToString();
        }

        private IEnumerable<SearchSortField> EnumerateFields(bool compile)
        {
            foreach (var field in Fields)
                if (compile && Mappings.ContainsKey(field.Name))
                    foreach (var mappingField in Mappings[field.Name])
                        yield return mappingField;
                else
                    yield return field;
        }

        #endregion
    }
}
