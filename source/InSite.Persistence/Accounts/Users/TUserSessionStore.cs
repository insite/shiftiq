using System.Data.Entity;

namespace InSite.Persistence
{
    public static class TUserSessionStore
    {
        public static void Insert(TUserSession entity)
        {
            using (var context = new InternalDbContext())
            {
                context.TUserSessions.Add(entity);
                context.SaveChanges();
            }
        }

        public static void Update(TUserSession entity)
        {
            using (var context = new InternalDbContext())
            {
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
