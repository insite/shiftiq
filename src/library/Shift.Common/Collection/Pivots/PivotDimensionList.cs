using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shift.Common
{
    [Serializable]
    [JsonConverter(typeof(JsonDimensionListConverter))]
    public class PivotDimensionList : IEnumerable<PivotDimension>
    {
        #region Classes

        private class JsonDimensionListConverter : JsonConverter
        {
            public override bool CanConvert(Type type) => type == typeof(PivotDimensionList);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jObj = JObject.Load(reader);
                var names = (JArray)jObj["names"];
                var units = (JArray)jObj["units"];

                if (names.Count != units.Count)
                    throw new ApplicationError("Invalid JSON data: " + jObj.ToString());

                var list = new PivotDimensionList();

                for (var i = 0; i < names.Count; i++)
                {
                    var dimension = new PivotDimension(names[i].Value<string>());
                    var dimensionUnits = (JArray)units[i];

                    foreach (JValue unit in dimensionUnits)
                        dimension.AddUnit(unit.Value<string>());

                    list._items.Add(dimension);
                }

                return list;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var list = (PivotDimensionList)value;

                var names = new JArray();
                var units = new JArray();

                foreach (var item in list._items)
                {
                    names.Add(item.Name);
                    units.Add(new JArray(item.ToArray()));
                }

                var jObj = new JObject
                {
                    { "names", names },
                    { "units", units }
                };

                jObj.WriteTo(writer);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The number of dimensions in the list.
        /// </summary>
        public int Count => _items.Count;

        public PivotDimension this[int index] => _items[index];

        #endregion

        #region Fields

        private List<PivotDimension> _items;

        #endregion

        #region Construction

        public PivotDimensionList()
        {
            _items = new List<PivotDimension>();
        }

        #endregion

        #region Methods (public)

        /// <summary>
        /// Adds a dimension to the list.
        /// </summary>
        public PivotDimension Add(string name)
        {
            var dimension = new PivotDimension(name);

            _items.Add(dimension);

            return dimension;
        }

        public MultiKey<string> GetOrAddKey(params string[] units)
        {
            if (_items.Count != units.Length)
                throw new InvalidPivotKeyException();

            var key = new MultiKey<string>(units);

            if (!IsValidKey(units))
            {
                for (var i = 0; i < _items.Count; i++)
                {
                    var unit = units[i];
                    var dimension = _items[i];

                    if (!dimension.Contains(unit))
                        dimension.AddUnit(unit);
                }
            }

            return key;
        }

        public bool IsValidKey(MultiKey<string> key) => IsValidKey(key.Values);

        public bool IsValidKey(params string[] units)
        {
            var isValid = false;

            if (units.Length == _items.Count)
            {
                isValid = true;

                for (var i = 0; i < _items.Count; i++)
                {
                    if (!_items[i].Contains(units[i]))
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            return isValid;
        }

        #endregion

        #region IEnumerable

        public IEnumerator<PivotDimension> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
