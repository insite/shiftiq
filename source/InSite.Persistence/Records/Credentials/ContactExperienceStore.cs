using System.Data.Entity;

namespace InSite.Persistence
{
    public static class ContactExperienceStore
    {
        public static void Insert(ContactExperience entity)
        {
            using (var db = new InternalDbContext())
            {
                db.ContactExperiences.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Update(ContactExperience entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(ContactExperience entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }
    }
}