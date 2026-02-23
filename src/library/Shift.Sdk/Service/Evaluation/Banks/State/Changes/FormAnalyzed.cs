using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormAnalyzed : Change
    {
        public Guid Identifier { get; set; }

        public FormAnalyzed(Guid identifier)
        {
            Identifier = identifier;
        }
    }
}
