using System;

namespace Shift.Contract
{
    public class WorkshopProblemQuestion
    {
        public class ProblemOption
        {
            public int Number { get; set; }
            public string Title { get; set; }
            public string Letter { get; set; }
            public int Points { get; set; }
        }

        public Guid QuestionId { get; set; }
        public int QuestionBankIndex { get; set; }
        public int QuestionAssetNumber { get; set; }
        public int QuestionAssetVersion { get; set; }
        public string QuestionSetName { get; set; }
        public string QuestionTitle { get; set; }
        public bool CanDelete { get; set; }
        public string ProblemDescription { get; set; }
        public ProblemOption[] Options { get; set; }
    }
}
