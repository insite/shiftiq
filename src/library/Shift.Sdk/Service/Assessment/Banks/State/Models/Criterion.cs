using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// A criterion represents a set of rules for selecting and filtering the question items from one or more question 
    /// sets. Each section on an exam form uses a criterion to determine the questions it contains and displays.
    /// </summary>
    [Serializable]
    public class Criterion
    {
        /// <summary>
        /// A criterion is either a Tag Selector (basic filter) or a Pivot Table (advanced filter).
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public CriterionFilterType FilterType { get; set; }

        /// <summary>
        /// Uniquely identifies the criterion.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Uniquely identifies the sets being referenced.
        /// </summary>
        [JsonProperty("Set")]
        public List<Guid> SetIdentifiers { get; set; }

        /// <summary>
        /// The internal name used to uniquely identify the criterion for filing purposes.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The old original ExamForm.DisplayFilter.
        /// </summary>
        public string TagFilter { get; set; }

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        /// <summary>
        /// The number of available questions in all the sets from which this criterion selects and filters.
        /// </summary>
        [JsonIgnore]
        public int QuestionCount => Sets.SelectMany(x => x.Questions).Count();

        /// <summary>
        /// The maximum number of questions allowed on an exam form from the question set to which this criterion applies.
        /// </summary>
        public int QuestionLimit { get; set; }

        /// <summary>
        /// The ordinal position of this criterion in the specification that contains it.
        /// </summary>
        [JsonIgnore]
        public int Sequence => 1 + Specification.Criteria.IndexOf(this);

        /// <summary>
        /// The desired weighting for the question set to which this criterion applies, within the overall specification. 
        /// The sum of all SetWeight values for the criteria in a specification must equal 1 (i.e. 100 percent).
        /// </summary>
        public decimal SetWeight { get; set; }

        /// <summary>
        /// The list of sections controlled by this criterion.
        /// </summary>
        [JsonIgnore]
        public List<Section> Sections { get; set; }

        /// <summary>
        /// The 'hydrated' set objects. This is NOT serialized because it already exists as an object in the bank.
        /// </summary>
        [JsonIgnore, JsonProperty("SetRefs")]
        public List<Set> Sets { get; set; }

        /// <summary>
        /// The specification that contains this criterion.
        /// </summary>
        [JsonIgnore]
        public Specification Specification { get; set; }

        /// <summary>
        /// The new pivot-table filter.
        /// </summary>
        public PivotTable PivotFilter { get; set; }

        /// <summary>
        /// Constructs an empty criterion.
        /// </summary>
        public Criterion()
        {
            SetIdentifiers = new List<Guid>();

            PivotFilter = new PivotTable();

            Sections = new List<Section>();
            Sets = new List<Set>();
        }

        public Criterion Clone()
        {
            var clone = new Criterion();

            this.ShallowCopyTo(clone);

            clone.SetIdentifiers = SetIdentifiers.ToList();
            clone.PivotFilter = PivotFilter.CloneJson();

            return clone;
        }

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            UpdateFilterType();
        }

        public bool ShouldSerializePivotFilter()
        {
            return PivotFilter != null && !PivotFilter.IsEmpty;
        }

        public void UpdateFilterType()
        {
            if (!string.IsNullOrEmpty(TagFilter))
                FilterType = CriterionFilterType.Tag;
            else if (ShouldSerializePivotFilter())
                FilterType = CriterionFilterType.Pivot;
            else
                FilterType = CriterionFilterType.All;
        }

        #endregion

        #region Methods (overriden)

        [JsonIgnore]
        public string Title => string.IsNullOrEmpty(Name) ? string.Join("; ", Sets.Select(x => x.Name)) : Name;

        public override string ToString()
        {
            return $"Criterion {Sequence}";
        }

        #endregion
    }
}
