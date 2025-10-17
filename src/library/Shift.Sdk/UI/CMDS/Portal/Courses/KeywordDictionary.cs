using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class KeywordDictionary
    {
        #region Classes

        [Serializable]
        private class KeywordInfo
        {
            public string Value { get; private set; }

            public KeywordInfo(string value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return Value;
            }
        }

        [Serializable]
        private class KeywordLinkInfo : IEnumerable<KeywordLinkInfo>
        {
            #region Properties

            public KeywordInfo Keyword { get; private set; }
            public int OccurrenceCount { get; set; }

            public KeywordLinkInfo this[string key] => _dict.ContainsKey(key) ? _dict[key] : null;

            #endregion

            #region Fields

            private Dictionary<string, KeywordLinkInfo> _dict;

            #endregion

            #region Construction

            public KeywordLinkInfo(string keyword)
                : this(new KeywordInfo(keyword))
            {

            }

            public KeywordLinkInfo(KeywordInfo keyword)
            {
                Keyword = keyword;
                OccurrenceCount = 1;

                _dict = new Dictionary<string, KeywordLinkInfo>();
            }

            #endregion

            #region Methods

            public KeywordLinkInfo Add(string keyword)
            {
                KeywordLinkInfo result;

                if (!_dict.ContainsKey(keyword))
                {
                    result = new KeywordLinkInfo(keyword);

                    _dict.Add(keyword, result);
                }
                else
                {
                    result = _dict[keyword];

                    result.OccurrenceCount++;
                }

                return result;
            }

            public KeywordLinkInfo Add(KeywordInfo keyword)
            {
                KeywordLinkInfo result;

                if (!_dict.ContainsKey(keyword.Value))
                {
                    result = new KeywordLinkInfo(keyword);

                    _dict.Add(keyword.Value, result);
                }
                else
                {
                    result = _dict[keyword.Value];

                    result.OccurrenceCount++;
                }

                return result;
            }

            #endregion

            #region IEnumerable

            public IEnumerator<KeywordLinkInfo> GetEnumerator()
            {
                return _dict.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region Overriden methods

            public override string ToString()
            {
                return $"{Keyword} ({OccurrenceCount})";
            }

            #endregion
        }

        #endregion

        #region Fields

        private static readonly HashSet<string> ExcludeWords = new HashSet<string> { "and", "the", "are", "don", "for" };

        private readonly KeywordLinkInfo _root = new KeywordLinkInfo("root");

        #endregion

        #region Methods

        public void AddKeywords(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            var keywords = new List<KeywordLinkInfo>();
            var sb = new StringBuilder();

            for (var i = 0; i < value.Length; i++)
            {
                var ch = value[i];
                var isLetter = char.IsLetter(ch);

                if (isLetter)
                    sb.Append(ch);

                if ((!isLetter || i == value.Length - 1) && sb.Length > 0)
                {
                    if (sb.Length > 2)
                    {
                        var word = sb.ToString().ToLower();
                        // word = Inflector.MakeSingular(word);

                        if (!ExcludeWords.Contains(word))
                        {
                            var keyword = _root.Add(word);
                            keywords.Add(keyword);
                        }
                    }

                    sb.Clear();
                }
            }

            for (var x = 0; x < keywords.Count; x++)
            {
                var keyword = keywords[x];

                for (var y = 0; y < keywords.Count; y++)
                {
                    if (x == y || keyword.Keyword.Value == keywords[y].Keyword.Value)
                        continue;

                    keyword.Add(keywords[y].Keyword);
                }
            }
        }

        public string[] GetSuggestions(string input, int count = 8)
        {
            var filter = ParseKeywords(input, true);
            var currentKeywords = filter.Length <= 1
                ? _root
                : GetKeywords(filter, _root);

            var keyword = filter.Length > 0 ? filter[filter.Length - 1] : null;
            IEnumerable<KeywordLinkInfo> data = string.IsNullOrEmpty(keyword)
                ? currentKeywords
                : currentKeywords.Where(x => (x.Keyword.Value.Length - x.Keyword.Value.Replace(keyword, string.Empty).Length) / keyword.Length > 0);

            return data
                .OrderByDescending(x => x.OccurrenceCount)
                .Select(x => x.Keyword.Value)
                .Take(count)
                .OrderBy(x => x)
                .ToArray();
        }

        public static string[] ParseKeywords( string input, bool includeEmpty )
        {
            var words = new List<string>( );
            var sb = new StringBuilder( );

            for (var i = 0; i < input.Length; i++)
            {
                var ch = input[i];
                var isLetter = char.IsLetter( ch );

                if (isLetter)
                    sb.Append( ch );

                if ((!isLetter || i == input.Length - 1) && sb.Length > 0)
                {
                    words.Add( sb.ToString( ).ToLower( ) );

                    sb.Clear( );
                }
            }

            if (includeEmpty && input.Length > 0 && !char.IsLetter( input[input.Length - 1] ))
                words.Add( string.Empty );

            return words.ToArray( );
        }

        #endregion

        #region Helpers

        private static IEnumerable<KeywordLinkInfo> GetKeywords(IReadOnlyList<string> filter, KeywordLinkInfo root)
        {
            var result = new List<KeywordLinkInfo>();

            for (var x = 0; x < filter.Count - 1; x++)
            {
                var currentKeyword = root[filter[x]];
                if (currentKeyword == null)
                    return new KeywordLinkInfo[0];

                if (x == 0)
                {
                    foreach (var link in currentKeyword)
                    {
                        result.Add(
                            new KeywordLinkInfo(link.Keyword)
                            {
                                OccurrenceCount = link.OccurrenceCount
                            });
                    }
                }
                else
                {
                    for (var y = 0; y < result.Count; y++)
                    {
                        var link = currentKeyword[result[y].Keyword.Value];
                        if (link == null)
                        {
                            result.RemoveAt(y);
                            y--;
                        }
                        else
                        {
                            result[y].OccurrenceCount += link.OccurrenceCount;
                        }
                    }
                }
            }

            return result.ToArray();
        }

        #endregion
    }
}
