using System;

namespace InSite.Domain.Events
{
    [Serializable]
    public class ExamCandidateNotAllowedException : Exception
    {
        public DateTimeOffset Cutoff { get; set; }

        public ExamCandidateNotAllowedException(DateTimeOffset cutoff)
        {
            Cutoff = cutoff;
        }
    }
}