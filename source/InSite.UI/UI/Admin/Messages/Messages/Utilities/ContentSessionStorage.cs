using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Admin.Messages.Messages.Utilities
{
    internal static class ContentSessionStorage
    {
        #region Construction

        static ContentSessionStorage()
        {
            _storagePath = Path.Combine(ServiceLocator.FilePaths.TempFolderPath, "Sessions", "Markdown");

            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);

            CleanUpStorage();
        }

        #endregion

        #region Constants

        private const int FileCleanUpTimeout = 5;
        private const int SessionLifetime = 2880;

        #endregion

        #region Properties

        private static Dictionary<Guid, ContentSession> SessionStateStorage
        {
            get => CurrentSessionState.MarkdownSessionStateStorage ?? (CurrentSessionState.MarkdownSessionStateStorage = new Dictionary<Guid, ContentSession>());
            set => CurrentSessionState.MarkdownSessionStateStorage = value;
        }

        #endregion

        #region Fields

        private static readonly object _syncRoot = new object();

        private static readonly string _storagePath;
        private static DateTime _nextCleanUpDate = DateTime.MinValue;

        #endregion

        #region Public methods

        public static ContentSession Create(Guid messageId, Guid userId, string organization)
        {
            CleanUpStorage();

            foreach (var key in SessionStateStorage.Where(x => x.Value == null).Select(x => x.Key).ToArray())
                SessionStateStorage.Remove(key);

            return CreateSessionFile(sessionId => ContentSession.Create(sessionId, userId, messageId, organization));
        }

        public static ContentSession Get(Guid sessionId, Guid userId)
        {
            if (SessionStateStorage.TryGetValue(sessionId, out var session))
                return session;

            session = LoadSessionFile(sessionId);

            if (session != null && session.UserIdentifier != userId)
                session = null;

            SessionStateStorage.Add(sessionId, session);

            return session;
        }

        public static void Set(ContentSession session)
        {
            SaveSessionFile(session, false);
        }

        public static void Remove(Guid sessionId, Guid userId)
        {
            var session = Get(sessionId, userId);
            if (session == null)
                return;

            RemoveSessionFile(sessionId);

            SessionStateStorage[sessionId] = null;
        }

        #endregion

        #region Session Managing

        private static ContentSession CreateSessionFile(Func<Guid, ContentSession> create)
        {
            lock (_syncRoot)
            {
                Guid key;

                for (var i = 0; ; i++)
                {
                    if (i > 20)
                        throw new Exception("Unable create session file");

                    key = Guid.NewGuid();

                    if (SessionStateStorage.ContainsKey(key))
                        continue;

                    var filepath = GetFilePath(key);
                    if (!File.Exists(filepath))
                        break;
                }

                var session = create(key);

                SaveSessionFile(session, true);

                return session;
            }
        }

        private static ContentSession LoadSessionFile(Guid sessionId)
        {
            var filePath = GetFilePath(sessionId);
            var json = string.Empty;

            lock (_syncRoot)
            {
                if (File.Exists(filePath))
                    json = File.ReadAllText(filePath);
            }

            return json.IsEmpty()
                ? null
                : JsonConvert.DeserializeObject<ContentSession>(json);
        }

        private static void SaveSessionFile(ContentSession session, bool create)
        {
            var filePath = GetFilePath(session.SessionIdentifier);
            var json = JsonConvert.SerializeObject(session);

            lock (_syncRoot)
            {
                if (create || File.Exists(filePath))
                    File.WriteAllText(filePath, json);
            }
        }

        private static void RemoveSessionFile(Guid sessionId)
        {
            var filepath = GetFilePath(sessionId);

            lock (_syncRoot)
            {
                if (File.Exists(filepath))
                    File.Delete(filepath);
            }
        }

        private static void CleanUpStorage()
        {
            if (DateTime.UtcNow < _nextCleanUpDate)
                return;

            lock (_syncRoot)
            {
                if (DateTime.UtcNow < _nextCleanUpDate)
                    return;

                var minDate = DateTime.UtcNow.AddMinutes(-SessionLifetime);

                foreach (var filename in Directory.EnumerateFiles(_storagePath))
                {
                    var isDelete = Path.GetExtension(filename) != ".json"
                        || !Guid.TryParse(Path.GetFileNameWithoutExtension(filename), out _)
                        || File.GetLastWriteTimeUtc(filename) <= minDate;

                    if (isDelete)
                        File.Delete(filename);
                }

                _nextCleanUpDate = DateTime.UtcNow.AddMinutes(FileCleanUpTimeout);
            }
        }

        #endregion

        #region Helper methods

        private static string GetFilePath(Guid key)
        {
            return Path.Combine(_storagePath, $"{key}.json");
        }

        #endregion
    }
}