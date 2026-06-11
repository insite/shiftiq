using System;

namespace InSite.Application.Banks.Read
{
    public class QBankQuestionDetail
    {
        public Guid BankIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid? QuestionCompetencyIdentifier { get; set; }
        public int BankIndex { get; set; }
        public string BankName { get; set; }
        public string QuestionCode { get; set; }
        public int? QuestionDifficulty { get; set; }
        public int? QuestionTaxonomy { get; set; }
        public string QuestionText { get; set; }
        public string QuestionReference { get; set; }
        public string QuestionTag { get; set; }
        public string QuestionTags { get; set; }
        public string QuestionType { get; set; }
        public string QuestionCompetencyTitle { get; set; }
        public string Rubric { get; set; }
        public string QuestionFlag { get; set; }
        public string QuestionPublicationStatus { get; set; }
    }
}
