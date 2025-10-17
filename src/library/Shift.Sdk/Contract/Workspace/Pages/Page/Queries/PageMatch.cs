using System;

namespace Shift.Contract
{
    public partial class PageMatch
    {
        public Guid PageIdentifier { get; set; }

        public string PageIcon { get; set; }
        public string PageTitle { get; set; }
        public string PageUrl { get; set; }
    }
}