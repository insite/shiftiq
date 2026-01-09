using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ImportExamAttempts : Command
    {
        public bool AllowDuplicates { get; set; }

        public ImportExamAttempts(Guid aggregate, bool allowDuplicates)
        {
            AggregateIdentifier = aggregate;
            AllowDuplicates = allowDuplicates;
        }
    }
}
