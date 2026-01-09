using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyOptionItemsReordered : Change
    {
        public Guid List { get; set; }

        /// <summary>
        /// The dictionary key is the new ordinal position for the list that contains the items. The dictionary value is 
        /// the identifier for an item.
        /// </summary>
        public Dictionary<Guid, int> Sequences { get; }

        public SurveyOptionItemsReordered(Guid list, Dictionary<Guid, int> sequences)
        {
            List = list;
            Sequences = sequences;
        }
    }
}