using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.UI;

using InSite.Common.Text;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

namespace InSite.Web.Optimization
{
    public class CachePageStatePersister_Refactored : PageStatePersister
    {
        private const string VSKEY = "__VSKEY";
        private const string VSPREFIX = "VIEWSTATE_";

        private static double StorageTimeout;
        private static string StoragePath;
        private static ConcurrentQueue<ViewStateKey> StorageFiles;

        private ViewStateKey _currentKey;

        public CachePageStatePersister_Refactored(Page page) : base(page) { }

        public static void Initialize()
        {
            StoragePath = Path.Combine(ServiceLocator.FilePaths.TempFolderPath, "Sessions", "ViewState");
            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);

            var sessionStateSection = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");
            StorageTimeout = sessionStateSection.Timeout.TotalMinutes;

            LoadStorageFiles();
            DeleteExpiredFiles();
        }

        public override void Load()
        {
            if (!Page.IsPostBack) // Don't load anything if this is an inital request
                return;

            var fileName = Page.Request.Form[VSKEY];

            _currentKey = ViewStateKey.ReadFileName(fileName);

            if (_currentKey == null || _currentKey.SessionID != Page.Session.SessionID)
                throw new ViewStateException();

            Pair stateGraph = null;

            var filePath = GetFilePath(fileName);
            if (File.Exists(filePath))
            {
                try
                {
                    var formatter = (ObjectStateFormatter)StateFormatter;
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                        stateGraph = (Pair)formatter.Deserialize(stream);
                }
                catch (IOException ioex)
                {
                    // Ignore an I/O error in case the system tries to read and delete a file at the same time

                    // Aug 1, 2024 - Oleg: Temporary, to make sure the code works correctly
                    AppSentry.SentryError(ioex);
                }
            }

            if (stateGraph != null)
            {
                ViewState = stateGraph.First;
                ControlState = stateGraph.Second;

                DeleteFile(_currentKey.NextKey());
            }
            else
            {
                OnViewStateExpired();
            }
        }

        public override void Save()
        {
            if (ViewState == null && ControlState == null)
                return;

            if (Page.Session == null)
                throw new InvalidOperationException("Session is required for CachePageStatePersister (SessionID -> Key)");

            var isNew = _currentKey == null;
            var key = isNew ? new ViewStateKey(Page.Session.SessionID) : _currentKey.NextKey();
            var fileName = key.BuildFileName();
            var filePath = GetFilePath(fileName);
            var stateGraph = new Pair(ViewState, ControlState);
            var formatter = (ObjectStateFormatter)StateFormatter;

            using (var stream = File.Open(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                formatter.Serialize(stream, stateGraph);

            if (!isNew && _currentKey.Version > 1)
                DeleteFile(_currentKey.PreviousKey());

            StorageFiles.Enqueue(key);

            ScriptManager.RegisterHiddenField(Page, VSKEY, fileName);
        }

        private void OnViewStateExpired()
        {
            var isE03 = ServiceLocator.AppSettings.IsE03;

            var id = CurrentSessionState.Identity;

            var url = HttpResponseHelper.BuildUrl(
                ServiceLocator.Urls.GetHomeUrl(id.User?.AccessGrantedToCmds ?? false, isE03, id.IsAdministrator),
                "referrer-reason=expired-viewstate");

            AdminBasePage.Redirect(Page, url);
        }

        private class ViewStateKey
        {
            public string SessionID { get; }
            public long Timestamp { get; }
            public int Version { get; }
            public DateTime UtcCreated { get; set; }

            public ViewStateKey(string sessionId)
            {
                var now = DateTime.UtcNow;

                SessionID = sessionId;
                Timestamp = now.Ticks;
                Version = 1;
                UtcCreated = now;
            }

            private ViewStateKey(string sessionId, long timestamp, int version)
            {
                SessionID = sessionId;
                Timestamp = timestamp;
                Version = version;
                UtcCreated = DateTime.UtcNow;
            }

            public ViewStateKey PreviousKey() =>
                new ViewStateKey(SessionID, Timestamp, Version - 1);

            public ViewStateKey NextKey() =>
                new ViewStateKey(SessionID, Timestamp, Version + 1);

            public string BuildFileName() =>
                VSPREFIX + SessionID + "." + Timestamp + "." + Version;

            public static ViewStateKey ReadFileName(string value)
            {
                if (value.IsEmpty() || !value.StartsWith(VSPREFIX))
                    return null;

                var parts = value.Substring(VSPREFIX.Length).Split('.');
                if (parts.Length != 3)
                    return null;

                if (!long.TryParse(parts[1], out var timestamp))
                    return null;

                if (!int.TryParse(parts[2], out var version))
                    return null;

                return new ViewStateKey(parts[0], timestamp, version);
            }
        }

        private static string GetFilePath(ViewStateKey key) => GetFilePath(key.BuildFileName());

        private static string GetFilePath(string fileName) => Path.Combine(StoragePath, fileName + ".cache");

        private static void DeleteFile(ViewStateKey key) => DeleteFile(key.BuildFileName());

        private static void DeleteFile(string fileName)
        {
            var filePath = GetFilePath(fileName);
            if (!File.Exists(filePath))
                return;

            try
            {
                File.Delete(filePath);
            }
            catch (IOException ioex)
            {
                // Ignore an I/O error in case the system tries to delete and read a file at the same time

                // Aug 1, 2024 - Oleg: Temporary, to make sure the code works correctly
                AppSentry.SentryError(ioex);
            }
        }

        private static void LoadStorageFiles()
        {
            var keys = new List<ViewStateKey>();

            foreach (var filePath in Directory.EnumerateFiles(StoragePath))
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var key = ViewStateKey.ReadFileName(fileName);

                if (key != null)
                {
                    key.UtcCreated = File.GetCreationTimeUtc(filePath);
                    keys.Add(key);
                }
                else
                    File.Delete(filePath);
            }

            StorageFiles = new ConcurrentQueue<ViewStateKey>(
                keys.OrderBy(x => x.UtcCreated).ThenBy(x => x.Timestamp).ThenBy(x => x.Version));
        }

        private static void DeleteExpiredFiles() =>
            DeleteExpiredFiles(typeof(CachePageStatePersister).FullName + "." + nameof(DeleteExpiredFiles), true, CacheItemRemovedReason.Expired);

        private static void DeleteExpiredFiles(string key, object value, CacheItemRemovedReason reason)
        {
            var expireFrom = DateTime.UtcNow.AddMinutes(-StorageTimeout);

            while (StorageFiles.TryPeek(out var peekItem) && peekItem.UtcCreated <= expireFrom)
            {
                StorageFiles.TryDequeue(out _);
                DeleteFile(peekItem);
            }

            if (reason != CacheItemRemovedReason.Removed)
                HttpRuntime.Cache.Add(
                    key,
                    value,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(1),
                    CacheItemPriority.Normal,
                    DeleteExpiredFiles
                );
        }
    }
}