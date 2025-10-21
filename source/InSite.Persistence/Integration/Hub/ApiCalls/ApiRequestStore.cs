using System.Data.Entity;
using System.Threading.Tasks;

using InSite.Domain.Integration;

namespace InSite.Persistence
{
    public static class ApiRequestStore
    {
        public static void Insert(ApiRequest request)
        {
            using (var db = new InternalDbContext())
            {
                db.ApiRequests.Add(request);
                db.SaveChanges();
            }
        }

        public static async Task InsertAsync(ApiRequest request)
        {
            using (var db = new InternalDbContext())
            {
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
