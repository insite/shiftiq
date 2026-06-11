using System;
using System.Data.Entity;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TActionStore
    {
        public static void Delete(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var action = db.TActions.SingleOrDefault(x => x.ActionIdentifier == id);
                if (action == null)
                    return;

                db.TActions.Remove(action);
                db.SaveChanges();
            }

            TActionSearch.Refresh();
        }

        public static void Insert(TAction action)
        {
            using (var db = new InternalDbContext())
            {
                action.ActionIdentifier = UniqueIdentifier.Create();

                db.TActions.Add(action);
                db.SaveChanges();
            }

            TActionSearch.Refresh();
        }

        public static void Update(TAction action)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(action).State = EntityState.Modified;
                db.SaveChanges();
            }

            TActionSearch.Refresh();
        }

        public static TAction Update(Guid actionId, Action<TAction> change)
        {
            TAction action;

            using (var db = new InternalDbContext())
            {
                action = db.TActions.Single(x => x.ActionIdentifier == actionId);
                change(action);
                db.SaveChanges();
            }

            TActionSearch.Refresh();

            return action;
        }
    }
}
