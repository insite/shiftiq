using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeBankType : Command
    {
        public string Type { get; set; }

        public ChangeBankType(Guid bank, string type)
        {
            AggregateIdentifier = bank;
            Type = type;
        }
    }
}