using System.Collections.Generic;
using System.Data.Entity;

namespace InSite.Persistence
{
    public static class DepartmentProfileCompetencyStore
    {
        #region INSERT

        public static void Insert(DepartmentProfileCompetency entity)
        {
            using (var db = new InternalDbContext())
            {
                db.DepartmentProfileCompetencies.Add(entity);
                db.SaveChanges();
            }
        }

        #endregion

        #region DELETE

        public static void Delete(IEnumerable<DepartmentProfileCompetency> entities)
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

        public static void Update(DepartmentProfileCompetency entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void InsertUpdateDelete(IEnumerable<DepartmentProfileCompetency> insertList, IEnumerable<DepartmentProfileCompetency> updateList, IEnumerable<DepartmentProfileCompetency> deleteList)
        {
            using (var db = new InternalDbContext())
            {
                if (insertList != null)
                    db.DepartmentProfileCompetencies.AddRange(insertList);

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
