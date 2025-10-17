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
    /// An exam form is a specific set of questions (grouped into sections) for a student or candidate to answer.
    /// </summary>
    [Serializable]
    public class Form : IHasAssetNumber, IHasVersionControl<Form>
    {
        /// <summary>
        /// The specification that contains this form defines the requirements that the form must satisfy.
        /// </summary>
        [JsonIgnore]
        public Specification Specification { get; set; }

        /// <summary>
        /// Uniquely identifies the form.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// The referenced gradebook
        /// </summary>
        public Guid? Gradebook { get; set; }

        /// <summary>
        /// The catalog reference code for the form.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The internal name used to uniquely identify the form for filing purposes.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Hook { get; set; }

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        /// <summary>
        /// Reference to the source of the content and/or configuration for this form.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Identifies the originating platform and/or record for this form. When this property is used, it should 
        /// ideally contain a fully qualified URL or API path.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Does this form have any diagrams associated with the questions it contains?
        /// </summary>
        public bool HasDiagrams { get; set; }

        /// <summary>
        /// Does this form allow a third-party assessor to submit an assessment of a learner's performance?
        /// </summary>
        /// <remarks>
        /// Note this is false by default because learners assess themselves when they complete typical assessment forms.
        /// </remarks>
        public bool ThirdPartyAssessmentIsEnabled { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ReferenceMaterialType HasReferenceMaterials { get; set; }

        /// <summary>
        /// Every form is assigned a unique asset number in the organization's inventory.
        /// </summary>
        public int Asset { get; set; }

        /// <summary>
        /// Every asset number has a unique version number.
        /// </summary>
        public int AssetVersion => this.GetVersionNumber();

        /// <summary>
        /// The ordinal position of this form in the specification that contains it.
        /// </summary>
        [JsonIgnore]
        public int Sequence => Specification.Forms.GetSequence(this);


        /// <summary>
        /// Message identifier to notify administrators when an attempt is started.
        /// </summary>
        [JsonProperty(PropertyName = "WhenAttemptStartedNotifyAdminMessageIdentifier")]
        public Guid? WhenAttemptStartedNotifyAdminMessageIdentifier { get; set; }

        /// <summary>
        /// Message identifier to notify administrators when an attempt is completed.
        /// </summary>
        [JsonProperty(PropertyName = "WhenAttemptCompletedNotifyAdminMessageIdentifier")]
        public Guid? WhenAttemptCompletedNotifyAdminMessageIdentifier { get; set; }


        /// <summary>
        /// Forms need support for multilingual titles, summaries, materials, etc.
        /// </summary>
        public ContentExamForm Content { get; set; }

        /// <summary>
        /// Form classification attributes are encapsulated in a single property for improved readability.
        /// </summary>
        public FormClassification Classification { get; set; }

        /// <summary>
        /// Invigilation attributes for the form.
        /// </summary>
        public FormInvigilation Invigilation { get; set; }

        /// <summary>
        /// Publication properties for the form.
        /// </summary>
        public FormPublication Publication { get; set; }

        /// <summary>
        /// A form addendum is the sequence of asset numbers (in order) for the attachments that need to be printed 
        /// whenever the form is printed. It may be an empty list, or it may contain any subset of the attachments 
        /// contained in the bank.
        /// </summary>
        public FormAddendum Addendum { get; set; }

        /// <summary>
        /// A form can be assigned to any number of categories. This is helpful for searching, indexing, and 
        /// organizing forms.
        /// </summary>
        public List<string> Categories { get; set; }

        /// <summary>
        /// A form contains fields grouped into sections. A section uses a special-purpose filter (i.e. question sieve)
        /// to draw from a pool of question items (i.e. question set).
        /// </summary>
        public List<Section> Sections { get; set; }

        [JsonIgnore]
        public Form NextVersion { get; set; }

        public Form PreviousVersion { get; set; }

        DateTimeOffset? IHasVersionControl<Form>.FirstPublished => Publication.FirstPublished;

        public Guid[] StaticQuestionOrder { get; set; }

        public DateTimeOffset? StaticQuestionOrderVerified { get; set; }

        public string[] Languages { get; set; }

        /// <summary>
        /// Constructs an empty form.
        /// </summary>
        public Form()
        {
            Categories = new List<string>();
            Addendum = new FormAddendum();
            Classification = new FormClassification();
            Content = new ContentExamForm();
            Invigilation = new FormInvigilation();
            Publication = new FormPublication();
            Sections = new List<Section>();
        }

        protected Form(Form source)
        {
            source.ShallowCopyTo(this);

            Content = source.Content.Clone();
            Classification = source.Classification.Clone();
            Invigilation = source.Invigilation.Clone();
            Publication = source.Publication.Clone();
            Addendum = source.Addendum.Clone();
            PreviousVersion = source.PreviousVersion?.Clone();
            Categories = source.Categories.ToList();
            Sections = source.Sections.EmptyIfNull().Select(x => x.Clone()).ToList();

            RestoreReferences();
        }

        public Form Clone() => new Form(this);

        public Question FindQuestion(Guid id)
        {
            var question = Specification.Bank.FindQuestion(id);

            return question != null && question.Fields.Where(x => x.Section.Form.Identifier == Identifier).Any() ? question : null;
        }

        public List<Question> GetQuestions()
        {
            if (Specification.Type == SpecificationType.Static)
            {
                return Sections.SelectMany(x => x.Fields.Select(y => y.Question)).ToList();
            }
            else if (Specification.Type == SpecificationType.Dynamic)
            {
                return Specification.Criteria.Count > 0
                    ? Specification.Criteria.SelectMany(x => x.Sets.SelectMany(y => y.Questions)).ToList()
                    : Specification.Bank.Sets.SelectMany(x => x.Questions).ToList();
            }
            else
            {
                throw new ArgumentException("Unkown specification type: " + Specification.Type);
            }
        }

        /// <remarks>
        /// The documentation for SelectMany does not explicitly guarantee the order of the elements in the result, 
        /// therefore we build the array explicitly to ensure the order is preserved.
        /// </remarks>
        public Guid[] GetStaticFormQuestionIdentifiersInOrder()
        {
            if (Specification.Type != SpecificationType.Static)
                return new Guid[0];

            var questions = new List<Guid>();
            foreach (var section in Sections)
                foreach (var field in section.Fields)
                    questions.Add(field.QuestionIdentifier);

            return questions.ToArray();
        }

        public bool ContainsQuestion(Guid id)
        {
            return Specification.Type == SpecificationType.Static
                ? Sections.Any(x => x.Fields.Any(y => y.QuestionIdentifier == id))
                : GetQuestions().Any(x => x.Identifier == id);
        }

        #region Methods (serialization)

        public bool ShouldSerializeCategories()
            => Categories.Count > 0;

        public bool ShouldSerializeVersion()
            => AssetVersion > 0;

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            RestoreReferences();
        }

        internal void RestoreReferences()
        {
            this.RestoreNextVersionReferences();

            foreach (var section in Sections)
            {
                section.Form = this;
                foreach (var field in section.Fields)
                    field.Section = section;
            }
        }

        #endregion

        #region Methods (overriden)

        public override string ToString()
        {
            return $"Form {Specification?.Letter}{Sequence} - {Name ?? "(Untitled)"}";
        }

        #endregion
    }
}
