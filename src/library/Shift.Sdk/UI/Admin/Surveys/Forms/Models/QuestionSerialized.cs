using System;
using System.Collections.Generic;

using Shift.Constant;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class QuestionSerialized
    {
        public SurveyQuestionType Type { get; set; }

        public int? Index { get; set; }

        public string Code { get; set; }
        public string Attribute { get; set; }
        public string Indicator { get; set; }
        public string Source { get; set; }

        public bool IsHidden { get; set; }
        public string LikertAnalysis { get; set; }
        public bool IsNested { get; set; }
        public bool IsRequired { get; set; }
        public bool ListDisableColumnHeadingWrap { get; set; }
        public bool ListEnableBranch { get; set; }
        public bool ListEnableOtherText { get; set; }
        public bool ListEnableRandomization { get; set; }
        public bool ListEnableGroupMembership { get; set; }
        public bool NumberEnableStatistics { get; set; }
        public bool NumberEnableAutoCalc { get; set; }
        public bool NumberEnableNotApplicable { get; set; }
        public bool EnableCreateCase { get; set; }
        public int? TextCharacterLimit { get; set; }
        public int? TextLineCount { get; set; }

        public List<SurveyContentSerialized> Content { get; set; }
        public List<OptionSerialized> Options { get; set; }
        public List<ScaleSerialized> Scales { get; set; }
    }
}