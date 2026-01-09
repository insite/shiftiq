using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Shift.Sdk.UI.Help
{
    public class Lexicon
    {
        private Dictionary<string, Dictionary<string, string>> _entries;

        public Lexicon()
        {
            _entries = new Dictionary<string, Dictionary<string, string>>();
        }

        public void AddEntry(string key, string language, string display)
        {
            if (!_entries.ContainsKey(key))
            {
                _entries[key] = new Dictionary<string, string>();
            }

            _entries[key][language] = display;
        }

        public string GetDisplay(string key, string language)
        {
            if (_entries.TryGetValue(key, out var languages))
            {
                if (languages.TryGetValue(language, out var translation))
                {
                    return translation;
                }
            }

            return key;
        }

        public Dictionary<string, string> GetDisplays(string key)
        {
            return _entries.TryGetValue(key, out var languages)
                ? new Dictionary<string, string>(languages)
                : new Dictionary<string, string>();
        }

        public IEnumerable<string> GetKeys()
        {
            return _entries.Keys;
        }

        public string ToJson(bool indented = true)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = indented,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return JsonSerializer.Serialize(_entries, options);
        }

        public void SaveToFile(string filePath, bool indented = true)
        {
            var json = ToJson(indented);
            File.WriteAllText(filePath, json);
        }

        public static Lexicon FromJson(string json)
        {
            var lexicon = new Lexicon();
            var entries = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);

            if (entries != null)
            {
                lexicon._entries = entries;
            }

            return lexicon;
        }

        public static Lexicon LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return FromJson(json);
        }

        public Dictionary<string, Dictionary<string, string>> GetRawEntries()
        {
            return new Dictionary<string, Dictionary<string, string>>(_entries);
        }
    }
}