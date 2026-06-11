using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Banks.Read
{
    [Serializable]
    public class QBankQuestionFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }

        public Guid? BankIdentifier { get; set; }
        public Guid? ParentQuestionIdentifier { get; set; }
        public Guid? RubricIdentifier { get; set; }

        public bool? HasParentQuestion { get; set; }

        public string QuestionAsset { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionReference { get; set; }
        public string QuestionText { get; set; }
        public string QuestionTextWithAssetNumber { get; set; }

        public string QuestionLikeItemGroup { get; set; }
        public string QuestionTag { get; set; }
        public int? QuestionTaxonomy { get; set; }
        public int? QuestionDifficulty { get; set; }
        public string QuestionFlag { get; set; }
        public string QuestionBank { get; set; }
        public string QuestionType { get; set; }
        public string QuestionCompetencyTitle { get; set; }
        public string QuestionClassificationTag { get; set; }

        public PublicationStatus? QuestionPublicationStatus { get; set; }

        public DateTimeOffsetRange QuestionChangedRange { get; set; }

        public string StandardCode { get; set; }

        public QBankQuestionFilter Clone()
        {
            return (QBankQuestionFilter)MemberwiseClone();
        }
    }
}
