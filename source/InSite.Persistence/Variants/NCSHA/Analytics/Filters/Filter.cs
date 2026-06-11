using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class Filter
    {
        public Guid FilterId { get; set; }
        public Guid AuthorUserIdentifier { get; set; }
        public string FilterName { get; set; }
        public string FilterData { get; set; }
        public DateTimeOffset DateSaved { get; set; }
    }
}
