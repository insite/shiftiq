using System;

namespace InSite.Persistence.Integration.Moodle
{
    public class TScormEvent
    {
        public Guid EventIdentifier { get; set; }
        public string EventData { get; set; }
        public DateTimeOffset EventWhen { get; set; }
    }
}
