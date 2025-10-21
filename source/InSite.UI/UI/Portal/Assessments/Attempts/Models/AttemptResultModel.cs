using InSite.Application.Attempts.Read;
using InSite.Domain.Banks;

namespace InSite.Portal.Assessments.Attempts.Models
{
    public class AttemptResultModel
    {
        public bool IsSuccess { get; internal set; }
        public decimal ScoreScaled { get; internal set; }

        public string AttemptOrdinal { get; internal set; }
        public string RetryInstruction { get; internal set; }

        public Form BankForm { get; internal set; }
        public QAttempt Attempt { get; internal set; }
    }
}