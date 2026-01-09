using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using InSite.Application.Glossaries.Read;
using InSite.Application.Glossaries.Write;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assets.Glossaries.Utilities
{
    [Serializable]
    public sealed class GlossaryHelper
    {
        #region Static

        private static readonly ConcurrentDictionary<Guid, Guid> _organizationGlossaries = new ConcurrentDictionary<Guid, Guid>();

        public static Guid GlossaryIdentifier => _organizationGlossaries.GetOrAdd(CurrentSessionState.Identity.Organization.Identifier, (organizationId) =>
        {
            var organization = OrganizationSearch.Select(organizationId);

            if (!organization.GlossaryIdentifier.HasValue)
            {
                var command = new InitializeGlossary(UniqueIdentifier.Create());

                ServiceLocator.SendCommand(command);

                OrganizationSearch.Refresh(organization.Identifier);

                organization = OrganizationSearch.Select(organizationId);
            }

            return organization.GlossaryIdentifier.Value;
        });

        #endregion

        #region Classes

        public interface ITerm
        {
            Guid TermID { get; set; }
            string ReferenceID { get; }
            int RefCount { get; }
            string Name { get; }
            string Title { get; }
        }

        [Serializable]
        private class TermInfo : ITerm
        {
            public Guid TermID { get; set; }

            public string ReferenceID { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public ContentContainerItem Description { get; set; }
            public int RefCount { get; set; }
        }

        public class GapInfo
        {
            public int Start { get; private set; }
            public int End { get; private set; }

            public GapInfo(int start, int end)
            {
                Start = start;
                End = end;
            }

            public void Move(int offset)
            {
                Start += offset;
                End += offset;
            }

            public bool Contains(int index) => index >= Start && index < End;
        }

        public class LocalGapInfo
        {
            public int Start { get; private set; }
            public int OldLength { get; private set; }
            public int NewLength { get; private set; }

            public LocalGapInfo(int start, int oldLength, int newLength)
            {
                Start = start;
                OldLength = oldLength;
                NewLength = newLength;
            }

            public bool Contains(int index) => index >= Start && index < (Start + OldLength);
        }

        [JsonObject(MemberSerialization.OptIn)]
        private abstract class JsonDictionaryItem
        {
            #region Classes

            [JsonObject(MemberSerialization.OptIn)]
            public class SingleLanguage : JsonDictionaryItem
            {
                [JsonProperty(PropertyName = "descr")]
                public string Description { get; set; }

                public SingleLanguage(TermInfo info, string lang)
                    : base(info)
                {
                    Description = info.Description.GetHtml(lang)
                        .IfNullOrEmpty(() => info.Description.GetHtml(ContentContainer.DefaultLanguage));
                }
            }

            [JsonObject(MemberSerialization.OptIn)]
            public class MultiLanguage : JsonDictionaryItem
            {
                [JsonProperty(PropertyName = "descr")]
                public Dictionary<string, string> Description { get; set; }

                public MultiLanguage(TermInfo info)
                    : base(info)
                {
                    Description = info.Description.Languages
                        .ToDictionary(lang => lang, lang => info.Description.GetHtml(lang));
                }
            }

            #endregion

            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }

            public JsonDictionaryItem(TermInfo info)
            {
                Title = info.Title;
            }

            public static JsonDictionaryItem Get(TermInfo info)
            {
                return new MultiLanguage(info);
            }

            public static JsonDictionaryItem Get(TermInfo info, string lang)
            {
                return new SingleLanguage(info, lang);
            }
        }

        #endregion

        #region Constants

        private static readonly Regex MdLinkPatternRegex = new Regex(
            "(?<Img>!?)\\[(?<Title>[^\\]]+)\\]\\((?<URL>[^\\)]+)\\)",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex HtmlLinkPatternRegex = new Regex(
            "\\<a\\s+(?<Params>.*?)\\>(?<Content>.*?)\\</a\\>",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex HtmlTagParametersPatternRegex = new Regex(
            "(?<Name>[a-z-]+)\\s*=\\s*(?:\"(?<Value>[^\"]+)\"|'(?<Value>[^']+)')",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        #endregion

        #region Properties

        public string ID => _clientId;

        public string Language => _lang;

        public IReadOnlyList<ITerm> Dictionary => _dictionary.Values.Cast<ITerm>().ToArray();

        #endregion

        #region Fields

        private Guid _glossaryId;
        private string _lang;
        private bool _isMultiLang;
        private string _clientId;

        private HashSet<string> _ids;
        private RandomStringGenerator _idGenerator;
        private Dictionary<Guid, TermInfo> _dictionary;
        private Dictionary<Guid, TermInfo[]> _containerMapping;

        #endregion

        #region Construction

        public GlossaryHelper(string language, bool isMultiLang = false)
            : this(GlossaryIdentifier, language, isMultiLang)
        {

        }

        public GlossaryHelper(Guid glossary, string language, bool isMultiLang = false)
        {
            if (language.IsEmpty())
                throw new ArgumentNullException(language);

            _dictionary = new Dictionary<Guid, TermInfo>();
            _idGenerator = new RandomStringGenerator(RandomStringType.AlphanumericCaseSensitive, 4);
            _ids = new HashSet<string>();
            _containerMapping = new Dictionary<Guid, TermInfo[]>();

            _glossaryId = glossary;
            _lang = language;
            _isMultiLang = isMultiLang;
            _clientId = _idGenerator.Next();
        }

        #endregion

        #region Markdown

        public string Process(Guid containerId, string label, string input)
        {
            if (input.IsEmpty())
                return input;

            TermInfo[] infos;

            if (!_containerMapping.ContainsKey(containerId))
            {
                var terms = ServiceLocator.GlossarySearch.GetContainerTerms(_glossaryId, containerId, label);

                AddItems(terms);

                infos = terms.Select(x => _dictionary[x.TermIdentifier]).ToArray();

                _containerMapping.Add(containerId, infos);
            }
            else
            {
                infos = _containerMapping[containerId];
            }

            return Process(infos, input);
        }

        public string Process(IEnumerable<QGlossaryTerm> terms, string input)
        {
            if (terms == null)
                throw new ArgumentNullException(nameof(terms));

            AddItems(terms);

            var infos = terms.Select(x => _dictionary[x.TermIdentifier]).ToArray();

            return Process(infos, input);
        }

        private void AddItems(IEnumerable<QGlossaryTerm> terms)
        {
            var newTerms = terms.Where(x => !_dictionary.ContainsKey(x.TermIdentifier)).ToArray();
            if (newTerms.Length == 0)
                return;

            var languages = _isMultiLang
                ? null
                : _lang == ContentContainer.DefaultLanguage
                    ? new[] { _lang }
                    : new[] { _lang, Shift.Common.ContentContainer.DefaultLanguage };

            var contents = ServiceLocator.ContentSearch.GetBlocks(
                newTerms.Select(x => x.TermIdentifier),
                languages,
                new[] { ContentLabel.Title, ContentLabel.Description });

            foreach (var term in newTerms)
            {
                var info = new TermInfo
                {
                    ReferenceID = GetClientId(),
                    TermID = term.TermIdentifier,
                    Name = term.TermName
                };

                if (contents.TryGetValue(term.TermIdentifier, out var content))
                {
                    info.Title = content.Title.GetText(_lang, true).IfNullOrEmpty(info.Name);
                    info.Description = content.Description;
                }

                _dictionary.Add(term.TermIdentifier, info);
            }
        }

        private string Process(TermInfo[] infos, string input)
        {
            if (input.IsEmpty())
                return input;

            var gaps = new List<GapInfo>();
            var localGaps = new Queue<LocalGapInfo>();
            var directLinks = new HashSet<Guid>();

            var output = MdLinkPatternRegex.Replace(input, (Match m) =>
            {
                var result = m.Value;
                if (gaps.Any(x => x.Contains(m.Index)) || localGaps.Any(x => x.Contains(m.Index)))
                    return result;

                if (m.Groups["Img"].Value != "!")
                {
                    var urlGroup = m.Groups["URL"];

                    var info = GetTermInfoByGroup(urlGroup);
                    if (info != null)
                    {
                        info.RefCount++;

                        directLinks.Add(info.TermID);

                        var index = urlGroup.Index - m.Index;

                        result = result.Substring(0, index) + info.ReferenceID + result.Substring(index + urlGroup.Length);
                    }
                }

                localGaps.Enqueue(new LocalGapInfo(m.Index, m.Length, result.Length));

                return result;
            });

            AddGaps();

            output = HtmlLinkPatternRegex.Replace(output, (Match m1) =>
            {
                var result1 = m1.Value;
                if (gaps.Any(x => x.Contains(m1.Index)) || localGaps.Any(x => x.Contains(m1.Index)))
                    return result1;

                result1 = HtmlTagParametersPatternRegex.Replace(result1, (Match m2) =>
                {
                    var result2 = m2.Value;

                    if (!string.Equals(m2.Groups["Name"].Value, "href", StringComparison.OrdinalIgnoreCase))
                        return result2;

                    var valueGroup = m2.Groups["Value"];

                    var info = GetTermInfoByGroup(valueGroup);
                    if (info == null)
                        return result2;

                    info.RefCount++;

                    directLinks.Add(info.TermID);

                    var index = valueGroup.Index - m2.Index;

                    return result2.Substring(0, index) + info.ReferenceID + result2.Substring(index + valueGroup.Length);
                });

                localGaps.Enqueue(new LocalGapInfo(m1.Index, m1.Length, result1.Length));

                return result1;
            });

            AddGaps();

            infos = infos.Where(x => !directLinks.Contains(x.TermID)).ToArray();

            foreach (var info in infos)
            {
                var namePattern = info.Title.IsEmpty() || string.Equals(info.Name, info.Title)
                    ? Regex.Escape(info.Name)
                    : Regex.Escape(info.Name) + "|" + Regex.Escape(info.Title);
                var pattern = new Regex(
                    $"(?<=(?:\\W|^))(?:{namePattern})(?=(?:\\W|$))",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);

                output = pattern.Replace(output, (Match m) =>
                {
                    var result = m.Value;

                    if (gaps.Any(x => x.Contains(m.Index)) || localGaps.Any(x => x.Contains(m.Index)))
                        return result;

                    result = $"<a href='{info.ReferenceID}'>{HttpUtility.HtmlEncode(result)}</a>";

                    info.RefCount++;

                    localGaps.Enqueue(new LocalGapInfo(m.Index, m.Length, result.Length));

                    return result;
                });

                AddGaps();
            }

            return output;

            TermInfo GetTermInfoByGroup(System.Text.RegularExpressions.Group group)
            {
                if (group.Length <= 5)
                    return null;

                var urlValue = group.Value;

                return urlValue.StartsWith("term:") ? GetTermInfoByName(urlValue.Substring(5)) : null;
            }

            TermInfo GetTermInfoByName(string name)
            {
                return infos
                    .Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
            }

            void AddGaps()
            {
                var accumOffset = 0;

                while (localGaps.Count > 0)
                {
                    var lg = localGaps.Dequeue();

                    var offset = lg.NewLength - lg.OldLength;
                    if (offset != 0)
                    {
                        foreach (var g in gaps)
                            if (g.Start > lg.Start)
                                g.Move(offset);
                    }

                    var start = lg.Start + accumOffset;

                    gaps.Add(new GapInfo(start, start + lg.NewLength));

                    accumOffset += offset;
                }
            }
        }

        public string GetJsonDictionary() => JsonConvert.SerializeObject(GetOutputDictionary());

        public string GetJavaScriptDictionary() => JsonHelper.SerializeJsObject(GetOutputDictionary());

        private Dictionary<string, JsonDictionaryItem> GetOutputDictionary()
        {
            if (_isMultiLang)
            {
                return _dictionary.Values.ToDictionary(
                    x => x.ReferenceID,
                    x => JsonDictionaryItem.Get(x)
                );
            }
            else
            {
                return _dictionary.Values.ToDictionary(
                    x => x.ReferenceID,
                    x => JsonDictionaryItem.Get(x, _lang)
                );
            }
        }

        #endregion

        #region Helper methods

        private string GetClientId()
        {
            for (var i = 0; i < 100; i++)
            {
                var id = _idGenerator.Next();
                if (_ids.Add(id))
                    return _clientId + ":" + id;
            }

            throw ApplicationError.Create("Can't generate unique ID");
        }

        #endregion
    }
}