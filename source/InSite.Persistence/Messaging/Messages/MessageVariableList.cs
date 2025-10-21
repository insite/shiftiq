using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class MessageVariableList
    {
        private readonly Dictionary<string, string> _variables;
        private readonly List<string> _filter;

        public MessageVariableList(string appUrl = null, string filter = null)
        {
            _filter = filter != null ? StringHelper.Split(filter).ToList() : null;
            _variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (appUrl != null)
            {
                AddValue(MessageVariable.AppUrl, appUrl);
            }
            else
            {
                var request = HttpContext.Current?.Request;
                if (request != null)
                    AddValue(MessageVariable.AppUrl, request.Url.Scheme + "://" + request.Url.Host);
            }

            AddValue(MessageVariable.UtcNow, string.Format("{0:MMM d, yyyy} {0:HH:mm:ss tt} UTC", DateTime.UtcNow));
            AddValue(MessageVariable.CurrentYear, DateTime.Now.Year.ToString());
        }

        public bool IsVariableAccepted(string name)
        {
            return _filter == null || _filter.Contains(name);
        }

        public string GetValue(string name)
        {
            return _variables.ContainsKey(name)
                ? _variables[name]
                : null;
        }

        public void AddValue(string name, Guid value)
        {
            AddValue(name, $"{value}");
        }

        public void AddValue(string name, int value)
        {
            AddValue(name, $"{value:n0}");
        }

        public void AddValue(string name, DateTimeOffset? value)
        {
            if (value.HasValue)
                AddValue(name, $"{value:dddd, MMM d, yyyy}");
            else
                AddValue(name, "(Date Not Specified)");
        }

        public void AddValue(string name, decimal value)
        {
            AddValue(name, $"{value:n2}");
        }

        public void AddValue(string name, string value)
        {
            // If there is a filter then confirm the variable name is included in it.
            if (!IsVariableAccepted(name))
                return;

            if (_variables.ContainsKey(name))
                _variables[name] = value;
            else
                _variables.Add(name, value);
        }

        public void AddImage(string name, string value)
        {
            var html = string.Empty;

            if (!string.IsNullOrEmpty(value))
                html = $"<img src='{value}' border='0' />";

            AddValue(name, html);
        }

        public Dictionary<string, string> ToDictionary()
        {
            var result = new Dictionary<string, string>();

            foreach (var kv in _variables)
            {
                var value = MessageHelper.ReplaceVariables(_variables, kv.Value);
                result.Add(kv.Key, value);
            }

            return result;
        }
    }
}