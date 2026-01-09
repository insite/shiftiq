using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAttemptSection : Query<AttemptSectionModel>
    {
        public Guid AttemptIdentifier { get; set; }

        public int SectionIndex { get; set; }
    }
}