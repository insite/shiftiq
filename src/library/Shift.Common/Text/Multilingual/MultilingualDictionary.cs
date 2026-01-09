using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Constant;

namespace Shift.Common
{
    [Serializable, JsonConverter(typeof(MultilingualDictionaryJsonConverter))]
    public class MultilingualDictionary
    {
        #region Classes

        public class MultilingualDictionaryJsonConverter : JsonConverter<MultilingualDictionary>
        {
            private bool _keepEmpty = false;

            public MultilingualDictionaryJsonConverter()
            {

            }

            public MultilingualDictionaryJsonConverter(bool keepEmpty) : this()
            {
                _keepEmpty = keepEmpty;
            }

            private static readonly Type BaseType = typeof(MultilingualDictionary);

            public override MultilingualDictionary ReadJson(JsonReader reader, Type objectType, MultilingualDictionary existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (objectType != BaseType && !objectType.IsSubclassOf(BaseType))
                    throw ApplicationError.Create("Unexpected type: " + objectType.FullName);

                var ctor = objectType.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                    throw ApplicationError.Create("Constructor not found: " + objectType.FullName);

                if (reader.TokenType != JsonToken.StartObject)
                    throw WrongTokenTypeException(JsonToken.StartObject);

                var instance = (MultilingualDictionary)ctor.Invoke(Array.Empty<object>());

                while (true)
                {
                    ReadReader();

                    if (reader.TokenType == JsonToken.EndObject)
                        break;

                    if (reader.TokenType != JsonToken.PropertyName)
                        throw WrongTokenTypeException(JsonToken.PropertyName);

                    var name = (string)reader.Value;

                    ReadReader();

                    if (reader.TokenType == JsonToken.Null)
                        continue;

                    if (reader.TokenType != JsonToken.StartObject)
                        throw WrongTokenTypeException(JsonToken.StartObject);

                    var value = serializer.Deserialize<MultilingualString>(reader);

                    instance._items.Add(name, value);
                }

                return instance;

                ApplicationError WrongTokenTypeException(JsonToken type)
                {
                    throw ApplicationError.Create(
                        "Unexpected token type: get {0} while expected {1}",
                        reader.TokenType.GetName(),
                        type.GetName());
                }

                void ReadReader()
                {
                    if (!reader.Read())
                        throw ApplicationError.Create("Unexpected end of object");
                }
            }

            public override void WriteJson(JsonWriter writer, MultilingualDictionary value, JsonSerializer serializer)
            {
                var allowWrite = _keepEmpty ? (Func<MultilingualString, bool>)HasItems : HasValue;

                writer.WriteStartObject();

                foreach (var kv in value._items)
                {
                    var item = kv.Value;
                    if (item == null || !allowWrite(item))
                        continue;

                    writer.WritePropertyName(kv.Key);
                    serializer.Serialize(writer, item);
                }

                writer.WriteEndObject();

                bool HasValue(MultilingualString item) => !item.IsEmpty;

                bool HasItems(MultilingualString item) => item.Count > 0;
            }
        }

        #endregion

        #region Properties

        public int Count => _items.Count;

        public bool IsEmpty => !_items.Any(x => !x.Value.IsEmpty);

        public virtual MultilingualString this[string name]
        {
            get => _items.TryGetValue(name, out var item) ? item : null;
            set => _items[name] = value;
        }

        public IEnumerable<string> Languages => _items.Values.SelectMany(x => x.Languages).Distinct().OrderBy(x => x);

        #endregion

        #region Fields

        private readonly Dictionary<string, MultilingualString> _items;

        #endregion

        #region Construction

        public MultilingualDictionary()
        {
            _items = new Dictionary<string, MultilingualString>(StringComparer.OrdinalIgnoreCase);
        }

        protected MultilingualDictionary(MultilingualDictionary source)
            : this()
        {
            if (source == null)
                return;

            foreach (var kv in source._items)
            {
                if (kv.Value != null)
                    _items.Add(kv.Key, kv.Value.Clone());
            }
        }

        public virtual MultilingualDictionary Clone() => new MultilingualDictionary(this);

        #endregion

        #region Methods (comparison)

        public bool IsEqual(MultilingualDictionary other)
        {
            if (other == null)
                return false;

            if (_items.Count == 0 && other._items.Count == 0)
                return true;

            foreach (var key in _items.Keys.Concat(other._items.Keys.Where(x => !_items.ContainsKey(x))))
            {
                var isEmpty1 = !_items.TryGetValue(key, out var value1) || value1.IsEmpty;
                var isEmpty2 = !other._items.TryGetValue(key, out var value2) || value2.IsEmpty;

                if (isEmpty1 && isEmpty2)
                    continue;

                if (isEmpty1 != isEmpty2 || !value1.IsEqual(value2))
                    return false;
            }

            return true;
        }

        #endregion

        #region Methods (get)

        public MultilingualString Get(string name) => this[name];

        public static MultilingualString Get(string json, string name) => Deserialize(json).Get(name);

        #endregion

        #region Methods (add)

        public MultilingualString AddOrGet(string name)
        {
            if (!_items.TryGetValue(name, out var item))
                _items.Add(name, item = new MultilingualString());
            else if (item == null)
                _items[name] = item = new MultilingualString();

            return item;
        }

        #endregion

        #region Methods (remove)

        public void Clear()
        {
            _items.Clear();
        }

        public void ClearInstructions()
        {
            _items.Remove(EventInstructionType.Accommodation.GetName());
            _items.Remove(EventInstructionType.Additional.GetName());
            _items.Remove(EventInstructionType.Cancellation.GetName());
            _items.Remove(EventInstructionType.Contact.GetName());
            _items.Remove(EventInstructionType.Completion.GetName());
        }

        public bool Remove(string name) => _items.Remove(name);

        public bool RemoveEmptyItems()
        {
            var isRemoved = false;

            foreach (var key in _items.Select(kv => kv.Key).ToArray())
            {
                var value = _items[key];
                if (value.IsEmpty)
                {
                    _items.Remove(key);
                    isRemoved = true;
                }
                else if (value.RemoveEmptyItems())
                {
                    isRemoved = true;
                }
            }

            return isRemoved;
        }

        #endregion

        #region Methods (helpers)

        public IEnumerable<string> GetNames() => _items.Keys;

        public IEnumerable<MultilingualString> GetItems() => _items.Values;

        public bool Exists(string name) => _items.ContainsKey(name);

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static MultilingualDictionary Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new MultilingualDictionary()
                : JsonConvert.DeserializeObject<MultilingualDictionary>(json);
        }

        #endregion
    }
}
