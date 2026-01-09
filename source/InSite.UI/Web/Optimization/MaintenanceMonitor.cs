using System;
using System.Web;

using InSite.Common.Web;

using Newtonsoft.Json;

using Shift.Common;

using Cache = Shift.Common.MemoryCache<string, Shift.Common.Lockouts>;

namespace InSite.Web.Optimization
{
    public class MaintenanceMonitor
    {
        private static readonly int OneMinute = 60; // Seconds

        private static readonly Cache Cache = new Cache();

        public Lockouts Lockouts { get; set; }

        public MaintenanceMonitor()
        {
            var lockouts = ServiceLocator.AppSettings.Platform.Maintenance.Lockouts;

            if (lockouts == null || lockouts.Length == 0)
                return;

            var now = DateTimeOffset.Now;

            var enterprise = ServiceLocator.Partition.Slug;

            var environment = ServiceLocator.AppSettings.Environment.Name.ToString();

            var key = $"{enterprise} {environment}";

            if (Cache.Exists(key))
            {
                Lockouts = Cache.Get(key);
            }
            else
            {
                Lockouts = new Lockouts(lockouts, now, enterprise, environment);

                Cache.Add(key, Lockouts, OneMinute);
            }
        }

        public void FilterRequest(HttpContext context)
        {
            try
            {
                if (Lockouts == null)
                    return;

                if (Lockouts.State != LockoutState.Open)
                    return;

                if (Global.IsWebUIRequest(context.Request.RawUrl))
                    if (!Global.IsRequestIgnored(context))
                        if (Lockouts.IsStandbyExpected())
                            RedirectToStandby(Lockouts.Description);

                if (Global.IsWebApiRequest(context.Request.RawUrl))
                    if (Lockouts.IsUnavailableExpected())
                        ReturnServiceUnavailable(Lockouts.Description, context.Response);
            }
            catch
            {
                // Ignore exceptions that occur in this method.
            }
        }

        private void RedirectToStandby(string description)
            => HttpResponseHelper.Redirect("/standby?description=" + StringHelper.EncodeBase64Url(description));

        private void ReturnServiceUnavailable(string description, HttpResponse response)
        {
            response.Clear();
            response.StatusCode = 503;
            response.ContentType = "application/json";
            response.Write(JsonConvert.SerializeObject(description));
            response.Flush();
        }
    }
}