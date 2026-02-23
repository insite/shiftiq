using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ReorderOptions : Command
    {
        public Guid Question { get; set; }
        public Dictionary<int, int> Sequences { get; }

        public ReorderOptions(Guid bank, Guid question)
        {
            AggregateIdentifier = bank;
            Question = question;
            Sequences = new Dictionary<int, int>();
        }
    }
}
