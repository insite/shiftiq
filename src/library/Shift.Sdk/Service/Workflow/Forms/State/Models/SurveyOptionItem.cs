using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Surveys.Forms
{
    [Serializable]
    public class SurveyOptionItem
    {
        /// <summary>
        /// The container for the option.
        /// </summary>
        [JsonIgnore]
        public SurveyOptionList List { get; set; }

        /// <summary>
        /// The container for the option.
        /// </summary>
        [JsonIgnore]
        public SurveyQuestion Question { get; set; }

        /// <summary>
        /// Uniquely identifies the question.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// The question to which the respondent must be skipped after selecting this option.
        /// </summary>
        public Guid? BranchToQuestionIdentifier { get; set; }

        /// <summary>
        /// The questions that must be masked for the respondent after selecting this option.
        /// </summary>
        public List<Guid> MaskedQuestionIdentifiers { get; set; }

        /// <summary>
        /// Options can be grouped into logical categories.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The ordinal position of this option in the question that contains it.
        /// </summary>
        [JsonIgnore]
        public int Sequence => 1 + List.Items.IndexOf(this);

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        /// <summary>
        /// The number of points awarded for selecting this option on a survey question.
        /// </summary>
        public decimal Points { get; set; }

        /// <summary>
        /// The multilingual Text and/or HTML content for this option.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ContentContainer Content { get; set; }

        [JsonIgnore]
        public static string[] ContentLabels => new[] { "Title", "Description", "Feedback" };

        public SurveyOptionItem()
        {
            MaskedQuestionIdentifiers = new List<Guid>();
            Content = new ContentContainer();
        }

        public SurveyOptionItem(Guid identifier) : this()
        {
            Identifier = identifier;
        }

        public bool ShouldSerializeContent() => Content != null && !Content.IsEmpty;
        public bool ShouldSerializeMaskedQuestionIdentifiers() => (MaskedQuestionIdentifiers?.Count ?? 0) > 0;
    }
}
