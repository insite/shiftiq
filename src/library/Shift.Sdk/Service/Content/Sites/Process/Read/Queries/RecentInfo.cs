using System;

namespace InSite.Application.Sites.Read
{
    public class RecentInfo
    {
        public Guid Identifier { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
    }

}
