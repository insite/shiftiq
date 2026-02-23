using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ReorderFields : Command
    {
        public Guid Section { get; set; }

        /// <summary>
        /// The dictionary item key is the old ordinal position for the field, and the item value is the new ordinal.
        /// </summary>
        public Dictionary<int, int> Sequences { get; }

        public ReorderFields(Guid bank, Guid section, Dictionary<int, int> sequences)
        {
            AggregateIdentifier = bank;
            Section = section;
            Sequences = sequences;
        }
    }
}
