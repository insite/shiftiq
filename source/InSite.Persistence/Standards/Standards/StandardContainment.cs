using System;

namespace InSite.Persistence
{
    public class StandardContainment
    {
        public StandardContainment() { }
        public StandardContainment(Guid parent, Guid child) { ParentStandardIdentifier = parent; ChildStandardIdentifier = child; }

        public Int32 ChildSequence { get; set; }
        public Guid ChildStandardIdentifier { get; set; }
        public Guid ParentStandardIdentifier { get; set; }
        public String CreditType { get; set; }
        public Decimal? CreditHours { get; set; }

        public virtual Standard Parent { get; set; }
        public virtual Standard Child { get; set; }
    }
}
