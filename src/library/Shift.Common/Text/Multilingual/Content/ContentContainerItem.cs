using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Constant;

namespace Shift.Common
{
    /// <summary>
    /// A ContentContainerItem represents a unit of content where we have HTML and equivalent Markdown Text (or vice versa), and then a plain-text 
    /// snippet of the first 100 characters. Each of unit of content is designed to store the same value in multiple languages.
    /// </summary>
    /// <example>
    /// {
    ///     "Text": {
    ///         "en": "Hello",
    ///         "fr": "Bonjour",
    ///         "pl": "Witam"
    ///     },
    ///     "Html": {
    ///         "en": "<h1>Hello</h1>",
    ///         "fr": "<h1>Bonjour</h1>",
    ///         "pl": "<h1>Witam</h1>"
    ///     },
    ///     "Snip": {
    ///         "en": "Hello",
    ///         "fr": "Bonjour",
    ///         "pl": "Witam"
    ///     }
    /// }
    /// </example>
    [Serializable, JsonConverter(typeof(ContentContainerItemJsonConverter))]
    public sealed class ContentContainerItem
    {
        #region Classes

        public class ContentContainerItemJsonConverter : JsonConverter<ContentContainerItem>
        {
            private bool _keepEmpty = false;

            public ContentContainerItemJsonConverter()
            {

            }

            public ContentContainerItemJsonConverter(bool keepEmpty) : this()
            {
                _keepEmpty = keepEmpty;
            }

            public override ContentContainerItem ReadJson(JsonReader reader, Type objectType, ContentContainerItem existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.StartObject)
                    throw WrongTokenTypeException(JsonToken.StartObject);

                MultilingualString text = null, html = null, snip = null;

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

                    if (name == nameof(ContentContainerItem.Text))
                        text = value;
                    else if (name == nameof(ContentContainerItem.Html))
                        html = value;
                    else if (name == nameof(ContentContainerItem.Snip))
                        snip = value;
                }

                return new ContentContainerItem(text, html, snip);

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

            public override void WriteJson(JsonWriter writer, ContentContainerItem value, JsonSerializer serializer)
            {
                var allowWrite = _keepEmpty ? (Func<MultilingualString, bool>)HasItems : HasValue;

                writer.WriteStartObject();

                var text = value.Text;
                if (allowWrite(text))
                {
                    writer.WritePropertyName(nameof(ContentContainerItem.Text));
                    serializer.Serialize(writer, text);
                }

                var html = value.Html;
                if (allowWrite(html))
                {
                    writer.WritePropertyName(nameof(ContentContainerItem.Html));
                    serializer.Serialize(writer, html);
                }

                var snip = value.Snip;
                if (allowWrite(snip))
                {
                    writer.WritePropertyName(nameof(ContentContainerItem.Snip));
                    serializer.Serialize(writer, snip);
                }

                writer.WriteEndObject();

                bool HasValue(MultilingualString str) => !str.IsEmpty;

                bool HasItems(MultilingualString str) => str.Count > 0;
            }
        }

        #endregion

        public MultilingualString Text
        {
            get => _text;
            set => _text.Set(value);
        }

        public MultilingualString Html
        {
            get => _html;
            set => _html.Set(value);
        }

        public MultilingualString Snip
        {
            get => _snip;
            set => _snip.Set(value);
        }

        private MultilingualString _text, _html, _snip;

        public ContentContainerItem()
        {
            _text = new MultilingualString();
            _html = new MultilingualString();
            _snip = new MultilingualString();
        }

        private ContentContainerItem(MultilingualString text, MultilingualString html, MultilingualString snip)
        {
            _text = text ?? new MultilingualString();
            _html = html ?? new MultilingualString();
            _snip = snip ?? new MultilingualString();
        }

        private ContentContainerItem(ContentContainerItem source)
            : this()
        {
            Text = source.Text;
            Html = source.Html;
            Snip = source.Snip;
        }

        public ContentContainerItem Clone() => new ContentContainerItem(this);

        public bool IsEmpty => Text.IsEmpty && Html.IsEmpty && IsSnipEmpty(Snip);

        public bool HasItems => Text.Count > 0 || Html.Count > 0 || Snip.Count > 0;

        public bool IsEqual(ContentContainerItem other)
        {
            if (other == null)
                return false;

            return IsEqual(Text, other.Text) && IsEqual(Html, other.Html) && IsEqualSnip(Snip, other.Snip);

            bool IsEqual(MultilingualString value1, MultilingualString value2)
            {
                var isEmpty1 = value1.IsEmpty;
                var isEmpty2 = value2.IsEmpty;

                return isEmpty1 && isEmpty2 || !isEmpty1 && !isEmpty2 && value1.IsEqual(value2);
            }

            bool IsEqualSnip(MultilingualString value1, MultilingualString value2)
            {
                var isEmpty1 = IsSnipEmpty(value1);
                var isEmpty2 = IsSnipEmpty(value2);

                return isEmpty1 && isEmpty2 || !isEmpty1 && !isEmpty2 && value1.IsEqual(value2);
            }
        }

        public void Set(ContentContainerItem item)
        {
            if (item != null)
            {
                Text = item.Text;
                Html = item.Html;
                Snip = item.Snip;
            }
            else
            {
                Text = null;
                Html = null;
                Snip = null;
            }
        }

        public string GetText(string language = ContentContainer.DefaultLanguage) => GetText(Text[language], Html[language]);

        public string GetHtml(string language = ContentContainer.DefaultLanguage) => GetHtml(Text[language], Html[language]);

        public string GetSnip(string language = ContentContainer.DefaultLanguage)
        {
            var snip = Snip[language];

            if (string.IsNullOrWhiteSpace(snip))
                snip = GetSnip(Text[language], Html[language]);

            return snip;
        }

        public string GetText(string language, bool defaultIfNull)
        {
            var value = GetText(language);

            if (defaultIfNull && value.IsEmpty() && !language.Equals(ContentContainer.DefaultLanguage, StringComparison.OrdinalIgnoreCase))
                value = GetText(ContentContainer.DefaultLanguage);

            return value;
        }

        public string GetHtml(string language, bool defaultIfNull)
        {
            var value = GetHtml(language);

            if (defaultIfNull && value.IsEmpty() && !language.Equals(ContentContainer.DefaultLanguage, StringComparison.OrdinalIgnoreCase))
                value = GetHtml(ContentContainer.DefaultLanguage);

            return value;
        }

        public string GetSnip(string language, bool defaultIfNull)
        {
            var value = GetSnip(language);

            if (defaultIfNull && value.IsEmpty() && !language.Equals(ContentContainer.DefaultLanguage, StringComparison.OrdinalIgnoreCase))
                value = GetSnip(ContentContainer.DefaultLanguage);

            return value;
        }

        public static string GetText(string contentText, string contentHtml)
        {
            if (!string.IsNullOrWhiteSpace(contentText))
                return contentText;

            if (!string.IsNullOrWhiteSpace(contentHtml))
                return StringHelper.StripHtml(contentHtml);

            return null;
        }

        public static string GetHtml(string contentText, string contentHtml)
        {
            if (!string.IsNullOrWhiteSpace(contentHtml))
                return contentHtml;

            if (!string.IsNullOrWhiteSpace(contentText))
                return Markdown.ToHtml(contentText);

            return null;
        }

        public static string GetSnip(string contentText, string contentHtml, int maxLength = 100)
        {
            string snip;

            if (!string.IsNullOrWhiteSpace(contentHtml))
                snip = StringHelper.StripHtml(contentHtml);
            else if (!string.IsNullOrWhiteSpace(contentText))
                snip = StringHelper.StripMarkdown(contentText, true);
            else
                snip = null;

            if (!string.IsNullOrWhiteSpace(snip))
            {
                if (snip.Length > maxLength)
                    snip = snip.Substring(0, maxLength - 3) + "...";
            }
            else
            {
                snip = "None";
            }

            return snip;
        }

        private bool IsSnipEmpty(MultilingualString value) =>
            value.All(x => string.IsNullOrWhiteSpace(x.Value) || x.Value == "None");

        public IEnumerable<MultilingualString> GetStrings()
        {
            yield return Text;
            yield return Html;
            yield return Snip;
        }

        public void CreateSnip()
        {
            var languages = Languages;
            foreach (var language in languages)
                Snip[language] = GetSnip(Text[language], Html[language]);

            foreach (var language in Snip.Languages.Where(x => !languages.Contains(x)).ToArray())
                Snip.Remove(language);
        }

        public string[] Languages => Text.Languages.Concat(Html.Languages).Distinct().ToArray();
    }
}
