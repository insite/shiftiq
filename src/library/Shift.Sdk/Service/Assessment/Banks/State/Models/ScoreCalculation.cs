using System;
using System.Globalization;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Represents the settings used to calculate and display the score for an exam submission.
    /// </summary>
    [Serializable]
    public class ScoreCalculation
    {
        /// <summary>
        /// What information is disclosed to a student/candidate after completing an exam submission?
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public DisclosureType Disclosure { get; set; }

        /// <summary>
        /// What is the minimum score required to pass the exam?
        /// </summary>
        public decimal PassingScore { get; set; }

        /// <summary>
        /// If the student/candidate passes the exam, what weight is applied to the submission score?
        /// </summary>
        public decimal SuccessWeight { get; set; }

        /// <summary>
        /// If the student/candidate fails the exam, what weight is applied to the submission score?
        /// </summary>
        public decimal FailureWeight { get; set; }

        public ScoreCalculation Clone()
        {
            var clone = new ScoreCalculation();

            this.ShallowCopyTo(clone);

            return clone;
        }

        /// <summary>
        /// Sets the disclosure type from a string value. (Here we need some flexibility in case the text does not 
        /// exactly matching the enumeration name.)
        /// </summary>
        public void SetDisclosure(string text)
        {
            if (string.IsNullOrEmpty(text))
                Disclosure = DisclosureType.Full;

            else if (text.StartsWith("Full", true, CultureInfo.CurrentCulture))
                Disclosure = DisclosureType.Full;

            else if (text.StartsWith("Score", true, CultureInfo.CurrentCulture))
                Disclosure = DisclosureType.Score;

            else if (text.StartsWith("Answers", true, CultureInfo.CurrentCulture))
                Disclosure = DisclosureType.Answers;

            else
                Disclosure = DisclosureType.None;
        }
    }
}