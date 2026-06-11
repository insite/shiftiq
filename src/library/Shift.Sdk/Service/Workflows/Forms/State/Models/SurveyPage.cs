using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Surveys.Forms
{
    [Serializable]
    public class SurveyPage
    {
        public int Sequence { get; set; }

        /// <summary>
        /// The multilingual Text and/or HTML content for this option.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ContentContainer Content { get; set; }

        [JsonIgnore]
        public static string[] ContentLabels => new[] { "Title", "Description" };

        /// <summary>
        /// The questions contained by the page.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SurveyQuestion> Questions { get; set; }

        public SurveyPage(int n)
        {
            Sequence = n;
            Questions = new List<SurveyQuestion>();
            Content = new ContentContainer();
        }

        public SurveyPage(int n, Guid identifier) : this(n)
        {
        }

        public bool ShouldSerializeQuestions() => (Questions?.Count ?? 0) > 0;

        public SurveyOptionItem[] FlattenOptionItems()
            => Questions.SelectMany(x => x.Options.Lists.SelectMany(y => y.Items)).ToArray();
    }
}
