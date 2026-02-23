using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// A field is a reference to a specific question item from within a specific form (in a specific section).
    /// </summary>
    [Serializable]
    public class Field
    {
        /// <summary>
        /// Uniquely identifies the field.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Uniquely identifies the question being referenced.
        /// </summary>
        [JsonProperty("Question")]
        public Guid QuestionIdentifier { get; set; }

        /// <summary>
        /// Returns an alphabetic equivalent for the integer sequence (e.g. A = 1, B = 2, C = 3).
        /// </summary>
        [JsonIgnore]
        public string Letter => Calculator.ToBase26(Sequence);

        /// <summary>
        /// The ordinal position of this field in the section that contains it.
        /// </summary>
        [JsonIgnore]
        public int Sequence => 1 + Section.Fields.IndexOf(this);

        /// <summary>
        /// The ordinal position of this field in the form that contains it.
        /// </summary>
        [JsonIgnore]
        public int FormSequence
        {
            get
            {
                var result = Sequence;

                foreach (var section in Section.Form.Sections)
                {
                    if (section.Identifier == Section.Identifier)
                        break;

                    result += section.Fields.Count;
                }

                return result;
            }
        }

        /// <summary>
        /// The section that contains this field.
        /// </summary>
        [JsonIgnore]
        public Section Section { get; set; }

        /// <summary>
        /// The 'hydrated' question object. This is NOT serialized because it already exists as an object in the bank.
        /// </summary>
        [JsonIgnore, JsonProperty("QuestionRef")]
        public Question Question { get; set; }

        public Field Clone()
        {
            var clone = new Field();

            this.ShallowCopyTo(clone);

            return clone;
        }

        #region Methods (serialization)

        #endregion
    }
}
