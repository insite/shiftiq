using System;

using Shift.Common;
using Shift.Constant;

namespace Shift.Contract
{
    public class WorkshopQuestion
    {
        public class LayoutColumn
        {
            public HorizontalAlignment Alignment { get; set; }
            public string CssClass { get; set; }
            public string TextMarkdown { get; set; }
            public string TextHtml { get; set; }
        }

        public class QuestionForm
        {
            public Guid FormId { get; set; }
            public string FormName { get; set; }
            public int FormSequence { get; set; }
            public int FormAssetNumber { get; set; }
            public int FormAssetversion { get; set; }
        }

        public class QuestionSource
        {
            public Guid QuestionId { get; set; }
            public int QuestionAssetNumber { get; set; }
        }

        public class QuestionReplaceButtons
        {
            public bool NewVersion { get; set; }
            public bool NewQuestionAndSurplus { get; set; }
            public bool NewQuestionAndPurge { get; set; }
            public bool RollbackQuestion { get; set; }
        }

        public Guid QuestionId { get; set; }
        public Guid? FieldId { get; set; }
        public Guid? ParentStandardId { get; set; }
        public Guid? StandardId { get; set; }
        public int QuestionBankIndex { get; set; }
        public int? QuestionFormSequence { get; set; }
        public FlagType QuestionFlag { get; set; }
        public QuestionItemType QuestionType { get; set; }
        public MultilingualString QuestionTitle { get; set; }
        public string QuestionTitleHtml { get; set; }
        public string Rationale { get; set; }
        public string RationaleOnCorrectAnswer { get; set; }
        public string RationaleOnIncorrectAnswer { get; set; }
        public int QuestionAssetNumber { get; set; }
        public int QuestionAssetVersion { get; set; }
        public string QuestionPublicationStatusDescription { get; set; }
        public string QuestionCondition { get; set; }
        public int? QuestionTaxonomy { get; set; }
        public string QuestionLikeItemGroup { get; set; }
        public string QuestionReference { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionTag { get; set; }
        public OptionLayoutType QuestionLayoutType { get; set; }
        public int? QuestionPoints { get; set; }
        public int? QuestionCutScore { get; set; }
        public string QuestionCalculationMethodDescription { get; set; }
        public int? QuestionClassificationDifficulty { get; set; }
        public bool QuestionRandomizationEnabled { get; set; }

        public LayoutColumn[] LayoutColumns { get; set; }

        public int CandidateCommentCount { get; set; }

        public bool CanEdit { get; set; }
        public bool CanNavigateToChangePage { get; set; }
        public bool CanCopyField { get; set; }
        public QuestionReplaceButtons ReplaceButtons { get; set; }

        public QuestionSource Source { get; set; }

        public QuestionForm[] Forms { get; set; }
        public WorkshopComment[] Comments { get; set; }
        public WorkshopQuestionOption[] Options { get; set; }
        public WorkshopQuestionMatch[] Matches { get; set; }
        public string[] Distractors { get; set; }
    }
}
