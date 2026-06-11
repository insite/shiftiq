using System;

namespace InSite.Application.Attempts.Read
{
    public class VPerformanceReport
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid FormIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }
        public Guid CompetencyAreaIdentifier { get; set; }
        public string CompetencyAreaTitle { get; set; }
        public string CompetencyAreaLabel { get; set; }
        public Guid? Alt_CompetencyAreaIdentifier { get; set; }
        public string Alt_CompetencyAreaTitle { get; set; }
        public string Alt_CompetencyAreaLabel { get; set; }
        public int QuestionSequence { get; set; }
        public string ParentQuestionText { get; set; }
        public string QuestionText { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionTags { get; set; }
        public string FormClassificationInstrument { get; set; }
        public decimal Points { get; set; }
        public decimal MaxPoints { get; set; }
        public DateTimeOffset? AttemptStarted { get; set; }
        public DateTimeOffset AttemptGraded { get; set; }
        public int? AnswerOptionKey { get; set; }
        public int? AnswerOptionSequence { get; set; }
        public string AnswerOptionText { get; set; }
        public string QuestionType { get; set; }
        public string FormName { get; set; }
    }
}
