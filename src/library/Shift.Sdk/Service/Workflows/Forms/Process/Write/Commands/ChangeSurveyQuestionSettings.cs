using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Surveys.Forms;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyQuestionSettings : Command
    {
        public ChangeSurveyQuestionSettings(Guid form, Guid question, bool isHidden, bool isRequired, bool isNested, string likertAnalysis, bool listEnableRandomization, bool listEnableOtherText, bool listEnableBranch, bool listEnableGroupMembership, bool listDisableColumnHeadingWrap, int? textLineCount, int? textCharacterLimit, bool numberEnableStatistics, bool numberEnableAutoCalc, Guid[] numberAutoCalcQuestions, bool numberEnableNotApplicable, SurveyQuestionListSelectionRange listSelectionRange, bool enableCreateCase)
        {
            AggregateIdentifier = form;
            Question = question;
            IsHidden = isHidden;
            IsRequired = isRequired;
            IsNested = isNested;
            LikertAnalysis = likertAnalysis;
            ListEnableRandomization = listEnableRandomization;
            ListEnableOtherText = listEnableOtherText;
            ListEnableBranch = listEnableBranch;
            ListEnableGroupMembership = listEnableGroupMembership;
            ListDisableColumnHeadingWrap = listDisableColumnHeadingWrap;
            TextLineCount = textLineCount;
            TextCharacterLimit = textCharacterLimit;
            NumberEnableStatistics = numberEnableStatistics;
            NumberEnableAutoCalc = numberEnableAutoCalc;
            NumberAutoCalcQuestions = numberAutoCalcQuestions;
            NumberEnableNotApplicable = numberEnableNotApplicable;
            ListSelectionRange = listSelectionRange;
            EnableCreateCase = enableCreateCase;
        }

        public Guid Question { get; }
        public bool IsHidden { get; }
        public bool IsNested { get; }
        public bool IsRequired { get; }
        public string LikertAnalysis { get; }
        public bool ListEnableRandomization { get; }
        public bool ListEnableOtherText { get; }
        public bool ListEnableBranch { get; }
        public bool ListEnableGroupMembership { get; }
        public bool ListDisableColumnHeadingWrap { get; }
        public int? TextLineCount { get; }
        public int? TextCharacterLimit { get; }
        public bool NumberEnableStatistics { get; }
        public bool NumberEnableAutoCalc { get; }
        public Guid[] NumberAutoCalcQuestions { get; }
        public bool NumberEnableNotApplicable { get; }
        public SurveyQuestionListSelectionRange ListSelectionRange { get; }
        public bool EnableCreateCase { get; }
    }
}