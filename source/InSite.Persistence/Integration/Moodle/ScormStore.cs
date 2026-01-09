using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence.Integration.Moodle
{
    public class ScormStore
    {
        public void InsertScormEvent(Guid activityId, TScormEvent e, DateTimeOffset when)
        {
            using (var db = CreateContext())
            {
                var entity = new TScormEvent();

                entity.EventIdentifier = UniqueIdentifier.Create();
                entity.EventData = JsonConvert.SerializeObject(e);
                entity.EventWhen = when;

                db.TScormEvents.Add(entity);
                db.SaveChanges();
            }
        }

        private InternalDbContext CreateContext()
            => new InternalDbContext(true);
    }
}
