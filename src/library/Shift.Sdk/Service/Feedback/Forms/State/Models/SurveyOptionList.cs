using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Surveys.Forms
{
    [Serializable]
    public class SurveyOptionList
    {
        [JsonIgnore]
        public bool IsEmpty => Items.IsEmpty();

        /// <summary>
        /// The container for the option.
        /// </summary>
        [JsonIgnore]
        public SurveyOptionTable Table { get; set; }

        /// <summary>
        /// The container for the option.
        /// </summary>
        [JsonIgnore]
        public SurveyQuestion Question { get; set; }

        /// <summary>
        /// Uniquely identifies the list.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Lists can be grouped into logical categories.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The ordinal position of this option in the question that contains it.
        /// </summary>
        [JsonIgnore]
        public int Sequence => 1 + Table.IndexOf(this);

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        public List<SurveyOptionItem> Items { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ContentContainer Content { get; set; }

        [JsonIgnore]
        public static string[] ContentLabels => new[] { "Title" };

        public SurveyOptionList()
        {
            Items = new List<SurveyOptionItem>();
            Content = new ContentContainer();
        }

        public SurveyOptionList(Guid identifier) : this()
        {
            Identifier = identifier;
        }

        public bool ShouldSerializeContent() => Content != null && !Content.IsEmpty;
    }
}
