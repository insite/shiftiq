using System.Collections.Generic;
using System.Data.Entity;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TAchievementDepartmentStore
    {
        public static void Insert(IEnumerable<TAchievementDepartment> list)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var item in list)
                    item.JoinIdentifier = UniqueIdentifier.Create();
                db.TAchievementDepartments.AddRange(list);
                db.SaveChanges();
            }
        }

        public static void Delete(IEnumerable<TAchievementDepartment> list)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var entity in list)
                    db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }
    }
}
