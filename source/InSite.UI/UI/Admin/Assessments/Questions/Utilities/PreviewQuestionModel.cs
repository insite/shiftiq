using InSite.Domain.Attempts;
using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Questions.Utilities
{
    public class PreviewQuestionModel
    {
        public int Sequence { get; }

        public Question BankQuestion { get; }
        public AttemptQuestion AttemptQuestion { get; }

        public PreviewQuestionModel(int sequence, Question bankQuestion, AttemptQuestion attemptQuestion)
        {
            Sequence = sequence;
            BankQuestion = bankQuestion;
            AttemptQuestion = attemptQuestion;
        }
    }
}