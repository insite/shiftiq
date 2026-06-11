using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// A section is a reference to a specific question set from within a specific form.
    /// </summary>
    [Serializable]
    public class Section
    {
        /// <summary>
        /// Uniquely identifies the section.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Uniquely identifies the criterion being referenced.
        /// </summary>
        [JsonProperty("Criterion")]
        public Guid CriterionIdentifier { get; set; }

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        /// <summary>
        /// The ordinal position of this section in the form that contains it.
        /// </summary>
        [JsonIgnore]
        public int Sequence => 1 + Form.Sections.IndexOf(this);

        /// <summary>
        /// Sets need support for multilingual titles, summaries, etc.
        /// </summary>
        public ContentExamSection Content { get; set; }

        /// <summary>
        /// The fields contained by the section.
        /// </summary>
        public List<Field> Fields { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool WarningOnNextTabEnabled { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool BreakTimerEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TimeLimit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public FormSectionTimeType TimerType { get; set; } = FormSectionTimeType.Optional;

        /// <summary>
        /// The form that contains the section.
        /// </summary>
        [JsonIgnore]
        public Form Form { get; set; }

        /// <summary>
        /// The 'hydrated' criterion object. This is NOT serialized because it already exists as an object in the bank.
        /// </summary>
        [JsonIgnore, JsonProperty("CriterionRef")]
        public Criterion Criterion { get; set; }

        /// <summary>
        /// Constructs an empty section.
        /// </summary>
        public Section()
        {
            Content = new ContentExamSection();
            Fields = new List<Field>();
        }

        protected Section(Section source)
            : this()
        {
            source.ShallowCopyTo(this);

            Content = source.Content?.Clone();
            Fields = source.Fields.EmptyIfNull().Select(x => x.Clone()).ToList();
        }

        public Section Clone() => new Section(this);

        #region Methods (serialization)

        public bool ShouldSerializeContent()
        {
            return Content != null && !Content.IsEmpty;
        }

        #endregion
    }
}
