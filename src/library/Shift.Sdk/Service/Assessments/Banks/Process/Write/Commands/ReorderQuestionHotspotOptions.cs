using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ReorderQuestionHotspotOptions : Command
    {
        public Guid Question { get; set; }
        public Dictionary<Guid, int> OptionsOrder { get; set; }

        public ReorderQuestionHotspotOptions(Guid bank, Guid question, Dictionary<Guid, int> optionsOrder)
        {
            AggregateIdentifier = bank;
            Question = question;
            OptionsOrder = optionsOrder;
        }
    }
}
