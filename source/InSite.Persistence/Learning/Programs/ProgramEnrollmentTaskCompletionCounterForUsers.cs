using System;

namespace InSite.Persistence
{
    public class ProgramEnrollmentTaskCompletionCounterForUsers
    {
        public Guid ProgramIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public int CompletionCounter { get; set; }
        public int TaskCount { get; set; }

        public int CompletionPercent 
            => TaskCount == 0 ? 0 : (int)Math.Round((double)CompletionCounter / TaskCount * 100);
    }
}