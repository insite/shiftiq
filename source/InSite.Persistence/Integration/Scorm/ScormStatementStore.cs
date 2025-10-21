using System;
using System.Data.Entity;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class ScormStatementStore
    {
        public static void Save(TScormStatement statement)
        {
            var x = ScormStatementSearch.Select(statement.StatementIdentifier);
            if (x == null)
                Insert(statement);
            else
                Update(statement);
        }

        public static void Insert(TScormStatement statement)
        {
            using (var db = new InternalDbContext())
            {
                SnipStrings(statement);
                db.TScormStatements.Add(statement);
                db.SaveChanges();
            }
        }

        public static void Update(TScormStatement statement)
        {
            using (var db = new InternalDbContext())
            {
                SnipStrings(statement);
                db.Entry(statement).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var statement = db.TScormStatements.FirstOrDefault(x => x.StatementIdentifier == id);
                if (statement == null)
                    return;
                db.TScormStatements.Remove(statement);
                db.SaveChanges();
            }
        }

        private static void SnipStrings(TScormStatement entity)
        {
            var max = InternalDbContext.GetMaxLength<TScormStatement>(x => x.ActorName);
            if (max.HasValue)
                entity.ActorName = entity.ActorName.MaxLength(max.Value, true);

            max = InternalDbContext.GetMaxLength<TScormStatement>(x => x.ObjectDefinitionName);
            if (max.HasValue)
                entity.ObjectDefinitionName = entity.ObjectDefinitionName.MaxLength(max.Value, true);

            max = InternalDbContext.GetMaxLength<TScormStatement>(x => x.VerbDisplay);
            if (max.HasValue)
                entity.VerbDisplay = entity.VerbDisplay.MaxLength(max.Value, true);
        }
    }
}