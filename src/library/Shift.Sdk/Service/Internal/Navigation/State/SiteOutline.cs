using System;

namespace InSite.Domain.Foundations
{
    public class SiteOutline : ISiteOutline
    {
        public Guid Identifier { get; set; }
        public Guid Organization { get; set; }
        public string Domain { get; set; }
        public PageTree Pages { get; set; }
    }
}
