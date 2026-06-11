using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyOptionListsReordered : Change
    {
        public Guid Question { get; set; }

        /// <summary>
        /// The dictionary key is the new ordinal position for the question that contains the lists. The dictionary 
        /// value is the identifier for a list.
        /// </summary>
        public Dictionary<Guid, int> Sequences { get; }

        public SurveyOptionListsReordered(Guid question, Dictionary<Guid, int> sequences)
        {
            Question = question;
            Sequences = sequences;
        }
    }
}