using System;

namespace Shift.Contract
{
    public class DeleteAttemptSection
    {
        public Guid AttemptIdentifier { get; set; }

        public int SectionIndex { get; set; }
    }
}