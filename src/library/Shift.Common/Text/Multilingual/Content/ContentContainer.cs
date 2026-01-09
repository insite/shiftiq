using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Constant;

namespace Shift.Common
{
    [Serializable, JsonConverter(typeof(ContentContainerJsonConverter))]
    public sealed class ContentContainer
    {
        #region Classes

        public enum SetNullAction
        {
            None,
            Set,
            Remove
        }

        public class ContentContainerJsonConverter : JsonConverter<ContentContainer>
        {
            private bool _keepEmpty = false;

            public ContentContainerJsonConverter()
            {

            }

            public ContentContainerJsonConverter(bool keepEmpty) : this()
            {
                _keepEmpty = keepEmpty;
            }

            public override ContentContainer ReadJson(JsonReader reader, Type objectType, ContentContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.StartObject)
                    throw WrongTokenTypeException(JsonToken.StartObject);

                var instance = new ContentContainer();

                while (true)
                {
                    ReadReader();

                    if (reader.TokenType == JsonToken.EndObject)
                        break;

                    if (reader.TokenType != JsonToken.PropertyName)
                        throw WrongTokenTypeException(JsonToken.PropertyName);

                    var name = (string)reader.Value;

                    ReadReader();

                    if (name == "$loaded")
                    {
                        if (reader.TokenType != JsonToken.Boolean)
                            throw WrongTokenTypeException(JsonToken.Boolean);

                        instance.IsLoaded = (bool)reader.Value;
                    }
                    else
                    {

                        if (reader.TokenType == JsonToken.Null)
                            continue;

                        if (reader.TokenType != JsonToken.StartObject)
                            throw WrongTokenTypeException(JsonToken.StartObject);

                        var value = serializer.Deserialize<ContentContainerItem>(reader);

                        instance._items.Add(name, value);
                    }
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

            public override void WriteJson(JsonWriter writer, ContentContainer value, JsonSerializer serializer)
            {
                var allowWrite = _keepEmpty ? (Func<ContentContainerItem, bool>)HasItems : HasValue;

                writer.WriteStartObject();

                if (value.IsLoaded)
                {
                    writer.WritePropertyName("$loaded");
                    writer.WriteValue(true);
                }

                foreach (var kv in value._items)
                {
                    var item = kv.Value;
                    if (!allowWrite(item))
                        continue;

                    writer.WritePropertyName(kv.Key);
                    serializer.Serialize(writer, item);
                }

                writer.WriteEndObject();

                bool HasValue(ContentContainerItem item) => !item.IsEmpty;

                bool HasItems(ContentContainerItem item) => item.HasItems;
            }
        }

        #endregion

        public const string DefaultLanguage = Language.Default;

        private readonly Dictionary<string, ContentContainerItem> _items;

        public int Count => _items.Count;

        public bool IsEmpty => !_items.Values.Any(x => !x.IsEmpty);

        public bool HasItems => _items.Values.Any(x => x.HasItems);

        public bool IsLoaded { get; set; }

        public string[] Languages => _items.Values.SelectMany(x => x.Languages).Distinct().ToArray();

        public ContentContainer()
            : base()
        {
            _items = new Dictionary<string, ContentContainerItem>(StringComparer.OrdinalIgnoreCase);
        }

        private ContentContainer(ContentContainer source)
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

        public ContentContainer Clone()
        {
            return new ContentContainer(this)
            {
                IsLoaded = IsLoaded
            };
        }

        public IEnumerable<string> GetLabels() => _items.Keys;

        public IEnumerable<ContentContainerItem> GetItems() => _items.Values;

        public bool Exists(string label) => _items.ContainsKey(label);

        public ContentContainerItem this[string label]
        {
            get
            {
                if (!_items.TryGetValue(label, out var result))
                    _items.Add(label, result = new ContentContainerItem());

                return result;
            }
            set
            {
                this[label].Set(value);
            }
        }

        public string GetText(string label, string language = DefaultLanguage, bool defaultIfNull = false)
            => this[label].GetText(language, defaultIfNull);

        public string GetHtml(string label, string language = DefaultLanguage, bool defaultIfNull = false)
            => this[label].GetHtml(language, defaultIfNull);

        public string GetSnip(string label, string language = DefaultLanguage, bool defaultIfNull = false)
            => this[label].GetSnip(language, defaultIfNull);

        public void SetText(string label, string language, string text) => this[label].Text[language] = text;
        public void SetHtml(string label, string language, string text) => this[label].Html[language] = text;
        public void SetSnip(string label, string language, string text) => this[label].Snip[language] = text;

        public void CreateSnips()
        {
            foreach (var item in _items.Values)
                item.CreateSnip();
        }

        public void Set(ContentContainer content, SetNullAction nullAction)
        {
            if (content?.HasItems != true)
                return;

            var isSetNull = nullAction == SetNullAction.Set;
            var isRemoveNull = nullAction == SetNullAction.Remove;

            foreach (var label in content.GetLabels())
            {
                var itemThat = content[label];
                if (itemThat?.HasItems != true)
                    continue;

                var itemThis = this[label];

                SetString(itemThis.Text, itemThat.Text);
                SetString(itemThis.Html, itemThat.Html);
                SetString(itemThis.Snip, itemThat.Snip);
            }

            void SetString(MultilingualString strThis, MultilingualString strThat)
            {
                foreach (var lang in strThat.Languages)
                {
                    var valueThat = strThat[lang];

                    if (valueThat.IsNotEmpty())
                        strThis[lang] = valueThat;
                    else if (isSetNull)
                        strThis[lang] = null;
                    else if (isRemoveNull)
                        strThis.Remove(lang);
                }
            }
        }

        public static bool IsEqual(ContentContainer d1, ContentContainer d2)
        {
            var isEmpty1 = d1 == null || d1.IsEmpty;
            var isEmpty2 = d2 == null || d2.IsEmpty;

            return isEmpty1 == isEmpty2 && (isEmpty1 || d1.IsEqual(d2));
        }

        public bool IsEqual(ContentContainer other)
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

        #region Shortcuts

        public ContentContainerItem Title => this[ContentLabel.Title];

        public ContentContainerItem Summary => this[ContentLabel.Summary];

        public ContentContainerItem Description => this[ContentLabel.Description];

        public ContentContainerItem Feedback => this[ContentLabel.Feedback];

        public ContentContainerItem FeedbackWhenNotSelected => this[ContentLabel.FeedbackWhenNotSelected];

        public ContentContainerItem Hint => this[ContentLabel.Hint];

        public ContentContainerItem Body => this[ContentLabel.Body];

        #endregion

        #region Methods (removing items)

        public bool Remove(string name) => _items.Remove(name);

        #endregion
    }
}
