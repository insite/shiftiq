using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Glossaries
{
    public class GlossaryTermRejected : Change
    {
        public Guid Term { get; set; }

        public GlossaryTermRejected(Guid term)
        {
            Term = term;
        }
    }
}