using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Glossaries
{
    public class GlossaryTermApproved : Change
    {
        public Guid Term { get; set; }

        public GlossaryTermApproved(Guid term)
        {
            Term = term;
        }
    }
}