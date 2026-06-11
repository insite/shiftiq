using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class DepartmentProfileUserStore
    {
        #region INSERT

        public static void Insert(DepartmentProfileUser entity)
        {
            using (var db = new InternalDbContext())
            {
                db.DepartmentProfileUsers.Add(entity);
                db.SaveChanges();
            }
        }

        #endregion

        #region DELETE

        public static void DeleteByDepartmentIdentifier(Guid department, Guid actionUser, Guid actionOrganization)
        {
            using (var db = new InternalDbContext())
            {
                var entities = db.DepartmentProfileUsers.Where(x => x.DepartmentIdentifier == department).ToList();
                foreach (var entity in entities)
                    db.DepartmentProfileUsers.Remove(entity);

                db.SaveChanges();
            }
        }

        public static void Delete(IEnumerable<DepartmentProfileUser> entities, Guid actionUser, Guid actionOrganization)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var entity in entities)
                    db.Entry(entity).State = EntityState.Deleted;

                db.SaveChanges();
            }
        }

        #endregion

        #region UPDATE

        public static void Update(DepartmentProfileUser entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void InsertUpdateDelete(IEnumerable<DepartmentProfileUser> insertList, IEnumerable<DepartmentProfileUser> updateList, IEnumerable<DepartmentProfileUser> deleteList)
        {
            using (var db = new InternalDbContext())
            {
                if (insertList != null)
                    db.DepartmentProfileUsers.AddRange(insertList);

                if (updateList != null)
                {
                    foreach (var entity in updateList)
                        db.Entry(entity).State = EntityState.Modified;
                }

                if (deleteList != null)
                {
                    foreach (var entity in deleteList)
                        db.Entry(entity).State = EntityState.Deleted;
                }

                db.SaveChanges();
            }
        }

        #endregion
    }
}
