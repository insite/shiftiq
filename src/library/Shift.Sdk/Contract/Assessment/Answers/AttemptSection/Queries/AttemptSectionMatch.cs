using System;

namespace Shift.Contract
{
    public partial class AttemptSectionMatch
    {
        public Guid AttemptIdentifier { get; set; }

        public int SectionIndex { get; set; }
    }
}