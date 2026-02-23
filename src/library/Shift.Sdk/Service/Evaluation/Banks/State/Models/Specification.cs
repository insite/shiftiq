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
    /// A specification represents a set of rules to determine the content and behaviour of an exam form.
    /// </summary>
    [Serializable]
    public class Specification : IHasAssetNumber
    {
        /// <summary>
        /// The bank that contains the specification.
        /// </summary>
        [JsonIgnore]
        public BankState Bank { get; set; }

        /// <summary>
        /// Specifications are organized into types. Certain functions apply to one type and not to another type.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SpecificationType Type { get; set; } = SpecificationType.Dynamic;

        /// <summary>
        /// Indicates if the exam form is high-stakes or low-stakes.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ConsequenceType Consequence { get; set; } = ConsequenceType.Low;

        /// <summary>
        /// Defines the behavior of time management during an exam when tabs are enabled
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SpecificationTabTimeLimit TabTimeLimit { get; set; } = SpecificationTabTimeLimit.Disabled;

        /// <summary>
        /// Indicates if the tab time limit parameter can be applied
        /// </summary>
        [JsonIgnore]
        public bool IsTabTimeLimitAllowed => Type == SpecificationType.Static && SectionsAsTabsEnabled && !TabNavigationEnabled;

        /// <summary>
        /// Uniquely identifies the specification.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// The internal name used to uniquely identify this specification for filing purposes.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        /// <summary>
        /// Specifications need support for multilingual titles, summaries, materials, etc.
        /// </summary>
        public ContentExamSpecification Content { get; set; }

        /// <summary>
        /// Every specification is assigned a unique asset number in the organization's inventory.
        /// </summary>
        public int Asset { get; set; }

        /// <summary>
        /// The ordinal position of this specification in the bank that contains it.
        /// </summary>
        [JsonIgnore]
        public int Sequence => 1 + Bank.Specifications.IndexOf(this);

        /// <summary>
        /// The maximum number of forms allowed in this specification.
        /// </summary>
        public int FormLimit { get; set; }

        /// <summary>
        /// The maximum number of questions allowed on the exam form.
        /// </summary>
        public int QuestionLimit { get; set; }

        /// <summary>
        /// If SectionsAsTabsEnabled is true then each Section will displayed in a separate tab at the top of the form.
        /// </summary>
        public bool SectionsAsTabsEnabled { get; set; }

        /// <summary>
        /// If TabNavigationEnabled is true and the sections are displayed in tabs then the user will not be able to move between tabs by clicking on them but only by clicking the Next button at the bottom of the form.
        /// </summary>
        public bool TabNavigationEnabled { get; set; }

        /// <summary>
        /// If SingleQuestionPerTabEnabled is true, TabNavigationEnabled feature will be extended by displaying only one question at a time on the current tab.
        /// </summary>
        public bool SingleQuestionPerTabEnabled { get; set; }

        /// <summary>
        /// The rules for calculation and display of exam submission scores.
        /// </summary>
        public ScoreCalculation Calculation { get; set; }

        /// <summary>
        /// The forms controlled by this specification.
        /// </summary>
        public List<Form> Forms { get; set; }

        /// <summary>
        /// The sieves contained by this specification.
        /// </summary>
        public List<Criterion> Criteria { get; set; }

        /// <summary>
        /// Constructs an empty specification.
        /// </summary>
        public Specification()
        {
            Calculation = new ScoreCalculation();
            Content = new ContentExamSpecification();
            Forms = new List<Form>();
            Criteria = new List<Criterion>();
        }

        public Specification Clone()
        {
            var clone = new Specification();

            this.ShallowCopyTo(clone);

            clone.Content = Content.Clone();
            clone.Calculation = Calculation.Clone();
            clone.Forms = Forms.EmptyIfNull().Select(x => x.Clone()).ToList();
            clone.Criteria = Criteria.EmptyIfNull().Select(x => x.Clone()).ToList();

            clone.RestoreReferences();

            return clone;
        }

        #region Methods (helpers)

        public IEnumerable<Form> EnumerateAllForms() =>
            Forms.SelectMany(x => x.EnumerateAllVersions());

        public Form FindForm(Guid id)
        {
            return Forms.SingleOrDefault(x => x.Identifier == id);
        }

        #endregion

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            RestoreReferences();
        }

        internal void RestoreReferences()
        {
            var sectionCriterionMapping = new Dictionary<Guid, List<Section>>();

            foreach (var form in EnumerateAllForms())
            {
                form.Specification = this;

                foreach (var section in form.Sections)
                {
                    if (!sectionCriterionMapping.ContainsKey(section.CriterionIdentifier))
                        sectionCriterionMapping.Add(section.CriterionIdentifier, new List<Section>());

                    sectionCriterionMapping[section.CriterionIdentifier].Add(section);
                }
            }

            foreach (var criterion in Criteria)
            {
                criterion.Specification = this;

                if (sectionCriterionMapping.TryGetValue(criterion.Identifier, out var sections))
                {
                    foreach (var section in sections)
                    {
                        section.Criterion = criterion;
                        criterion.Sections.Add(section);
                    }
                }
            }
        }

        #endregion

        #region Methods (overriden)

        public override string ToString()
        {
            return $"{Type.GetName()} Specification {Sequence} - {Name ?? "(Untitled)"}";
        }

        #endregion
    }
}
