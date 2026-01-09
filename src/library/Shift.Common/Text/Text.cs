using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common.Base
{
    [Serializable]
    public sealed class Text : IEnumerable<KeyValuePair<string, string>>
    {
        #region Properties

        public const string DefaultLanguage = "en";

        public string[] SupportedLanguages = new[] { "en", "ar", "de", "eo", "es", "fr", "he", "it", "ja", "ko", "la", "nl", "no", "pa", "pl", "pt", "ru", "sv", "uk", "zh" };

        public string Default
        {
            get => this[DefaultLanguage];
            set => this[DefaultLanguage] = value;
        }

        public string this[string language]
        {
            get => _items.ContainsKey(language) ? _items[language] : null;
            set => _items[language] = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        public string this[string language, string dialect]
        {
            get => this[GetKey(language, dialect)] ?? this[language];
            set => this[GetKey(language, dialect)] = value;
        }

        public int Count => _items.Count;

        public IEnumerable<string> Languages => _items.Keys.OrderBy(x => x);

        public bool IsEmpty => !_items.Any(x => !string.IsNullOrWhiteSpace(x.Value));

        #endregion

        #region Fields

        private readonly Dictionary<string, string> _items;

        #endregion

        #region Construction

        public Text()
        {
            _items = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        private Text(Text source)
            : this()
        {
            if (source == null)
                return;

            foreach (var kv in source._items)
                _items.Add(kv.Key, string.IsNullOrWhiteSpace(kv.Value) ? string.Empty : kv.Value);
        }

        public Text Clone()
            => new Text(this);

        #endregion

        #region Methods

        public bool Exists(string language)
        {
            return _items.ContainsKey(language);
        }

        public string Get(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = DefaultLanguage;

            return _items.TryGetValue(language, out var value) && string.IsNullOrWhiteSpace(value)
                ? value
                : Default;
        }

        public void Set(Text value)
        {
            _items.Clear();

            if (value == null || value._items.Count == 0)
                return;

            foreach (var kv in value._items)
                _items.Add(kv.Key, string.IsNullOrWhiteSpace(kv.Value) ? string.Empty : kv.Value);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public static string GetKey(string language, string dialect)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentNullException(nameof(language));

            if (string.IsNullOrWhiteSpace(dialect))
                return language;

            return $"{language}:{dialect}";
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

        public bool IsEqual(Text other)
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