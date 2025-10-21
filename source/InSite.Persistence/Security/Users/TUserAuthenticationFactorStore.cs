using System.Data.Entity;

namespace InSite.Persistence
{
    public static class TUserAuthenticationFactorStore
    {
        public static void Insert(TUserAuthenticationFactor entity)
        {
            using (var context = new InternalDbContext())
            {
                context.TUserAuthenticationFactors.Add(entity);
                context.SaveChanges();
            }
        }

        public static void Update(TUserAuthenticationFactor entity)
        {
            using (var context = new InternalDbContext())
            {
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
