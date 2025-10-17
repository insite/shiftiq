using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Constant;

namespace Shift.Common
{
    [Serializable, JsonConverter(typeof(MultilingualStringJsonConverter))]
    public sealed class MultilingualString : IEnumerable<KeyValuePair<string, string>>
    {
        #region Classes

        public class MultilingualStringJsonConverter : JsonConverter<MultilingualString>
        {
            private bool _keepEmpty = false;

            public MultilingualStringJsonConverter()
            {

            }

            public MultilingualStringJsonConverter(bool keepEmpty) : this()
            {
                _keepEmpty = keepEmpty;
            }

            public override MultilingualString ReadJson(JsonReader reader, Type objectType, MultilingualString existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.StartObject)
                    throw WrongTokenTypeException(JsonToken.StartObject);

                var instance = new MultilingualString();

                while (true)
                {
                    ReadReader();

                    if (reader.TokenType == JsonToken.EndObject)
                        break;

                    if (reader.TokenType != JsonToken.PropertyName)
                        throw WrongTokenTypeException(JsonToken.PropertyName);

                    var name = (string)reader.Value;

                    ReadReader();

                    var value = string.Empty;

                    if (reader.TokenType != JsonToken.Null)
                    {
                        if (reader.TokenType != JsonToken.String)
                            throw WrongTokenTypeException(JsonToken.String);

                        value = (string)reader.Value;
                        if (string.IsNullOrWhiteSpace(value))
                            value = string.Empty;
                    }

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

            public override void WriteJson(JsonWriter writer, MultilingualString value, JsonSerializer serializer)
            {
                writer.WriteStartObject();

                if (_keepEmpty)
                {
                    foreach (var kv in value._items)
                    {
                        writer.WritePropertyName(kv.Key);
                        writer.WriteValue(kv.Value.EmptyIfNull());
                    }
                }
                else
                {
                    foreach (var kv in value._items)
                    {
                        if (string.IsNullOrWhiteSpace(kv.Value))
                            continue;

                        writer.WritePropertyName(kv.Key);
                        writer.WriteValue(kv.Value);
                    }
                }

                writer.WriteEndObject();
            }
        }

        #endregion

        #region Properties

        public const string DefaultLanguage = Language.Default;

        public string Default
        {
            get => this[DefaultLanguage];
            set => this[DefaultLanguage] = value;
        }

        public string this[string language]
        {
            get => _items.ContainsKey(language) ? _items[language] : null;
            set => _items[language] = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        }

        public string this[string language, string organization]
        {
            get => this[GetKey(language, organization)] ?? this[language];
            set => this[GetKey(language, organization)] = value;
        }

        public int Count => _items.Count;

        public IEnumerable<string> Languages => _items.Keys.OrderBy(x => x);

        public bool IsEmpty => _items.All(x => string.IsNullOrWhiteSpace(x.Value));

        #endregion

        #region Fields

        private readonly Dictionary<string, string> _items;

        #endregion

        #region Construction

        public MultilingualString()
        {
            _items = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public MultilingualString(string text)
            : this()
        {
            this[DefaultLanguage] = text;
        }

        private MultilingualString(MultilingualString source)
            : this()
        {
            if (source == null)
                return;

            foreach (var kv in source._items)
                _items.Add(kv.Key, string.IsNullOrWhiteSpace(kv.Value) ? string.Empty : kv.Value);
        }

        public MultilingualString Clone()
            => new MultilingualString(this);

        #endregion

        #region Methods

        public bool Exists(string language)
        {
            return _items.ContainsKey(language);
        }

        public string Get(string language)
        {
            if (language.IsEmpty())
                language = DefaultLanguage;

            return _items.TryGetValue(language, out var value) && value.IsNotEmpty()
                ? value
                : Default;
        }

        public void Set(MultilingualString value)
        {
            _items.Clear();

            if (value == null || value._items.Count == 0)
                return;

            foreach (var kv in value._items)
                _items.Add(kv.Key, string.IsNullOrWhiteSpace(kv.Value) ? string.Empty : kv.Value);
        }

        public bool Remove(string language)
        {
            return _items.Remove(language);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public static string GetKey(string language, string organization)
        {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException(nameof(language));

            if (string.IsNullOrEmpty(organization))
                return language;

            return $"{language}:{organization}";
        }

        public static string GetTranslation(string json, string language)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var @string = Deserialize(json);

            if (string.IsNullOrEmpty(language))
                language = DefaultLanguage;

            return @string[language] ?? @string.Default;
        }

        public static string SetTranslation(string json, string language, string text)
        {
            if (string.IsNullOrEmpty(language))
                language = DefaultLanguage;

            var @string = !string.IsNullOrEmpty(json) ? Deserialize(json) : new MultilingualString();
            @string[language] = text;

            return @string.Serialize();
        }

        public static string SetTranslation(string json, string language, Func<string, string> set)
        {
            var text = GetTranslation(json, language);

            return SetTranslation(json, language, set(text));
        }

        public bool RemoveEmptyItems()
        {
            var isRemoved = false;

            foreach (var key in _items.Select(kv => kv.Key).ToArray())
            {
                var value = _items[key];
                if (string.IsNullOrWhiteSpace(value))
                {
                    _items.Remove(key);
                    isRemoved = true;
                }
            }

            return isRemoved;
        }

        public void RemoveExist(MultilingualString other)
        {
            var langs = Languages.ToArray();
            foreach (var lang in langs)
            {
                var exists = other.Exists(lang);
                if (exists && this[lang] == other[lang] || !exists && this[lang].IsEmpty())
                    Remove(lang);
            }
        }

        #endregion

        #region Methods (serialization)

        public static MultilingualString Deserialize(string json)
        {
            if (json.IsEmpty())
                return new MultilingualString();

            if (json.StartsWith("{"))
                return JsonConvert.DeserializeObject<MultilingualString>(json);

            return new MultilingualString { Default = json };
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(_items);
        }

        #endregion

        #region Methods (enumeration)

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Methods (comparing)

        public bool IsEqual(MultilingualString other)
        {
            if (other == null)
                return false;

            if (_items.Count == 0 && other._items.Count == 0)
                return true;

            foreach (var key in _items.Keys.Concat(other._items.Keys.Where(x => !_items.ContainsKey(x))))
            {
                var isEmpty1 = !_items.TryGetValue(key, out var value1) || string.IsNullOrWhiteSpace(value1);
                var isEmpty2 = !other._items.TryGetValue(key, out var value2) || string.IsNullOrWhiteSpace(value2);

                if (isEmpty1 && isEmpty2)
                    continue;

                if (isEmpty1 != isEmpty2 || value1 != value2)
                    return false;
            }

            return true;
        }

        #endregion
    }
}