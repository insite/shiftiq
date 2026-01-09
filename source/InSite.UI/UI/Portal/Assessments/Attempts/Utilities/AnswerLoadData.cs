using InSite.Application.Attempts.Read;
using InSite.Domain.Banks;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    public class AnswerLoadData
    {
        public AttemptUrlBase Url { get; set; }
        public Form BankForm { get; set; }
        public QAttempt Attempt { get; set; }
        public string ContentStyle { get; internal set; }

        public string GlossaryTermsScript { get; set; }
        public int LastQuestionIndex { get; set; }
    }
}