using System.Collections.Generic;
using System.IO;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public class ProgressPanelContextVariableCollection : ProgressPanelContextBaseCollection
    {
        #region Classes

        private class DataItem
        {
            public string Key;
            public string Value;
        }

        #endregion

        #region Properties

        public override int Count => _items.Count;

        public string this[string name]
        {
            get
            {
                var index = GetIndex(name.ToLower());

                return index >= 0 ? _items[index].Value : null;
            }
            set
            {
                var key = name.ToLower();
                var index = GetIndex(key);

                if (value.IsEmpty())
                {
                    if (index >= 0)
                        _items.RemoveAt(index);
                }
                else if (index < 0)
                {
                    _items.Insert(~index, new DataItem
                    {
                        Key = key,
                        Value = value
                    });
                }
                else
                {
                    _items[index].Value = value;
                }
            }
        }

        #endregion

        #region Fields

        private List<DataItem> _items = new List<DataItem>();

        #endregion

        #region Methods

        private int GetIndex(string key)
        {
            return _items.BinaryIndexSearch(i => i.Key.CompareTo(key));
        }

        internal override void ToJson(TextWriter output)
        {
            var data = _items.ToArray();

            output.Write("{");

            for (var i = 0; i < data.Length; i++)
            {
                if (i > 0)
                    output.Write(",");

                var item = data[i];

                output.Write($"\"{item.Key.Replace("\"", "\\\"")}\":\"{item.Value.Replace("\"", "\\\"")}\"");
            }

            output.Write("}");
        }

        #endregion
    }
}