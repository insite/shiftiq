using System;

namespace Shift.Contract
{
    public partial class SpecWorkshop
    {
        public Guid BankId { get; set; }
        public WorkshopStandard[] Standards { get; set; }
        public SpecDetails Details { get; set; }
        public WorkshopQuestionData QuestionData { get; set; }
        public WorkshopComment[] Comments { get; set; }
        public WorkshopAttachment[] Attachments { get; set; }
        public WorkshopProblemQuestion[] ProblemQuestions { get; set; }
    }
}
