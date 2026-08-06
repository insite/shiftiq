using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

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
        /// Configuration for this section's behavior when presented as a tab.
        /// </summary>
        public SectionTabConfiguration TabConfiguration { get; set; }

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
            TabConfiguration = new SectionTabConfiguration();
        }

        protected Section(Section source)
            : this()
        {
            source.ShallowCopyTo(this);

            Content = source.Content?.Clone();
            Fields = source.Fields.EmptyIfNull().Select(x => x.Clone()).ToList();
            TabConfiguration = source.TabConfiguration.Clone();
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
