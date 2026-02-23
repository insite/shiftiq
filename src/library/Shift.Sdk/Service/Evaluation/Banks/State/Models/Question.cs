using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Represents a question item in a question bank.
    /// </summary>
    [Serializable]
    public class Question : IHasAssetNumber, IHasVersionControl<Question>
    {
        /// <summary>
        /// The question set that contains this question item.
        /// </summary>
        [JsonIgnore]
        public Set Set { get; set; }

        /// <summary>
        /// What method is used to calculate the points for this question?
        /// </summary>
        [DefaultValue(QuestionCalculationMethod.Default)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public QuestionCalculationMethod CalculationMethod { get; set; }

        /// <summary>
        /// The colored flag assigned to the question.
        /// </summary>
        [DefaultValue(FlagType.None)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public FlagType Flag { get; set; }

        /// <summary>
        /// What type of question item is this?
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public QuestionItemType Type { get; set; }

        /// <summary>
        /// Uniquely identifies the question.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Uniquely identifies the standard (e.g. competency) evaluated by the question item. If the question does not 
        /// evaluate any standard then Guid.Empty is assumed as a sentinel value.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Guid Standard { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid[] SubStandards { get; set; }

        /// <summary>
        /// If this question was copied from another question, then this property identifies the original question.
        /// </summary>
        public Guid? Source { get; set; }

        /// <summary>
        /// If this question was copied from another question, then this property identifies the original question.
        /// </summary>
        public Guid? Rubric { get; set; }

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        /// <summary>
        /// The current condition of the question item.
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Every question is assigned a unique asset number in the organization's inventory.
        /// </summary>
        public int Asset { get; set; }

        /// <summary>
        /// Every asset number has a unique version number.
        /// </summary>
        public int AssetVersion => this.GetVersionNumber();

        /// <summary>
        /// The ordinal position of this question in the bank after flattening its sets.
        /// </summary>
        [JsonIgnore]
        public virtual int BankIndex { get; set; }

        /// <summary>
        /// The ordinal position of this question in the set that contains it.
        /// </summary>
        [JsonIgnore]
        public virtual int Sequence => Set.Questions.GetSequence(this);

        /// <summary>
        /// The cut-score for this question item.
        /// </summary>
        public decimal? CutScore { get; set; }

        /// <summary>
        /// The maximum number of points awarded for correct answers to this question.
        /// </summary>
        public decimal? Points { get; set; }

        /// <summary>
        /// Questions need support for multilingual titles, rationales, etc.
        /// </summary>
        public ContentExamQuestion Content { get; set; }

        /// <summary>
        /// Question item classification attributes are encapsulated in a single property for improved readability.
        /// </summary>
        public QuestionClassification Classification { get; set; }

        /// <summary>
        /// Controls the display/layout of the options in the question. 
        /// </summary>
        public OptionLayout Layout { get; set; }

        /// <summary>
        /// The randomization settings for the options in the question. Any field that displays this question is 
        /// expected to use the same randomization settings for display of the options it contains.
        /// </summary>
        public Randomization Randomization { get; set; }

        /// <summary>
        /// The fields that display this question.
        /// </summary>
        [JsonIgnore]
        public List<Field> Fields { get; set; }

        /// <summary>
        /// Key is the FormId
        /// Value is the GradeItemId
        /// </summary>
        public Dictionary<Guid, Guid> GradeItems { get; set; }

        /// <summary>
        /// The matching list of pairs for the question. This applies only to questions where Type = Matching.
        /// </summary>
        public MatchingList Matches { get; set; }

        /// <summary>
        /// The options contained by the question.
        /// </summary>
        public List<Option> Options { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LikertMatrix Likert { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Hotspot Hotspot { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ComposedVoice ComposedVoice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Ordering Ordering { get; set; }

        [DefaultValue(PublicationStatus.Drafted)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public PublicationStatus PublicationStatus { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? FirstPublished { get; set; }

        public HashSet<Guid> AttachmentIdentifiers { get; set; }

        [JsonIgnore]
        public virtual Question NextVersion { get; set; }

        public virtual Question PreviousVersion { get; set; }

        /// <summary>
        /// Returns the list of comments posted about this question.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<Comment> Comments
        {
            get
            {
                if (Set == null || Set.Bank == null)
                    return new List<Comment>();

                var fieldIds = new HashSet<Guid>(Fields.Select(x => x.Identifier));
                return Set.Bank.Comments
                    .Where(
                        x => x.Type == CommentType.Question && x.Subject == Identifier
                          || x.Type == CommentType.Field && fieldIds.Contains(x.Subject))
                    .ToList();
            }
        }

        /// <summary>
        /// Constructs an empty question.
        /// </summary>
        public Question()
        {
            Classification = new QuestionClassification();
            CalculationMethod = QuestionCalculationMethod.Default;
            Content = new ContentExamQuestion();
            Fields = new List<Field>();
            GradeItems = new Dictionary<Guid, Guid>();
            Layout = new OptionLayout();
            Options = new List<Option>();
            Randomization = new Randomization();
            Matches = new MatchingList();
            AttachmentIdentifiers = new HashSet<Guid>();
            Likert = new LikertMatrix { Question = this };
            Hotspot = new Hotspot();
            ComposedVoice = new ComposedVoice();
            Ordering = new Ordering();
        }

        protected Question(Question source, bool cloneVersions, bool cloneIdentifiers)
        {
            source.ShallowCopyTo(this);

            BankIndex = 0;
            Content = source.Content.Clone();
            Fields = new List<Field>();
            GradeItems = new Dictionary<Guid, Guid>();
            Classification = source.Classification.Clone();
            Layout = source.Layout.Clone();
            Randomization = source.Randomization.Clone();
            Options = source.Options.EmptyIfNull().Select(x => x.Clone()).ToList();
            Matches = source.Matches.Clone();
            AttachmentIdentifiers = new HashSet<Guid>(source.AttachmentIdentifiers.EmptyIfNull());
            PreviousVersion = cloneVersions ? source.PreviousVersion?.Clone(cloneVersions) : null;
            Likert = source.Likert.Clone(cloneIdentifiers);
            Hotspot = source.Hotspot.Clone();
            ComposedVoice = source.ComposedVoice.Clone();
            Ordering = source.Ordering.Clone();

            RestoreReferences();
        }

        public Question Clone(bool cloneVersions = false, bool cloneIdentifiers = true) => new Question(this, cloneVersions, cloneIdentifiers);

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            RestoreReferences();
        }

        internal void RestoreReferences()
        {
            this.RestoreNextVersionReferences();

            foreach (var option in Options)
                option.Question = this;

            Likert.Question = this;

            if (Type.IsHotspot())
                Hotspot.SetQuestionType(Type);
        }

        public bool ShouldSerializeLayout()
        {
            return Layout.Type == OptionLayoutType.Table;
        }

        public bool ShouldSerializeMatches()
        {
            return Type == QuestionItemType.Matching && !Matches.IsEmpty;
        }

        public bool ShouldSerializeRandomization()
        {
            return Randomization.Enabled;
        }

        public bool ShouldSerializeOptions()
        {
            return Options.Count > 0;
        }

        public bool ShouldSerializeLikert()
        {
            return Type == QuestionItemType.Likert && !Likert.IsEmpty;
        }

        public bool ShouldSerializeHotspot()
        {
            return Type.IsHotspot() && !Hotspot.IsEmpty;
        }

        public bool ShouldSerializeComposedVoice()
        {
            return Type == QuestionItemType.ComposedVoice && !ComposedVoice.IsEmpty;
        }

        public bool ShouldSerializeOrdering()
        {
            return Type == QuestionItemType.Ordering && !Ordering.IsEmpty;
        }

        #endregion

        #region Methods (display)

        public override string ToString()
        {
            return $"Question {BankIndex + 1} - {Markdown.ToText(Content.Title.Default)}";
        }

        #endregion

        #region Methods (helpers)

        private const double PointsDecimalScale = 2d;
        private static readonly decimal PointsDecimalDivisor = (decimal)Math.Pow(10, PointsDecimalScale);
        private static readonly decimal PointsMinValue = 1 / PointsDecimalDivisor;

        public void CalculatePoints()
        {
            if (Type == QuestionItemType.SingleCorrect)
            {
                Points = Options.Count == 0 ? 0 : Options.Max(x => x.Points);
            }
            else if (Type == QuestionItemType.TrueOrFalse)
            {
                Points = Options.Count == 0 ? 0 : Options.Max(x => x.Points);
            }
            else if (Type.IsComposed())
            {
                Points = Options.Count == 0 ? 0 : Options.Sum(x => x.Points);
            }
            else if (Type != QuestionItemType.Matching && CalculationMethod == QuestionCalculationMethod.LimitedCorrect)
            {
                var correctOptionCount = Options.Count(x => x.IsTrue == true);
                var pointsPerOption = Points.HasValue && correctOptionCount != 0 ? Math.Truncate(Points.Value / correctOptionCount * PointsDecimalDivisor) / PointsDecimalDivisor : 0;

                foreach (var option in Options)
                    option.Points = option.IsTrue == true ? pointsPerOption : 0;

                var firstCorrectOption = Options.FirstOrDefault(x => x.IsTrue == true);

                if (firstCorrectOption != null && pointsPerOption > 0)
                    firstCorrectOption.Points = Points.Value - pointsPerOption * (correctOptionCount - 1);
            }
            else if (CalculationMethod != QuestionCalculationMethod.Default)
            {
                var items = Type == QuestionItemType.Matching ? (IReadOnlyList<IQuestionAnswer>)Matches.Pairs : Options;

                if (Points.HasValue && items.Count > 0)
                {
                    var pointsPerOption = Math.Truncate(Points.Value / items.Count * PointsDecimalDivisor) / PointsDecimalDivisor;
                    var roundingError = Points.Value - pointsPerOption * items.Count;
                    var errorFixCount = roundingError / PointsMinValue;
                    var enumerator = errorFixCount > 0
                        ? items
                            .Select((x, i) => new { Sequence = i, Item = x })
                            .OrderByDescending(x => x.Item.IsTrue).ThenBy(x => x.Sequence)
                            .Select(x => x.Item)
                            .GetEnumerator()
                        : items.GetEnumerator();

                    while (errorFixCount > 0 && enumerator.MoveNext())
                    {
                        enumerator.Current.Points = pointsPerOption + PointsMinValue;
                        errorFixCount--;
                    }

                    while (enumerator.MoveNext())
                        enumerator.Current.Points = pointsPerOption;
                }
                else
                {
                    foreach (var o in items)
                        o.Points = 0;
                }
            }
            else if (Type == QuestionItemType.Matching)
            {
                Points = Matches.Pairs.Count == 0 ? 0 : Matches.Pairs.Sum(x => x.Points);
            }
            else if (Type == QuestionItemType.Likert)
            {
                Points = Likert.Points ?? 0;
            }
            else if (Type.IsHotspot())
            {
                Points = Hotspot.Options.OrderByDescending(x => x.Points).Take(Hotspot.PinLimit).Sum(x => x.Points);
            }
            else if (Type == QuestionItemType.Ordering)
            {
                Points = Ordering.Solutions.Count == 0 ? 0 : Ordering.Solutions.Max(x => x.Points);
            }
            else
            {
                Points = Options.Count == 0 ? 0 : Options.Sum(x => x.Points);
            }
        }

        #endregion
    }
}
