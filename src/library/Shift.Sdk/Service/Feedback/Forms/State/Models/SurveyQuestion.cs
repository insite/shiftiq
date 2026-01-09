using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Surveys.Forms
{
    [Serializable]
    public class SurveyQuestion
    {
        /// <summary>
        /// The container for the question.
        /// </summary>
        [JsonIgnore]
        public SurveyForm Form { get; set; }

        /// <summary>
        /// The page of the question
        /// </summary>
        [JsonIgnore]
        public SurveyPage Page => Form.GetPage(Identifier);

        /// <summary>
        /// Uniquely identifies the question.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// What type of question is this?
        /// </summary>
        public SurveyQuestionType Type { get; set; }

        /// <summary>
        /// Reference code assigned to the question.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Name of the field in which to store answers to this question in a respondent's user account.
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Bootstrap CSS class indicator used to style the Code.
        /// </summary>
        public string Indicator { get; set; }

        /// <summary>
        /// Where did this question come from?
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The ordinal position of this question in the form that contains it.
        /// </summary>
        [JsonIgnore]
        public int Sequence => 1 + Form.Questions.IndexOf(this);

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        public bool IsHidden { get; set; }
        public string LikertAnalysis { get; set; }

        public bool IsNested { get; set; }
        public bool IsRequired { get; set; }
        public bool ListDisableColumnHeadingWrap { get; set; }
        public bool ListEnableBranch { get; set; }
        public bool ListEnableGroupMembership { get; set; }
        public bool ListEnableOtherText { get; set; }
        public bool ListEnableRandomization { get; set; }
        public bool NumberEnableStatistics { get; set; }
        public bool NumberEnableAutoCalc { get; set; }
        public Guid[] NumberAutoCalcQuestions { get; set; }
        public bool NumberEnableNotApplicable { get; set; }
        public bool EnableCreateCase { get; set; }

        public int? TextCharacterLimit { get; set; }
        public int? TextLineCount { get; set; }

        /// <summary>
        /// The multilingual Text and/or HTML content for this option.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Shift.Common.ContentContainer Content { get; set; }

        [JsonIgnore]
        public static string[] ContentLabels => new[] { "Title", "Hint", "Likert Low", "Likert High" };

        /// <summary>
        /// The options contained by the question.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SurveyOptionTable Options { get; set; }

        /// <summary>
        /// The scales defined for the question.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SurveyScale> Scales { get; set; }

        [JsonIgnore]
        public bool HasInput => Type != SurveyQuestionType.BreakPage
            && Type != SurveyQuestionType.BreakQuestion
            && Type != SurveyQuestionType.Terminate;

        [JsonIgnore]
        public bool IsList => Type == SurveyQuestionType.CheckList
            || Type == SurveyQuestionType.RadioList
            || Type == SurveyQuestionType.Selection;

        [JsonProperty]
        public SurveyQuestionListSelectionRange ListSelectionRange { get; private set; }

        public SurveyQuestion()
        {
            Content = new Shift.Common.ContentContainer();
            Options = new SurveyOptionTable();
            Scales = new List<SurveyScale>();
            ListSelectionRange = new SurveyQuestionListSelectionRange();
        }

        public SurveyQuestion(Guid identifier) : this()
        {
            Identifier = identifier;
        }

        public SurveyQuestion Clone()
        {
            var json = JsonConvert.SerializeObject(this);
            var question = JsonConvert.DeserializeObject<SurveyQuestion>(json);

            foreach (var list in question.Options.Lists)
            {
                list.Table = question.Options;

                foreach (var option in list.Items)
                    option.List = list;
            }

            return question;
        }

        public bool DisplayAnswerInput()
        {
            return HasInput
                && (Type != SurveyQuestionType.Likert
                 || string.IsNullOrEmpty(LikertAnalysis)
                 || StringHelper.Equals(LikertAnalysis, "Current Question Only")
                 );
        }

        public bool ShouldSerializeIsHidden() => IsHidden;
        public bool ShouldSerializeContent() => Content != null && !Content.IsEmpty;
        public bool ShouldSerializeOptions() => Options != null && !Options.IsEmpty;
        public bool ShouldSerializeScales() => Scales.IsNotEmpty();

        public SurveyOptionItem[] FlattenOptionItems()
            => Options.Lists.SelectMany(x => x.Items).ToArray();

        public void RemoveOrphanScales()
        {
            var categories = Options.Lists.Select(x => x.Category).Distinct().ToList();
            var orphans = Scales.Where(scale => !categories.Any(category => string.Compare(category, scale.Category, true) == 0)).ToList();
            foreach (var orphan in orphans)
                Scales.Remove(orphan);
        }

        public string GetIndicatorStyleName()
        {
            return Indicator.ToEnum(Shift.Constant.Indicator.None).GetContextualClass();
        }
    }
}
