using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyQuestionsReordered : Change
    {
        /// <summary>
        /// The dictionary value is the identifier for a question, and the dictionary key is the ordinal position for 
        /// that question in the containing form.
        /// </summary>
        public Dictionary<Guid, int> Sequences { get; }

        public SurveyQuestionsReordered(Dictionary<Guid, int> sequences)
        {
            Sequences = sequences;
        }
    }
}