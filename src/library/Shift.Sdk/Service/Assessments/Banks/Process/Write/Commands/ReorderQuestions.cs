using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ReorderQuestions : Command
    {
        public Guid Set { get; set; }

        /// <summary>
        /// The dictionary item key is the new ordinal position for the question. The dictionary item value is the 
        /// asset number for the question.
        /// </summary>
        public Dictionary<int, int> Sequences { get; }

        public ReorderQuestions(Guid bank, Guid set, Dictionary<int, int> sequences)
        {
            AggregateIdentifier = bank;
            Set = set;
            Sequences = sequences;
        }
    }
}
