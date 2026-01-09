using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    public class AnswerSectionsContainer
    {
        #region Classes

        [Serializable]
        private class SectionList : List<(int Index, string Html)>
        {
            public SectionList() : base() { }

            public SectionList(int capacity) : base(capacity) { }

            public SectionList(IEnumerable<(int Index, string Html)> items) : base(items) { }
        }

        #endregion

        public const string ErrorInvalidType = "Invalid type";
        public const string ErrorSectionStorageEmpty = "Sections session storage is empty";
        public const string ErrorSectionNotFound = "Sections not found";

        public int Count => _sections.Count;

        private SectionList _sections = new SectionList();
        private static readonly Regex _questionPattern = new Regex("(?:\\<!--question start (?<Index>[0-9]+)--\\>)(?<Html>.+?)(?:\\<!--question end--\\>)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly string _storageSessionKey = typeof(AnswerSectionsContainer).FullName + ".Storage";

        public AnswerSectionsContainer()
        {
            _sections = new SectionList();
        }

        private AnswerSectionsContainer(SectionList sections)
        {
            _sections = sections;
        }

        public void Add(int index, string html)
        {
            _sections.Add((index, html));
        }

        public (int NavItemIndex, string Html) Get(int index)
        {
            for (var i = 0; i < _sections.Count; i++)
            {
                var section = _sections[i];
                if (section.Index == index)
                    return (i, section.Html);
            }

            return default;
        }

        public (int NavItemIndex, string Html) Get(int section, int question)
        {
            var sectionData = Get(section);
            if (sectionData == default)
                return sectionData;

            var result = _questionPattern.Replace(sectionData.Html, (m) =>
            {
                var index = int.Parse(m.Groups["Index"].Value);

                return question == index ? m.Value : null;
            });

            return (sectionData.NavItemIndex, result);
        }

        private static string GetKey(Guid attemptId, Guid userId)
        {
            return $"{EncryptionKey.Default} {attemptId} {userId}";
        }

        public string Serialize(Guid attemptId, Guid userId)
        {
            return StringHelper.EncodeBase64(GetKey(attemptId, userId), stream =>
            {
                using (var zipStream = new GZipStream(stream, CompressionLevel.Fastest))
                {
                    using (var writer = new BinaryWriter(zipStream))
                    {
                        writer.Write(typeof(AnswerSectionsContainer).FullName);
                        writer.Write(_sections.Count);

                        foreach (var section in _sections)
                        {
                            writer.Write(section.Index);
                            writer.Write(section.Html);
                        }
                    }
                }
            });
        }

        public static AnswerSectionsContainer Deserialize(Guid attemptId, Guid userId, string data)
        {
            return (AnswerSectionsContainer)StringHelper.DecodeBase64(data, GetKey(attemptId, userId), stream =>
            {
                using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (var reader = new BinaryReader(zipStream))
                    {
                        var type = reader.ReadString();
                        if (type != typeof(AnswerSectionsContainer).FullName)
                            throw ApplicationError.Create(ErrorInvalidType);

                        var count = reader.ReadInt32();
                        var sections = new SectionList(count);

                        for (var i = 0; i < count; i++)
                            sections.Add((reader.ReadInt32(), reader.ReadString()));

                        return new AnswerSectionsContainer(sections);
                    }
                }
            });
        }

        public void Save(Guid attemptId, string sessionId)
        {
            if (attemptId == default)
                throw new ArgumentNullException(nameof(attemptId));

            if (sessionId.IsEmpty())
                throw new ArgumentNullException(nameof(sessionId));

            var storage = GetSessionStorage();
            storage[(attemptId, sessionId)] = new SectionList(_sections);
        }

        public static AnswerSectionsContainer Load(Guid attemptId, string sessionId)
        {
            if (attemptId == default)
                throw new ArgumentNullException(nameof(attemptId));

            if (sessionId.IsEmpty())
                throw new ArgumentNullException(nameof(sessionId));

            var storage = GetSessionStorage();
            if (storage.Count == 0)
                throw ApplicationError.Create(ErrorSectionStorageEmpty);

            var sections = storage.GetOrDefault((attemptId, sessionId))
                ?? throw ApplicationError.Create(ErrorSectionNotFound);

            return new AnswerSectionsContainer(sections);
        }

        public static bool Remove(Guid attemptId)
        {
            if (attemptId == default)
                throw new ArgumentNullException(nameof(attemptId));

            var storage = GetSessionStorage();
            var keys = storage.Keys.Where(x => x.Item1 == attemptId).ToArray();

            foreach (var key in keys)
                storage.Remove(key);

            return keys.Length > 0;
        }

        public static bool Remove(Guid attemptId, string sessionId)
        {
            if (attemptId == default)
                throw new ArgumentNullException(nameof(attemptId));

            if (sessionId.IsEmpty())
                throw new ArgumentNullException(nameof(sessionId));

            var storage = GetSessionStorage();
            return storage.Remove((attemptId, sessionId));
        }

        public static bool RemoveExcept(Guid attemptId, string exceptSessionId)
        {
            if (attemptId == default)
                throw new ArgumentNullException(nameof(attemptId));

            if (exceptSessionId.IsEmpty())
                throw new ArgumentNullException(nameof(exceptSessionId));

            var storage = GetSessionStorage();
            var keys = storage.Keys.Where(x => x.Item1 == attemptId && x.Item2 != exceptSessionId).ToArray();

            foreach (var key in keys)
                storage.Remove(key);

            return keys.Length > 0;
        }

        private static Dictionary<(Guid, string), SectionList> GetSessionStorage()
        {
            return (Dictionary<(Guid, string), SectionList>)(HttpContext.Current.Session[_storageSessionKey]
                ?? (HttpContext.Current.Session[_storageSessionKey] = new Dictionary<(Guid, string), SectionList>()));
        }
    }
}