using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Publication properties determine the rules for availability and/or display of an online assessment form.
    /// </summary>
    [Serializable]
    public class FormPublication
    {
        [DefaultValue(PublicationStatus.Drafted)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public PublicationStatus Status { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? FirstPublished { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsProgram { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsPublished { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool AllowFeedback { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool AllowRationaleForCorrectAnswers { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool AllowDownloadAssessmentsQA { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool AllowRationaleForIncorrectAnswers { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string NavigateUrl { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Icon { get; set; }

        public FormPublication Clone()
        {
            var clone = new FormPublication();

            this.ShallowCopyTo(clone);

            return clone;
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (IsPublished && Status == PublicationStatus.Drafted)
                Status = PublicationStatus.Published;

            if (Status == PublicationStatus.Published && !IsPublished)
                IsPublished = true;
        }
    }
}
