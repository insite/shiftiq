using System;

namespace Shift.Contract
{
    public partial class FormWorkshop
    {
        public Guid BankId { get; set; }
        public FormDetails Details { get; set; }
        public QuestionStatistics Statistics { get; set; }
        public WorkshopQuestionData QuestionData { get; set; }
        public WorkshopComment[] Comments { get; set; }
        public WorkshopAttachment[] Attachments { get; set; }
        public WorkshopProblemQuestion[] ProblemQuestions { get; set; }
    }
}
