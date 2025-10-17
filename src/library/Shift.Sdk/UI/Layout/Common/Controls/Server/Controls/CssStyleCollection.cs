using System;
using System.Collections.Generic;
using System.Text;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public class CssStyleCollection
    {
        #region Properties

        public string this[string name]
        {
            get
            {
                return _definitions.TryGetValue(name, out string value)
                    ? value
                    : string.Empty;
            }
            set
            {
                if (_definitions.ContainsKey(name))
                    _definitions[name] = value;
                else
                    _definitions.Add(name, value);
            }
        }

        public bool IsEmpty => _definitions.Count == 0;

        #endregion

        #region Fields

        private Dictionary<string, string> _definitions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Methods

        public static CssStyleCollection Parse(string input)
        {
            var result = new CssStyleCollection();

            if (input.IsNotEmpty())
            {
                var state = 1;
                var nameBuffer = new StringBuilder();
                var valueBuffer = new StringBuilder();

                for (var i = 0; i < input.Length; i++)
                {
                    var ch = input[i];

                    if (state == 1)
                    {
                        if (ch == ':')
                            state = 2;
                        else if (!char.IsWhiteSpace(ch))
                            nameBuffer.Append(ch);
                    }
                    else if (state == 2)
                    {
                        if (ch == ';')
                        {
                            state = 1;
                            FlushBuffer();
                        }
                        else
                        {
                            valueBuffer.Append(ch);
                        }
                    }
                }

                FlushBuffer();

                void FlushBuffer()
                {
                    var name = nameBuffer.ToString();
                    var value = valueBuffer.ToString().Trim();

                    if (name.Length > 0 && value.Length > 0)
                        result[name] = value;

                    nameBuffer.Clear();
                    valueBuffer.Clear();
                }
            }

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var key in _definitions.Keys)
                sb.AppendFormat("{0}:{1};", key, _definitions[key]);

            return sb.ToString();
        }

        #endregion
    }
}