using System;

namespace InSite.Application.Attempts.Read
{
    public interface ILearnerAttemptSummarySearch
    {
        TLearnerAttemptSummary GetSummary(Guid form, Guid candidate);
        TLearnerAttemptSummary GetFormSummary(Guid form);
    }
}
