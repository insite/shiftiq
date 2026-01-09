using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ReorderSections : Command
    {
        public Guid Form { get; set; }

        public Dictionary<int, int> Sequences { get; }

        public ReorderSections(Guid bank, Guid form, Dictionary<int, int> sequences)
        {
            AggregateIdentifier = bank;
            Form = form;
            Sequences = sequences;
        }
    }
}
