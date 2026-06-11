using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FieldsReordered : Change
    {
        public Guid Section { get; set; }

        /// <summary>
        /// The dictionary item key is the new ordinal position for the question. The dictionary item value is the 
        /// asset number for the question.
        /// </summary>
        public Dictionary<int, int> Sequences { get; }

        public FieldsReordered(Guid section, Dictionary<int, int> sequences)
        {
            Section = section;
            Sequences = sequences;
        }
    }
}
