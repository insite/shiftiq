using System.Diagnostics;
using System.Web;

namespace InSite
{
    public static class PageDownloadTimer
    {
        private static readonly string _stopwatchKey = typeof(PageDownloadTimer).FullName + ".Stopwatch";

        public static void Start()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            HttpContext.Current.Items[_stopwatchKey] = stopwatch;
        }

        public static long GetLoadingTime()
        {
            var stopwatch = (Stopwatch)HttpContext.Current.Items[_stopwatchKey];

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }
    }
}
