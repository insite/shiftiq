using System.Data.Entity;
using System.Threading.Tasks;

using InSite.Domain.Integration;

namespace InSite.Persistence
{
    public static class ApiRequestStore
    {
        // The request row is an audit record written on the request path, before the call it
        // describes runs, and every caller catches a failed write and continues. With the default
        // 30-second timeout a transient database stall makes the customer's call absorb the whole
        // wait for a row nobody is waiting on (Sentry error SHIFT-3HN: one insert blocked for
        // 30 seconds on a quiet night). Five seconds is generous for a single-row append and bounds
        // the damage when the database pauses.
        private const int InsertTimeoutSeconds = 5;

        public static void Insert(ApiRequest request)
        {
            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = InsertTimeoutSeconds;

                db.ApiRequests.Add(request);
                db.SaveChanges();
            }
        }

        public static async Task InsertAsync(ApiRequest request)
        {
            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = InsertTimeoutSeconds;

                db.ApiRequests.Add(request);
                await db.SaveChangesAsync();
            }
        }

        public static void Update(ApiRequest request)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
