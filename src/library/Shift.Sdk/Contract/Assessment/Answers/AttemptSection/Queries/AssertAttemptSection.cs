using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertAttemptSection : Query<bool>
    {
        public Guid AttemptIdentifier { get; set; }

        public int SectionIndex { get; set; }
    }
}