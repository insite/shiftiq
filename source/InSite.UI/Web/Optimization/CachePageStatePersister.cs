using System;
using System.IO;
using System.Web.Caching;
using System.Web.UI;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Web.Optimization
{
    public class CachePageStatePersister : PageStatePersister
    {
        private static readonly object Locker = new object();

        private const string VSKEY = "__VSKEY";
        private const string VSPREFIX = "VIEWSTATE_";

        public CachePageStatePersister(Page page) : base(page) { }

        /// <summary>
        /// Overridden by derived classes to deserialize and load persisted state information when 
        /// a page initializes its control hierarchy.
        /// <para>
        /// 1. Get the viewstate key from the hidden form field
        /// 2. Retrieved the serialized viewstate object from the physical cache
        /// 3. deserialize the vs into a pair object
        /// 4. populate our viewstate and control state objects in the page
        /// </para>
        /// </summary>
        public override void Load()
        {
            if (!Page.IsPostBack) // Don't load anything if this is an inital request
                return;

            var vsKey = Page.Request.Form[VSKEY];

            // Sanity Checks
            if (vsKey.IsEmpty() || !vsKey.StartsWith(VSPREFIX))
                throw new ViewStateException();

            Pair stateGraph = null;

            var fileName = (string)Page.Cache[vsKey];
            if (fileName.IsNotEmpty() && File.Exists(fileName))
            {
                var formatter = (ObjectStateFormatter)StateFormatter;
                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                    stateGraph = formatter.Deserialize(stream) as Pair;
            }

            if (stateGraph == null)
            {
                var isE03 = ServiceLocator.Partition.IsE03();
                var id = CurrentSessionState.Identity;
                var url = HttpResponseHelper.BuildUrl(
                    ServiceLocator.Urls.GetHomeUrl(id.User?.AccessGrantedToCmds ?? false, isE03, id.IsAdministrator),
                    "referrer-reason=expired-viewstate");

                AdminBasePage.RedirectToUrl(Page, url);
            }
            else
            {
                ViewState = stateGraph.First;
                ControlState = stateGraph.Second;
            }
        }

        /// <summary>
        /// Overridden by derived classes to serialize persisted state information when a page is unloaded from memory.
        /// <para>
        /// 1. Save our viewstate and controlstate to a new pair object
        /// 2. Serialize the pair object to a string
        /// 3. Save the string to persistant storage (cache folder)
        /// 4. save the pointer to the file in Page Cache
        /// 5. Save the cache pointer to a hidden field object on the page
        /// </para>
        /// </summary>
        public override void Save()
        {
            if (ViewState == null && ControlState == null)
                return;

            if (Page.Session == null)
                throw new InvalidOperationException("Session is required for CachePageStatePersister (SessionID -> Key)");

            string vsKey;
            string cacheFile;

            if (!Page.IsPostBack) // create a unique cache file and key based on the user's session and page instance (time)
            {
                vsKey = $"{VSPREFIX}{Page.Request.RawUrl}_{Page.Session.SessionID}_{DateTime.Now.Ticks}";

                var cachePath = Path.Combine(ServiceLocator.FilePaths.TempFolderPath, "Sessions", "ViewState");

                if (!Directory.Exists(cachePath))
                    Directory.CreateDirectory(cachePath);

                cacheFile = Path.Combine(cachePath, BuildFileName(cachePath));
            }
            else // get vs key from the page and the cache file
            {
                vsKey = Page.Request.Form[VSKEY];
                if (vsKey.IsEmpty())
                    throw new ViewStateException();

                cacheFile = (string)Page.Cache[vsKey];

                if (cacheFile.IsEmpty())
                    throw new ViewStateException();
            }

            lock (Locker)
            {
                var formatter = (ObjectStateFormatter)StateFormatter;
                var stateGraph = new Pair(ViewState, ControlState);

                using (var stream = File.Open(cacheFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    formatter.Serialize(stream, stateGraph);
            }

            Page.Cache.Add(
                vsKey,
                cacheFile,
                null,
                System.Web.Caching.Cache.NoAbsoluteExpiration,
                TimeSpan.FromMinutes(Page.Session.Timeout),
                CacheItemPriority.Normal,
                ViewStateCacheRemoveCallback
            );

            Page.ClientScript.RegisterHiddenField(VSKEY, vsKey);
        }

        /// <summary>
        /// Removes the persisted storage of the cache object from the cache folder.
        /// </summary>
        public static void ViewStateCacheRemoveCallback(string key, object value, CacheItemRemovedReason reason)
        {
            string cacheFile = value as string;
            if (!string.IsNullOrEmpty(cacheFile))
                if (File.Exists(cacheFile))
                    File.Delete(cacheFile);
        }

        /// <summary>
        /// Builds a valid file name for our persistant cache storage based on sessionid and requested path.
        /// </summary>
        private string BuildFileName(string cachePath)
        {
            const int maxPathLength = 259;
            const string extension = ".cache";

            var maxFileNameLength = maxPathLength - cachePath.Length - 1 - extension.Length;

            var parser = new UrlParser();
            var url = parser.Parse(Page.Request.RawUrl);

            var ticks = DateTime.Now.Ticks;
            var fileName = GetFileNameWithoutExt(url.Resource, ticks);
            if (fileName.Length > maxFileNameLength)
            {
                var resource = url.Resource.Substring(0, url.Resource.Length - (fileName.Length - maxFileNameLength));
                fileName = GetFileNameWithoutExt(resource, ticks);
            }

            var badChars = Path.GetInvalidPathChars();
            foreach (char c in badChars)
                fileName = fileName.Replace(c, '-');

            badChars = Path.GetInvalidFileNameChars();
            foreach (char c in badChars)
                fileName = fileName.Replace(c, '_');

            return string.Concat(fileName, extension);

            string GetFileNameWithoutExt(string resource, long nameTicks)
                => string.Format("{0}{1}__{2}__{3}", VSPREFIX, Page.Session.SessionID, resource, nameTicks);
        }
    }
}