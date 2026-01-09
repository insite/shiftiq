using System;
using System.Collections.Generic;
using System.Linq;

namespace InSite.Persistence.Integration.Moodle
{
    public class MoodleSearch
    {
        private InternalDbContext CreateContext()
            => new InternalDbContext(true);

        public int CountMoodleEvents(IEnumerable<Guid> activities)
        {
            using (var db = CreateContext())
            {
                return db.TMoodleEvents
                    .Count(x => activities.Contains(x.ActivityIdentifier));
            }
        }

        public IEnumerable<TMoodleEvent> GetMoodleEvents(IEnumerable<Guid> activities)
        {
            using (var db = CreateContext())
            {
                return db.TMoodleEvents
                    .Where(x => activities.Contains(x.ActivityIdentifier))
                    .ToArray();
            }
        }
    }
}