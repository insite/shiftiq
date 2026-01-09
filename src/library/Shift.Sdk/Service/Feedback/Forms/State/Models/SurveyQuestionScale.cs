using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Surveys.Forms
{
    [Serializable]
    public class SurveyScale
    {
        /// <summary>
        /// The container for the scale.
        /// </summary>
        [JsonIgnore]
        public SurveyQuestion Question { get; set; }

        /// <summary>
        /// The category to which this scale applies.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The items in the scale.
        /// </summary>
        public List<SurveyScaleItem> Items { get; set; }

        public SurveyScale Clone()
        {
            var json = JsonConvert.SerializeObject(this);
            var scale = JsonConvert.DeserializeObject<SurveyScale>(json);
            return scale;
        }

        public SurveyScale()
        {
            Items = new List<SurveyScaleItem>();
        }

        public void Sort()
        {
            Items.Sort(new SurveyScaleItemComparer());
        }
    }

    public class SurveyScaleItemComparer : IComparer<SurveyScaleItem>
    {
        public int Compare(SurveyScaleItem x, SurveyScaleItem y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x.Minimum < y.Minimum)
                return -1;

            if (x.Minimum > y.Minimum)
                return 1;

            if (x.Maximum < y.Maximum)
                return -1;

            if (x.Maximum > y.Maximum)
                return 1;

            return (x.Grade ?? string.Empty).CompareTo(y.Grade ?? string.Empty);
        }
    }

    [Serializable]
    public class SurveyScaleItem
    {
        /// <summary>
        /// The minimum for the range of points defining this item in a scale.
        /// </summary>
        public decimal Minimum { get; set; }

        /// <summary>
        /// The maximum for the range of points defining this item in a scale.
        /// </summary>
        public decimal Maximum { get; set; }

        /// <summary>
        /// The grade that corresponds to the range of points defining this item in a scale.
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// The calculation (Sum|Average) to be performed on points that fall within this scale.
        /// </summary>
        public string Calculation { get; set; }

        /// <summary>
        /// The multilingual Text and/or HTML content for this item in a scale.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ContentContainer Content { get; set; }

        [JsonIgnore]
        public static string[] ContentLabels => new[] { "Description" };

        public SurveyScaleItem()
        {
            Content = new ContentContainer();
        }
    }
}
